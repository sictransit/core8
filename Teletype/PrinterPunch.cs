using Core8.Extensions;
using Core8.Model;
using Core8.Model.Interfaces;
using NetMQ;
using NetMQ.Sockets;
using Serilog;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core8.Peripherals.Teletype;

public class PrinterPunch : IODevice, IPrinterPunch
{
    private const int INTERRUPT_ENABLE = 1 << 0;

    private readonly ConcurrentQueue<byte> output = new();
    private readonly PublisherSocket publisherSocket;

    private int deviceControl;

    public PrinterPunch(string outputAddress, int deviceId = 4) : base(deviceId)
    {
        publisherSocket = new PublisherSocket();
        publisherSocket.Connect(outputAddress);
    }

    protected override bool InterruptEnable => (deviceControl & INTERRUPT_ENABLE) != 0;

    private byte? OutputBuffer { get; set; }

    protected override int TickDelay => 100;

    public IReadOnlyCollection<byte> Output => output.ToArray();


    public bool OutputFlag { get; private set; }

    public void ClearOutputFlag() => OutputFlag = false;

    public void SetOutputFlag() => OutputFlag = true;

    protected override bool RequestInterrupt => OutputFlag;

    public string Printout =>
        Encoding.ASCII.GetString(output.Select(x => (byte)(x & 0b_000_001_111_111)).ToArray());

    public void SetDeviceControl(int data)
    {
        deviceControl = data & INTERRUPT_ENABLE;
    }

    public void Print(byte c)
    {
        if (OutputBuffer == null)
        {
            OutputBuffer = c;

            Ticks = 0;

            Log.Debug($"Output: {c.ToPrintableAscii()}");
        }
        else
        {
            Log.Warning($"Type with char in buffer: {c.ToPrintableAscii()}");
        }
    }

    public void Clear()
    {
        SetDeviceControl(0b_000_000_000_011);

        ClearOutputFlag();

        OutputBuffer = null;

        Ticks = 0;
    }

    protected override void HandleTick()
    {
        HandleOutput();
    }

    private void HandleOutput()
    {
        if (!OutputBuffer.HasValue)
        {
            return;
        }

        output.Enqueue(OutputBuffer.Value);

        if (!publisherSocket.TrySendFrame(new[] { (byte)(OutputBuffer.Value & 0b_000_001_111_111) }))
        {
            Log.Warning("Failed to send 0MQ frame.");
        }

        OutputBuffer = null;

        SetOutputFlag();
    }
}