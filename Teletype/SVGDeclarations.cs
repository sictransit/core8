using System.Xml.Linq;

namespace Core8.Peripherals.Teletype;

internal static class SVGDeclarations
{
    public static readonly XNamespace svg = "http://www.w3.org/2000/svg";
    public static readonly XNamespace xlink = "http://www.w3.org/1999/xlink";

    public const string BYTE_ROW_PREFIX = "byte";
}
