using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Core8
{
    public static class SVGPunch
    {
        private static readonly XNamespace svg = "http://www.w3.org/2000/svg";
        private static readonly XNamespace xlink = "http://www.w3.org/1999/xlink";

        private const int spacing = 100;
        private const int dataWidth = 72;
        private const int feederWidth = 46;

        public static string Punch(byte[] data, string label)
        {
            var width = (data.Length + 1) * spacing;
            var height = spacing * 10;

            var defs = new XElement(svg + "defs", Hole(true), Hole(false), data.Distinct().OrderBy(x => x).Select(x => CreateRowShape(x)));

            var paper = CreatePaper(width, height);

            var holes = new XElement(svg + "g", data.Select((x, i) => UseRowShape(i, x)));

            var tape = new XElement(svg + "svg", new XAttribute("width", width), new XAttribute("height", height), new XAttribute(XNamespace.Xmlns + "xlink", xlink), Style, defs, paper, Label(label), holes);

            return tape.ToString();
        }

        private static string ByteRowID(int b) => $"byte{b}";

        private static XElement Style => new XElement(svg + "style", ".label { font: 64px courier; fill: red; }");

        private static XElement Label(string text) => new XElement(svg + "text", new XAttribute("x", spacing), new XAttribute("y", spacing / 2), new XAttribute("class", "label"), text);

        private static XElement CreateRowShape(int b) => new XElement(
            svg + "g", 
            new XAttribute("id", ByteRowID(b)), 
            CreateRow(b)
            );

        private static XElement UseRowShape(int offset, int b) => new XElement(
            svg + "use", 
            new XAttribute(xlink + "href", "#"+ ByteRowID(b)), 
            new XAttribute("x", (offset + 1) * spacing)
            );

        private static XElement CreatePaper(int width, int height) => new XElement(
            svg + "g", 
            new XElement(
                svg + "rect", 
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
            .Where(bit => ((data >> bit) & 1) == 1)
            .Select(bit => UseHole(bit + (bit > 2 ? 1 : 0), true))
            .Concat(new[] { UseHole(3, false) });
    }
}
