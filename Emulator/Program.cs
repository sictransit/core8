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

        private static HttpClient httpClient = new HttpClient();

        public static void Main(string[] args)
        {
            var pdp = new PDP();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.ControlledBy(loggingLevel)
                .CreateLogger();

            Console.WriteLine("Hook up your telnet client now ...");

            Console.WriteLine("Press the any-key when done ...");

            Console.ReadLine();

            TestBIN(pdp);
            //TestHelloWorld(pdp);

            Console.WriteLine("Press the any-key ...");

            Console.ReadLine();
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
            ToggleRIMLowSpeedLoader(pdp); // Toggle RIM loader

            pdp.LoadTape(File.ReadAllBytes("tapes/binLoader.rim")); // Load BIN loader


            pdp.Load8(7756);

            pdp.Start(waitForHalt: false); // Run! RIM loader won't HLT.


            while (pdp.Keyboard.IsTapeLoaded) // While there is tape to be read ...
            {
                Thread.Sleep(200);
            }

            pdp.Stop(); // HLT

            pdp.Clear();

            pdp.LoadTape(httpClient.GetByteArrayAsync("https://www.bernhard-baehr.de/pdp8e/MAINDECs/MAINDEC-8E-D0AB-PB").Result); // Load BIN loader

            pdp.Load8(7777);

            //loggingLevel.MinimumLevel = Serilog.Events.LogEventLevel.Debug;
            pdp.Start();

            Log.Information(pdp.Memory.ToString());
            Log.Information(pdp.Registers.LINK_AC.ToString());

            pdp.Load8(0200);
            pdp.Toggle8(7777);
            pdp.Clear();

            loggingLevel.MinimumLevel = Serilog.Events.LogEventLevel.Debug;
            pdp.DumpMemory();

            pdp.Start();

            Log.Information(pdp.Memory.ToString());
            Log.Information(pdp.Registers.LINK_AC.ToString());

            pdp.Start();
            //pdp.DumpMemory();
        }


        private static void ToggleRIMLowSpeedLoader(PDP pdp)
        {
            pdp.Load8(7756);

            //pdp.Deposit8(6014);
            //pdp.Deposit8(6011);
            //pdp.Deposit8(5357);
            //pdp.Deposit8(6016);
            //pdp.Deposit8(7106);
            //pdp.Deposit8(7006);
            //pdp.Deposit8(7510);
            //pdp.Deposit8(5357);
            //pdp.Deposit8(7006);
            //pdp.Deposit8(6011);
            //pdp.Deposit8(5367);
            //pdp.Deposit8(6016);
            //pdp.Deposit8(7420);
            //pdp.Deposit8(3776);
            //pdp.Deposit8(3376);
            //pdp.Deposit8(5357);
            //pdp.Deposit8(0);
            //pdp.Deposit8(0);

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
            pdp.Deposit8(0);
            pdp.Deposit8(0);


        }
    }
}
