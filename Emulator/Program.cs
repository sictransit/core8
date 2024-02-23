using CommandLine;
using Core8.Core;
using Core8.Extensions;
using Core8.Model.Interfaces;
using Core8.Peripherals.Teletype;
using Core8.Utilities;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace Core8;

public static class Program
{
    private static readonly LoggingLevelSwitch LoggingLevel = new();

    private static PDP pdp;

    public static void Main(string[] args)
    {
        var logFilename = "emulator.log";

        File.Delete(logFilename);

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(LogEventLevel.Information, "{Message:lj}{NewLine}")
            .WriteTo.File(logFilename, LogEventLevel.Debug, "{Message:lj}{NewLine}")
            .MinimumLevel.ControlledBy(LoggingLevel)
            .CreateLogger();

        Parser.Default.ParseArguments<Options>(args)
                .WithParsed(o =>
                {
                    pdp = new PDP(true, true);

                    pdp.Clear();

                    if (o.Debug)
                    {
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
                            if (o.Debug)
                            {
                                LoggingLevel.MinimumLevel = LogEventLevel.Debug;
                            }

                            Run(o.StartingAddress, o.DumpMemory);
                        }
                    }

                    if (o.OS8)
                    {
                        BootOS8();
                    }

                    if (o.Advent)
                    {
                        Advent();
                    }

                    if (o.Kermit)
                    {
                        Kermit();
                    }

                    if (o.TTY)
                    {
                        Console.WriteLine(pdp.CPU.PrinterPunch.Printout);
                    }

                    if (!string.IsNullOrEmpty(o.Punch))
                    {
                        Punch(o.Punch);
                    }
                });
    }

    private static void Punch(string binFile)
    {
        FileInfo file = new(binFile);

        var data = File.ReadAllBytes(binFile);

        SVGPunch punch = new();

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

    private static void Kermit()
    {
        pdp.CPU.Memory.Clear();
        pdp.CPU.RK8E.Load(0, File.ReadAllBytes("disks/diag-games-kermit.rk05"));
        pdp.ToggleRK8EBootstrap();
        pdp.Load8(0023);

        pdp.Continue(true);

    }

    private static void Advent()
    {
        pdp.CPU.Memory.Clear();

        pdp.CPU.RK8E.Load(0, File.ReadAllBytes("disks/advent.rk05"));

        pdp.ToggleRK8EBootstrap();

        pdp.Load8(0023);

        pdp.Continue(false);

        List<(Func<IPrinterPunch, bool>, byte[])> steps = new()
        {
            (tty=>tty.Printout.Contains('.'), Encoding.ASCII.GetBytes("R FRTS\r")),
            (tty=>tty.Printout.Contains('*'), Encoding.ASCII.GetBytes("ADVENT\r")),
            (tty=>tty.Printout.Count(c=> c == '*') == 2, new byte[]{0x1b}),
        };

        while (pdp.Running)
        {
            foreach (var step in steps.ToArray())
            {
                if (step.Item1(pdp.CPU.PrinterPunch))
                {
                    pdp.CPU.KeyboardReader.Type(step.Item2);

                    steps.Remove(step);
                    break;
                }
            }

            Thread.Sleep(200);
        }
    }

    private static void BootOS8()
    {
        pdp.CPU.Memory.Clear();

        pdp.ToggleRX8EBootstrap();

        pdp.LoadRX01(0, File.ReadAllBytes(@"disks\os8_rx.rx01"));

        pdp.Clear();

        Run(0022, false);
    }

    private static void TINT()
    {
        pdp.Clear();

        using HttpClient httpClient = new();

        pdp.LoadPaperTape(httpClient.GetByteArrayAsync(@"https://github.com/PontusPih/TINT8/releases/download/v0.1.0-alpha/tint.bin").Result);

        pdp.Clear();

        Run(200, false);
    }

    private static bool TryAssemble(string palbart, string file, out string binary)
    {
        Assembler assembler = new(palbart);

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
