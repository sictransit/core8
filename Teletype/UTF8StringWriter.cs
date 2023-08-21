using System.IO;
using System.Text;

namespace Core8.Peripherals.Teletype;

internal class UTF8StringWriter : StringWriter
{
    public override Encoding Encoding => Encoding.UTF8;
}