using Core8.Model.Interfaces;
using Core8.Peripherals.Teletype;

namespace Core8.Core;

public class LinePrinter : PrinterPunch, ILinePrinter
{
    public LinePrinter(string outputAddress, int deviceId = 54) : base(outputAddress, deviceId) // device 66: serial line printer
    {
    }
}
