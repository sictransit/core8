using CommandLine;
using Serilog;
using Serilog.Core;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace Core8
{
    public static class Program
    {
        private static LoggingLevelSwitch loggingLevel = new LoggingLevelSwitch(Serilog.Events.LogEventLevel.Information);

        private static PDP pdp;

        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("emulator.log")
                .MinimumLevel.ControlledBy(loggingLevel)
                .CreateLogger();

            Parser.Default.ParseArguments<Options>(args)
                    .WithParsed<Options>(o =>
                    {
                        pdp = new PDP();

                        if (o.TINT)
                        {
                            TINT();
                        }
                        else if (!string.IsNullOrWhiteSpace(o.Assemble))
                        {
                            if (TryAssemble(o.PALBART, o.Assemble, out var binary) && o.Run)
                            {
                                o.Load = binary;
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(o.Load))
                        {
                            Load(o.Load);

                            if (o.Run)
                            {
                                Run(o.StartingAddress, o.DumpMemory);
                            }
                        }

                        if (o.TTY)
                        {
                            Console.WriteLine(pdp.CPU.Teletype.Printout);
                        }
                    });
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

            pdp.Continue(waitForHalt: true);

            Thread.Sleep(1000);
        }

        private static void Load(string binFile)
        {
            pdp.LoadPaperTape(File.ReadAllBytes(binFile));
        }
    }
}
