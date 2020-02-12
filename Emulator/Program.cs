using CommandLine;
using Serilog;
using Serilog.Core;
using System.IO;
using System.Threading;

namespace Core8
{
    public static class Program
    {
        private static LoggingLevelSwitch loggingLevel = new LoggingLevelSwitch(Serilog.Events.LogEventLevel.Information);

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
                        if (o.TINT)
                        {
                            TINT.Play();
                        }
                        else if (!string.IsNullOrWhiteSpace(o.Assemble))
                        {
                            Assemble(o.PALBART, o.Assemble, o.Run, o.StartingAddress, o.DumpMemory);
                        }
                    });


        }

        private static void Assemble(string palbart, string file, bool run, uint startingAddress, bool dumpMemory)
        {
            var assembler = new Assembler(palbart);

            var assembled = assembler.TryAssemble(file, out var binary);

            if (!assembled)
            {
                Log.Error($"Assembly failed: {file}");
            }
            else
            {
                Log.Information($"Assembled: {file} -> {binary}");

                if (run)
                {
                    var pdp = new PDP();

                    pdp.LoadPaperTape(File.ReadAllBytes(binary));

                    pdp.Load8(startingAddress);

                    if (dumpMemory)
                    {
                        pdp.DumpMemory();
                    }

                    //loggingLevel.MinimumLevel = Serilog.Events.LogEventLevel.Verbose;

                    pdp.Continue(waitForHalt: true);

                    Thread.Sleep(1000);
                }
            }
        }
    }
}
