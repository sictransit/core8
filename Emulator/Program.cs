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

            var host = new Host(pdp.Keyboard, pdp.Teleprinter);

            host.Start();

            Console.WriteLine("Hook up your telnet client now ...");

            Console.WriteLine("Press the any-key when done ...");

            Console.ReadLine();

            //TestBIN(pdp);
            TestHelloWorld(pdp);

            Console.WriteLine("Press the any-key ...");

            Console.ReadLine();


            host.Stop();
        }

        private static void TestHelloWorld(PDP pdp)
        {
            loggingLevel.MinimumLevel = Serilog.Events.LogEventLevel.Debug;

            pdp.Load8(0200);

            pdp.Deposit8(7200);
            pdp.Deposit8(7100);
            pdp.Deposit8(1220);
            pdp.Deposit8(3010);
            pdp.Deposit8(7000);
            pdp.Deposit8(1410);
            pdp.Deposit8(7450);
            pdp.Deposit8(7402); //5577
            pdp.Deposit8(4212);
            pdp.Deposit8(5204);
            pdp.Deposit8(0000);
            pdp.Deposit8(6046);
            pdp.Deposit8(6041);
            pdp.Deposit8(5214);
            pdp.Deposit8(7200);
            pdp.Deposit8(5612);
            pdp.Deposit8(0220);
            pdp.Deposit8(0110);
            pdp.Deposit8(0105);
            pdp.Deposit8(0114);
            pdp.Deposit8(0114);
            pdp.Deposit8(0117);
            pdp.Deposit8(0040);
            pdp.Deposit8(0127);
            pdp.Deposit8(0117);
            pdp.Deposit8(0122);
            pdp.Deposit8(0114);
            pdp.Deposit8(0104);
            pdp.Deposit8(0041);
            pdp.Deposit8(0000);

            //pdp.Load8(0177);
            //pdp.Deposit8(7600);            

            pdp.Load8(0200);
            pdp.Start();

            Log.Information(pdp.Teleprinter.Printout);
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

            //var tape = client.GetByteArrayAsync(@"https://www.pdp8.net/pdp8cgi/os8_html/INST2.BN?act=file;fn=images/misc_dectapes/unlabled2.tu56;blk=308,11,0;to=sv_bin").Result;
            var tape = client.GetByteArrayAsync(@"https://github.com/PontusPih/TINT8/releases/download/v0.1.0-alpha/tint.bin").Result;

            pdp.LoadTape(tape);

            pdp.Load8(7777);

            pdp.Toggle8(0000);

            pdp.Start();

            pdp.Teleprinter.Clear();
            pdp.Keyboard.Clear();

            pdp.Load8(0200);

            loggingLevel.MinimumLevel = Serilog.Events.LogEventLevel.Information;

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
