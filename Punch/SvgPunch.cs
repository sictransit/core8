using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;

namespace Core8
{
    public static class SvgPunch
    {
        private static readonly XNamespace svg = "http://www.w3.org/2000/svg";
        private static readonly XNamespace xlink = "http://www.w3.org/1999/xlink";

        private const int spacing = 100;
        private const int dataWidth = 72;
        private const int feederWidth = 46;        
        
        private static readonly Func<bool, XElement> HoleFunc = isData => 
        {
            var type = isData ? "data" : "feeder";

            return new XElement(
                svg + "circle", 
                new XAttribute("fill", "red"), 
                new XAttribute("stroke-width", "0"), 
                new XAttribute("stroke", "#fff"), 
                new XAttribute("r", isData ? dataWidth / 2 : feederWidth / 2), 
                new XAttribute("id",$"{type}"));
        };

        public static string Punch(byte[] data)
        {
            var width = (data.Length + 1) * spacing;
            var height = spacing * 10;            

            var defs = new XElement(svg + "defs", HoleFunc(true), HoleFunc(false), data.Distinct().OrderBy(x=>x).Select(x => CreateRowShape(x)));

            var rows = data.Select((x, i) => UseRowShape(i, x));

            var holes = new XElement(svg + "g", rows);            

            var paper = CreatePaper(width, height);

            var tape = new XElement(svg + "svg", new XAttribute("width", width), new XAttribute("height", height), new XAttribute(XNamespace.Xmlns + "xlink", xlink), defs, paper, holes);

            return tape.ToString();
        }

        private static XElement UseRowShape(int offset, int b)
        {
            var use = new XElement(svg + "use", new XAttribute(xlink + "href", $"#b{b}"), new XAttribute("x", (offset + 1) * spacing));

            return use;
        }

        private static XElement CreateRowShape(int b)
        {
            var shape = new XElement(svg + "g", CreateRow(b));
            
            shape.SetAttributeValue("id", $"b{b}");
            
            return shape;
        }

        private static XElement CreatePaper(int width, int height)
        {
            var paper = new XElement(svg + "rect");
            paper.SetAttributeValue("x", 0);
            paper.SetAttributeValue("y", 0);
            paper.SetAttributeValue("width", width);
            paper.SetAttributeValue("height", height);
            paper.SetAttributeValue("fill", "#ffffaa");

            return new XElement(svg + "g", paper);
        }

        private static IEnumerable<XElement> CreateRow(int data)
        {
            var bitOffset = 0;

            for (int bit = 0; bit < 8; bit++)
            {
                if (((data >> bit) & 1) == 1)
                {
                    yield return UseHole(bit + bitOffset, true);
                }

                if (bit == 2)
                {
                    yield return UseHole(3, false);

                    bitOffset = 1;
                }
            }            
        }
        private static XElement UseHole(int bit, bool dataHole)
        {
            var holeType = dataHole ? "data" : "feeder";

            return new XElement(svg + "use", new XAttribute(xlink + "href", $"#{holeType}"), new XAttribute("y", (bit + 1) * spacing));
        }
    }
}
