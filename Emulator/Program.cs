using CommandLine;
using Core8.Core;
using Core8.Extensions;
using Core8.Peripherals.Teletype;
using Core8.Utilities;
using Serilog;
using Serilog.Core;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using Serilog.Events;

namespace Core8
{
    public static class Program
    {
        private static readonly LoggingLevelSwitch LoggingLevel = new();

        private static PDP pdp;

        public static void Main(string[] args)
        {
            var logFilename = "emulator.log";

            File.Delete(logFilename);

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(Serilog.Events.LogEventLevel.Information)
                .WriteTo.File(logFilename, Serilog.Events.LogEventLevel.Debug)
                .MinimumLevel.ControlledBy(LoggingLevel)
                .CreateLogger();

            Parser.Default.ParseArguments<Options>(args)
                    .WithParsed(o =>
                    {
                        pdp = new PDP(true);

                        pdp.Clear();

                        if (o.Debug)
                        {
                            pdp.CPU.Debug(true);

                            LoggingLevel.MinimumLevel = LogEventLevel.Debug;
                        }

                        if (!string.IsNullOrWhiteSpace(o.Convert))
                        {
                            Convert(o.Convert);
                        }

                        if (o.TINT)
                        {
                            TINT();
                        }
                        else if (!string.IsNullOrWhiteSpace(o.Assemble))
                        {
                            if (TryAssemble(o.PALBART, o.Assemble, out var binary))
                            {
                                o.Load = binary;
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(o.Load))
                        {
                            Load(o.Load);

                            if (o.Run)
                            {
                                if (o.Debug) { 
                                    LoggingLevel.MinimumLevel = Serilog.Events.LogEventLevel.Debug; 
                                }

                                Run(o.StartingAddress, o.DumpMemory);
                            }
                        }

                        if (o.Floppy)
                        {
                            //FloppyTesting();
                            FloppyDevelopment();
                        }

                        if (o.TTY)
                        {
                            Console.WriteLine(pdp.CPU.Teletype.Printout);
                        }

                        if (!string.IsNullOrEmpty(o.Punch))
                        {
                            Punch(o.Punch);
                        }
                    });
        }

        private static void Punch(string binFile)
        {
            var file = new FileInfo(binFile);

            var data = File.ReadAllBytes(binFile);

            var punch = new SVGPunch();

            var tape = punch.Punch(data, file.Name);

            var svgFile = Path.ChangeExtension(file.FullName, ".svg");

            File.WriteAllText(svgFile, tape);
        }

        private static void Convert(string s)
        {
            foreach (var c in s)
            {
                Console.WriteLine($"{c}: {((int)c).ToOctalString()}");
            }
        }

        private static void FloppyTesting()
        {
            LoggingLevel.MinimumLevel = Serilog.Events.LogEventLevel.Information;

            using var httpClient = new HttpClient();

            pdp.LoadPaperTape(httpClient.GetByteArrayAsync(@"https://www.dropbox.com/s/mvm1mh47jybfl5t/dirxa-d-pb?dl=1").Result);

            //pdp.Load8(0020);
            //pdp.Deposit8(0000);
            //pdp.Deposit8(0000);
            //pdp.Deposit8(0400);

            //pdp.Load8(0020);
            //pdp.Deposit8(0000);
            //pdp.Deposit8(4000);
            //pdp.Toggle8(0000);

            pdp.Clear();

            pdp.Load8(0200);

            pdp.Toggle8(0400);

            //pdp.SetBreakpoint8(01723); // Error test?
            //pdp.SetBreakpoint8(00406); // CAF
            //pdp.SetBreakpoint8(01532); // CAF
            //pdp.SetBreakpoint8(10473); // CAF

            //pdp.SetBreakpoint8(03306); // LCD
            //pdp.SetBreakpoint8(06206); // LCD

            //pdp.DumpMemory();

            pdp.Continue();

            //loggingLevel.MinimumLevel = Serilog.Events.LogEventLevel.Debug;

            //pdp.RemoveAllBreakpoints();

            //pdp.Continue(waitForHalt: true);
        }

        private static void FloppyDevelopment()
        {
            //pdp.Load8(00032);
            //pdp.Deposit8(7305);
            //pdp.Deposit8(6755);
            //pdp.Deposit8(5054);
            //pdp.Deposit8(1061);
            //pdp.Deposit8(6751);
            //pdp.Deposit8(5047);
            //pdp.Load8(00047);
            //pdp.Deposit8(4053);
            //pdp.Deposit8(3002);
            //pdp.Deposit8(2050);
            //pdp.Deposit8(5047);
            //pdp.Deposit8(0000);
            //pdp.Deposit8(6753);
            //pdp.Deposit8(5033);
            //pdp.Deposit8(6752);
            //pdp.Deposit8(5453);
            //pdp.Deposit8(7004);
            //pdp.Deposit8(0000);
            //pdp.Load8(00032);

            //pdp.Load8(0024);
            //pdp.Deposit8(7126);
            //pdp.Deposit8(1060);
            //pdp.Deposit8(6751);
            //pdp.Deposit8(7201);
            //pdp.Deposit8(4053);
            //pdp.Deposit8(4053);
            //pdp.Deposit8(7104);
            //pdp.Deposit8(6755);
            //pdp.Deposit8(5054);
            //pdp.Deposit8(6754);
            //pdp.Deposit8(7450);
            //pdp.Deposit8(7610);
            //pdp.Deposit8(5046);
            //pdp.Deposit8(1060);
            //pdp.Deposit8(7041);
            //pdp.Deposit8(1061);
            //pdp.Deposit8(3060);
            //pdp.Deposit8(5024);
            //pdp.Deposit8(6751);
            //pdp.Deposit8(4053);
            //pdp.Deposit8(3002);
            //pdp.Deposit8(2050);
            //pdp.Deposit8(5047);
            //pdp.Deposit8(0000);
            //pdp.Deposit8(6753);
            //pdp.Deposit8(5033);
            //pdp.Deposit8(6752);
            //pdp.Deposit8(5453);
            //pdp.Deposit8(7024);
            //pdp.Deposit8(6030);
            //pdp.Load8(0033);

            pdp.Load8(0022);
            pdp.Deposit8(06755);
            pdp.Deposit8(05022);
            pdp.Deposit8(07126);
            pdp.Deposit8(01060);
            pdp.Deposit8(06751);
            pdp.Deposit8(07201);
            pdp.Deposit8(04053);
            pdp.Deposit8(04053);
            pdp.Deposit8(07104);
            pdp.Deposit8(06755);
            pdp.Deposit8(05054);
            pdp.Deposit8(06754);
            pdp.Deposit8(07450);
            pdp.Deposit8(07610);
            pdp.Deposit8(05046);
            pdp.Deposit8(07402);
            pdp.Deposit8(07402);
            pdp.Deposit8(07402);
            pdp.Deposit8(07402);
            pdp.Deposit8(07402);
            pdp.Deposit8(06751);
            pdp.Deposit8(04053);
            pdp.Deposit8(03002);
            pdp.Deposit8(02050);
            pdp.Deposit8(05047);
            pdp.Deposit8(00000);
            pdp.Deposit8(06753);
            pdp.Deposit8(05033);
            pdp.Deposit8(06752);
            pdp.Deposit8(05453);
            pdp.Deposit8(07004);
            pdp.Deposit8(06030);

            pdp.Load8(0022);

            pdp.LoadFloppy(0, File.ReadAllBytes(@"C:\tmp\OS-8\os8.rx01"));

            pdp.Clear();

            //pdp.DumpMemory();

            //loggingLevel.MinimumLevel = Serilog.Events.LogEventLevel.Debug;

            pdp.Continue(false);

            while (pdp.Running)
            {
                Thread.Sleep(200);
            }

            pdp.DumpMemory();
        }

        private static void TINT()
        {
            pdp.Clear();

            using var httpClient = new HttpClient();

            pdp.LoadPaperTape(httpClient.GetByteArrayAsync(@"https://github.com/PontusPih/TINT8/releases/download/v0.1.0-alpha/tint.bin").Result);

            pdp.Clear();

            Run(200, false);
        }

        private static bool TryAssemble(string palbart, string file, out string binary)
        {
            var assembler = new Assembler(palbart);

            var assembled = assembler.TryAssemble(file, out binary);

            if (!assembled)
            {
                Log.Error($"Assembly failed: {file}");
            }
            else
            {
                Log.Information($"Assembled: {file} -> {binary}");
            }

            return assembled;
        }

        private static void Run(int startingAddress, bool dumpMemory)
        {
            pdp.Load8(startingAddress);

            if (dumpMemory)
            {
                pdp.DumpMemory();
            }

            //pdp.CPU.Debug(true);

            pdp.Continue();

            Thread.Sleep(1000);

            if (dumpMemory)
            {
                pdp.DumpMemory();
            }
        }

        private static void Load(string binFile)
        {
            pdp.LoadPaperTape(File.ReadAllBytes(binFile));

            if (pdp.CPU.Registry.AC.Accumulator != 0)
            {
                Log.Warning("BIN format checksum error.");
                Log.Information(pdp.CPU.Registry.AC.Accumulator.ToString());
            }
        }
    }
}
