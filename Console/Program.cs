using Serilog;
using System.IO;
using System.Threading;

namespace Core8.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var pdp = new PDP();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Debug()
                .CreateLogger();

            TestBIN(pdp);
        }

        public static void TestBIN(PDP pdp)
        {
            LoadRIMLowSpeed(pdp); // Toggle RIM loader

            //var bin = File.ReadAllBytes(@"Tapes/dec-08-lbaa-pm_5-10-67.bin"); // Load a paper tape image of 1967 from disk.
            var bin = File.ReadAllBytes(@"Tapes/dnnbin.rim");

            pdp.LoadTape(bin); // Load tape

            pdp.Start(waitForHalt: false); // Run! RIM loader won't HLT.

            while (pdp.Keyboard.IsTapeLoaded) // While there is tape to be read ...
            {
                Thread.Sleep(0);
            }

            pdp.Stop(); // HLT

            pdp.LoadTape(File.ReadAllBytes(@"Tapes/MAINDEC-8E-D0AB-PB.bin"));

            pdp.Load8(7777);

            pdp.Start();

            pdp.Load8(0200);

            pdp.Toggle8(7777);

            pdp.Registers.LINK_AC.Clear();

            pdp.Start();

            Log.Information(pdp.Registers.LINK_AC.Accumulator.ToString());

            pdp.Start();
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
