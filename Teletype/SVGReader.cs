using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Core8.Peripherals.Teletype;

public static class SVGReader
{
    public static IEnumerable<byte> Read(string tape)
    {
        var xml = XElement.Parse(tape);

        foreach (var useElement in xml.Descendants().Where(x => x.Name == SVGDeclarations.svg + "use"))
        {
            var value = useElement.Attribute(SVGDeclarations.xlink + "href")?.Value;

            if (value != null && value.StartsWith($"#{SVGDeclarations.BYTE_ROW_PREFIX}"))
            {
                yield return byte.Parse(value.Remove(0, SVGDeclarations.BYTE_ROW_PREFIX.Length + 1));
            }
        }
    }
}
