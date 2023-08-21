using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Core8.Peripherals.Teletype;

public static class SVGReader
{
    public static IEnumerable<byte> Read(string tape)
    {
        XElement xml = XElement.Parse(tape);

        foreach (XElement useElement in xml.Descendants().Where(x => x.Name == SVGDeclarations.svg + "use"))
        {
            string value = useElement.Attribute(SVGDeclarations.xlink + "href")?.Value;

            if (value != null && value.StartsWith($"#{SVGDeclarations.BYTE_ROW_PREFIX}"))
            {
                yield return byte.Parse(value.Remove(0, SVGDeclarations.BYTE_ROW_PREFIX.Length + 1));
            }
        }
    }
}
