using Core8.Abstract;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Core8
{
    public class SVGReader : SVGBase
    {
        public IEnumerable<byte> Read(string tape)
        {
            var xml = XElement.Parse(tape);

            foreach (var useElement in xml.Descendants().Where(x => x.Name == svg + "use"))
            {
                var value = useElement.Attribute(xlink + "href").Value;

                if (value.StartsWith($"#{ByteRowPrefix}"))
                {
                    yield return byte.Parse(value.Remove(0, ByteRowPrefix.Length + 1));
                }
            }
        }
    }
}
