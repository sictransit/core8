using Core8.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace Core8.Peripherals.Teletype
{
    public static class SVGPunch 
    {
        private const int SPACING = 100; // 1/1000 inches
        private const int DATA_WIDTH = 72;
        private const int FEEDER_WIDTH = 46;

        public static string Punch(byte[] data, string label, int wrap = 80, bool cutLeader = true)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            label ??= string.Empty;

            if (wrap < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(wrap));
            }

            if (cutLeader)
            {
                var trailer = data.Reverse().TakeWhile(x => IsLeaderTrailer(x)).ToArray();

                if (trailer.Length != 0)
                {
                    data = trailer.Concat(data.SkipWhile(x => IsLeaderTrailer(x))).ToArray();
                }
            }

            wrap = wrap == 0 ? data.Length : wrap;

            var paperWidth = wrap * SPACING;
            var paperHeight = SPACING * 10;

            var definition = new XElement(SVGDeclarations.svg + "defs", Hole(true), Hole(false), data.Distinct().OrderBy(x => x).Select(x => CreateRowShape(x)));

            var totalHeight = SPACING;

            var strips = new List<XElement>();

            var rows = new List<XElement>();

            var padding = new byte[(wrap - data.Length % wrap) % wrap];

            var content = data.Concat(padding);

            foreach (var chunk in content.ChunkBy(wrap))
            {
                var strip = CreatePaper(SPACING, totalHeight, paperWidth, paperHeight);

                strips.Add(strip);

                var height = totalHeight;

                var row = new XElement(SVGDeclarations.svg + "g", chunk.Select((x, i) => UseRowShape(SPACING / 2, height, i, x)));

                rows.Add(row);

                totalHeight += paperHeight + SPACING;
            }

            var totalWidth = paperWidth + 2 * SPACING;

            var tape = new XElement(
                SVGDeclarations.svg + "svg",
                new XAttribute("width", $"{(totalWidth / 1000d).ToString(CultureInfo.InvariantCulture)}in"),
                new XAttribute("height", $"{(totalHeight / 1000d).ToString(CultureInfo.InvariantCulture)}in"),
                new XAttribute("viewBox", $"0 0 {totalWidth} {totalHeight}"),
                new XAttribute(XNamespace.Xmlns + "xlink", SVGDeclarations.xlink),
                LabelStyle,
                definition,
                strips,
                Label(label),
                rows);

            return tape.ToString();
        }

        private static bool IsLeaderTrailer(byte x) => x == 1 << 7;

        private static string ByteRowId(int b) => $"{SVGDeclarations.BYTE_ROW_PREFIX}{b}";

        private static XElement LabelStyle => new XElement(
            SVGDeclarations.svg + "style",
            ".label { font: 64px courier; fill: red; }"
            );

        private static XElement Label(string text) => new XElement(
            SVGDeclarations.svg + "text",
            new XAttribute("x", SPACING + SPACING / 2),
            new XAttribute("y", SPACING + SPACING / 2),
            new XAttribute("class", "label"), text
            );

        private static XElement CreateRowShape(int b) => new XElement(
            SVGDeclarations.svg + "g",
            new XAttribute("id", ByteRowId(b)),
            CreateRow(b)
            );

        private static XElement UseRowShape(int x, int y, int offset, int b) => new XElement(
            SVGDeclarations.svg + "use",
            new XAttribute(SVGDeclarations.xlink + "href", "#" + ByteRowId(b)),
            new XAttribute("x", (offset + 1) * SPACING + x),
            new XAttribute("y", y)
            );

        private static XElement CreatePaper(int x, int y, int width, int height) => new XElement(
            SVGDeclarations.svg + "g",
            new XElement(
                SVGDeclarations.svg + "rect",
                new XAttribute("x", x),
                new XAttribute("y", y),
                new XAttribute("width", width),
                new XAttribute("height", height),
                new XAttribute("fill", "#ffffaa")
                )
            );

        private static string HoleTypeID(bool isData) => isData ? "data" : "feeder";

        private static XElement Hole(bool isData) => new XElement(
            SVGDeclarations.svg + "circle",
            new XAttribute("fill", "black"),
            new XAttribute("stroke-width", "0"),
            new XAttribute("stroke", "#fff"),
            new XAttribute("r", isData ? DATA_WIDTH / 2 : FEEDER_WIDTH / 2),
            new XAttribute("id", HoleTypeID(isData))
            );

        private static XElement UseHole(int bit, bool isData) => new XElement(
            SVGDeclarations.svg + "use",
            new XAttribute(SVGDeclarations.xlink + "href", "#" + HoleTypeID(isData)),
            new XAttribute("y", (bit + 1) * SPACING)
            );

        private static IEnumerable<XElement> CreateRow(int data) => Enumerable.Range(0, 8)
            .Where(bit => (data >> bit & 1) == 1)
            .Select(bit => UseHole(bit + (bit > 2 ? 1 : 0), true))
            .Concat(new[] { UseHole(3, false) });
    }
}
