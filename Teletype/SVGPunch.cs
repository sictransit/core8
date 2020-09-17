using Core8.Model.Extensions;
using Core8.Peripherals.Teletype.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Core8.Peripherals.Teletype
{
    public class SVGPunch : SVGBase
    {
        private const int spacing = 100;
        private const int dataWidth = 72;
        private const int feederWidth = 46;

        public string Punch(byte[] data, string label, int wrap = 80)
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

            wrap = wrap == 0 ? data.Length : wrap;

            var paperWidth = wrap * spacing;
            var paperHeight = spacing * 10;

            var definition = new XElement(svg + "defs", Hole(true), Hole(false), data.Distinct().OrderBy(x => x).Select(x => CreateRowShape(x)));

            var totalHeight = spacing;

            var strips = new List<XElement>();

            var rows = new List<XElement>();

            foreach (var chunk in data.Concat(new byte[wrap - data.Length % wrap]).ChunkBy(wrap))
            {
                var strip = CreatePaper(spacing, totalHeight, paperWidth, paperHeight);

                strips.Add(strip);

                var height = totalHeight;

                var row = new XElement(svg + "g", chunk.Select((x, i) => UseRowShape(spacing / 2, height, i, x)));

                rows.Add(row);

                totalHeight += paperHeight + spacing;
            }

            var tape = new XElement(svg + "svg", new XAttribute("width", paperWidth + 2 * spacing), new XAttribute("height", totalHeight), new XAttribute(XNamespace.Xmlns + "xlink", xlink), LabelStyle, RowLabelStyle, definition, strips, Label(label), rows);

            return tape.ToString();
        }

        private static string ByteRowID(int b) => $"{ByteRowPrefix}{b}";

        private static XElement LabelStyle => new XElement(
            svg + "style",
            ".label { font: 64px courier; fill: red; }"
            );

        private static XElement RowLabelStyle => new XElement(
            svg + "style",
            ".rowLabel { font: 64px courier; fill: gray; }"
            );

        private static XElement Label(string text) => new XElement(
            svg + "text",
            new XAttribute("x", spacing + spacing / 2),
            new XAttribute("y", spacing + spacing / 2),
            new XAttribute("class", "label"), text
            );

        private static XElement CreateRowShape(int b) => new XElement(
            svg + "g",
            new XAttribute("id", ByteRowID(b)),
            CreateRow(b)
            );

        private static XElement UseRowShape(int x, int y, int offset, int b) => new XElement(
            svg + "use",
            new XAttribute(xlink + "href", "#" + ByteRowID(b)),
            new XAttribute("x", (offset + 1) * spacing + x),
            new XAttribute("y", y)
            );

        private static XElement CreatePaper(int x, int y, int width, int height) => new XElement(
            svg + "g",
            new XElement(
                svg + "rect",
                new XAttribute("x", x),
                new XAttribute("y", y),
                new XAttribute("width", width),
                new XAttribute("height", height),
                new XAttribute("fill", "#ffffaa")
                )
            );

        private static string HoleTypeID(bool isData) => isData ? "data" : "feeder";

        private static XElement Hole(bool isData) => new XElement(
            svg + "circle",
            new XAttribute("fill", "black"),
            new XAttribute("stroke-width", "0"),
            new XAttribute("stroke", "#fff"),
            new XAttribute("r", isData ? dataWidth / 2 : feederWidth / 2),
            new XAttribute("id", HoleTypeID(isData))
            );

        private static XElement UseHole(int bit, bool isData) => new XElement(
            svg + "use",
            new XAttribute(xlink + "href", "#" + HoleTypeID(isData)),
            new XAttribute("y", (bit + 1) * spacing)
            );

        private static IEnumerable<XElement> CreateRow(int data) => Enumerable.Range(0, 8)
            .Where(bit => (data >> bit & 1) == 1)
            .Select(bit => UseHole(bit + (bit > 2 ? 1 : 0), true))
            .Concat(new[] { UseHole(3, false) });
    }
}
