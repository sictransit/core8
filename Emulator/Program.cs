using Serilog;
using Serilog.Core;
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

            var host = new Host(pdp.Keyboard, pdp.Teleprinter);

            host.Start();

            TestBIN(pdp);

            host.Stop();
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

            var tape = client.GetByteArrayAsync(@"https://www.pdp8.net/pdp8cgi/os8_html/INST2.BN?act=file;fn=images/misc_dectapes/unlabled2.tu56;blk=308,11,0;to=sv_bin").Result;

            pdp.LoadTape(tape);

            pdp.Load8(7777);

            pdp.Toggle8(0000);

            pdp.Start();

            pdp.Teleprinter.Clear();
            pdp.Keyboard.Clear();

            pdp.Load8(0200);

            loggingLevel.MinimumLevel = Serilog.Events.LogEventLevel.Debug;

            pdp.Start(waitForHalt: true);

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
