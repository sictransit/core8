using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace Core8
{
    public static class Program
    {
        private static LoggingLevelSwitch loggingLevel = new LoggingLevelSwitch(Serilog.Events.LogEventLevel.Information);

        private static HttpClient httpClient = new HttpClient();

        private static IConfigurationRoot configuration;

        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");

            configuration = builder.Build();

            var pdp = new PDP();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("emulator.log")
                .MinimumLevel.ControlledBy(loggingLevel)
                .CreateLogger();

            if (args.Any(x => x == "--tint"))
            {
                PlayTINT(pdp);
            }
        }

        public static void PlayTINT(PDP pdp)
        {
            pdp.Clear();

            pdp.LoadPaperTape(httpClient.GetByteArrayAsync(@"https://github.com/PontusPih/TINT8/releases/download/v0.1.0-alpha/tint.bin").Result);

            pdp.Clear();

            pdp.Load8(0200);

            pdp.Continue(waitForHalt: true);
        }
    }
}
