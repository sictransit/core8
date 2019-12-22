using Serilog;
using Serilog.Core;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace Core8
{
    public class Program
    {
        private static LoggingLevelSwitch loggingLevel = new LoggingLevelSwitch(Serilog.Events.LogEventLevel.Information);

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
