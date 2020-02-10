using Serilog;
using Serilog.Core;
using System;

namespace Core8
{
    public static class Program
    {
        private static LoggingLevelSwitch loggingLevel = new LoggingLevelSwitch(Serilog.Events.LogEventLevel.Debug);

        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.ControlledBy(loggingLevel)
                .CreateLogger();

            var host = new Host();

            host.Start();

            Console.WriteLine("Host has started ...");

            Console.ReadLine();

            host.Stop();

        }

    }
}
