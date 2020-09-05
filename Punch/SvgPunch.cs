using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Core8
{
    public static class SvgPunch
    {
        private static readonly XNamespace svgNamespace = "http://www.w3.org/2000/svg";        

        private const int spacing = 100;
        private const int dataWidth = 72;
        private const int feederWidth = 46;

        public static string Punch(byte[] data)
        {
            var width = (data.Length + 1) * spacing;
            var height = spacing * 10;

            var rows = data.Select((x, i) => CreateRow(i, x));

            var holes = new XElement(svgNamespace + "g", rows);
            holes.SetAttributeValue("fill", "#000");
            holes.SetAttributeValue("stroke-width", "0");
            holes.SetAttributeValue("stroke", "#fff");

            var paper = CreatePaper(width, height);

            var svg = new XElement(svgNamespace + "svg", new[] { paper, holes });

            svg.SetAttributeValue("width", width);
            svg.SetAttributeValue("height", height);

            return svg.ToString();
        }

        private static XElement CreatePaper(int width, int height)
        {
            var paper = new XElement(svgNamespace + "rect");
            paper.SetAttributeValue("x", 0);
            paper.SetAttributeValue("y", 0);
            paper.SetAttributeValue("width", width);
            paper.SetAttributeValue("height", height);
            paper.SetAttributeValue("fill", "#ffffaa");

            return new XElement(svgNamespace + "g", paper);
        }

        private static XElement CreateRow(int offset, int data)
        {
            var holes = new List<XElement>();

            var bitOffset = 0;

            for (int bit = 0; bit < 8; bit++)
            {
                if (((data >> bit) & 1) == 1)
                {
                    holes.Add(CreateHole(offset + 1, bit + bitOffset, true));
                }

                if (bit == 2)
                {
                    holes.Add(CreateHole(offset + 1, 3, false));

                    bitOffset = 1;
                }
            }

            return new XElement(svgNamespace + "g", holes);
        }

        private static XElement CreateHole(int offset, int bit, bool dataHole)
        {
            var hole = new XElement(svgNamespace +"ellipse");            

            hole.SetAttributeValue("rx", dataHole ? dataWidth / 2 : feederWidth / 2);
            hole.SetAttributeValue("ry", dataHole ? dataWidth / 2 : feederWidth / 2);
            hole.SetAttributeValue("cx", offset * spacing);
            hole.SetAttributeValue("cy", (bit + 1) * spacing);

            return hole;
        }
    }
}
