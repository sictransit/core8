using Serilog;
using Serilog.Core;
using System;

namespace Core8.Peripherals;

public static class Program
{
    private static readonly LoggingLevelSwitch loggingLevel = new(Serilog.Events.LogEventLevel.Debug);

    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .MinimumLevel.ControlledBy(loggingLevel)
            .CreateLogger();

        using Host host = new();

        host.Start();

        Console.WriteLine("Host has started ...");

        Console.ReadLine();

        host.Stop();
    }
}
