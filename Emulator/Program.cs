using Serilog;
using Serilog.Core;
using System;
using System.Net.Http;

namespace Core8
{
    public class Program
    {
        private static LoggingLevelSwitch loggingLevel = new LoggingLevelSwitch(Serilog.Events.LogEventLevel.Information);

        private static HttpClient httpClient = new HttpClient();

        public static void Main(string[] args)
        {
            var pdp = new PDP();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("emulator.log")
                .MinimumLevel.ControlledBy(loggingLevel)
                .CreateLogger();

            Console.WriteLine("Hook up your telnet client now ...");

            Console.WriteLine("Press the any-key when done ...");

            //Console.ReadLine();

            TestBIN(pdp);

            Console.WriteLine("Press the any-key ...");

            Console.ReadLine();
        }



        public static void TestBIN(PDP pdp)
        {
            var adapter = new MQRelay(pdp.Processor.Teleprinter);

            pdp.Clear();

            //pdp.LoadPaperTape(File.ReadAllBytes(@"tapes/hello_world.bin"));
            pdp.LoadPaperTape(httpClient.GetByteArrayAsync(@"https://github.com/PontusPih/TINT8/releases/download/v0.1.0-alpha/tint.bin").Result);

            pdp.Clear();

            pdp.Load8(0200);

            pdp.Continue(waitForHalt: true);
        }



    }
}
