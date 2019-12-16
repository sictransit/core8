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
            var pdp = new PDP();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.ControlledBy(loggingLevel)
                .CreateLogger();

            TestBIN(pdp);
        }

        public static void TestBIN(PDP pdp)
        {
            LoadRIMLowSpeed(pdp); // Toggle RIM loader

            pdp.LoadTape(File.ReadAllBytes(@"Tapes/dnnbin.rim")); // Load BIN loader

            pdp.Start(waitForHalt: false); // Run! RIM loader won't HLT.

            while (pdp.Keyboard.IsTapeLoaded) // While there is tape to be read ...
            {
                Thread.Sleep(200);
            }

            pdp.Stop(); // HLT

            var client = new HttpClient();

            var tape = client.GetByteArrayAsync(@"https://github.com/PontusPih/TINT8/releases/download/v0.1.0-alpha/tint.bin").Result; // Pontus' TINT

            pdp.LoadTape(tape);

            pdp.Load8(7777);

            pdp.Toggle8(0000);

            pdp.Start();

            pdp.Teleprinter.Clear();
            pdp.Keyboard.Clear();

            pdp.Load8(0200);

            loggingLevel.MinimumLevel = Serilog.Events.LogEventLevel.Debug;

            pdp.Start(waitForHalt: false);

            while (true)
            {
                Thread.Sleep(1000);

                pdp.Keyboard.Load(new[] { (byte)'F' });

                Log.Information("Typed something ...");
            }        
        }


        private static void LoadRIMLowSpeed(PDP pdp)
        {
            pdp.Load8(7756);

            pdp.Deposit8(6032);
            pdp.Deposit8(6031);
            pdp.Deposit8(5357);
            pdp.Deposit8(6036);
            pdp.Deposit8(7106);
            pdp.Deposit8(7006);
            pdp.Deposit8(7510);
            pdp.Deposit8(5357);
            pdp.Deposit8(7006);
            pdp.Deposit8(6031);
            pdp.Deposit8(5367);
            pdp.Deposit8(6034);
            pdp.Deposit8(7420);
            pdp.Deposit8(3776);
            pdp.Deposit8(3376);
            pdp.Deposit8(5356);

            pdp.Load8(7756);
        }
    }
}
