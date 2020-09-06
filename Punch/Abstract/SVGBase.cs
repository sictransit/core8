using System.Xml.Linq;

namespace Core8.Abstract
{
    public abstract class SVGBase
    {
        protected static readonly XNamespace svg = "http://www.w3.org/2000/svg";
        protected static readonly XNamespace xlink = "http://www.w3.org/1999/xlink";

        protected static readonly string ByteRowPrefix = "byte";
    }
}
