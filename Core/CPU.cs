using Core8.Extensions;
using Core8.Model;
using Core8.Model.Instructions;
using Core8.Model.Interfaces;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Core8.Core;

public class CPU : ICPU
{
    private const int INSTRUCTION_TYPE_MASK = 0b_111_000_000_000;

    private const int IOT = 0b_110_000_000_000;
    private const int MCI = 0b_111_000_000_000;
    private const int IO = 0b_000_111_111_000;
    private const int GROUP = 0b_000_100_000_000;
    private const int GROUP_3 = 0b_111_100_000_001;
    private const int GROUP_2_AND = 0b_111_100_001_000;
    private const int MEMORY_MANAGEMENT = 0b_110_010_000_000;
    private const int MEMORY_MANAGEMENT_MASK = 0b_111_111_000_000;
    private const int INTERRUPT_MASK = 0b_000_111_111_000;

    private readonly List<Breakpoint> breakpoints = new();

    private readonly RK8EInstructions rk8eInstructions;
    private readonly RX8EInstructions rx8eInstructions;
    private readonly Group1Instructions group1Instructions;
    private readonly Group2ANDInstructions group2AndInstructions;
    private readonly Group2ORInstructions group2OrInstructions;
    private readonly Group3Instructions group3Instructions;
    private readonly InterruptInstructions interruptInstructions;
    private readonly KeyboardReaderInstructions keyboardReaderInstructions;
    private readonly MemoryManagementInstructions memoryManagementInstructions;
    private readonly MemoryReferenceInstructions memoryReferenceInstructions;
    private readonly PrinterPunchInstructions printerPunchInstructions;
    private readonly LinePrinterInstructions linePrinterInstructions;
    private readonly PrivilegedNoOperationInstruction privilegedNoOperationInstruction;

    private int rk8eDeviceId = -1;

    private int rx8eDeviceId = -1;

    private int keyboardReaderDeviceId = -1;

    private int printerPunchDeviceId = -1;

    private int linePrinterDeviceId = -1;

    private volatile bool running;

    public CPU()
    {
        group1Instructions = new Group1Instructions(this);
        group2AndInstructions = new Group2ANDInstructions(this);
        group2OrInstructions = new Group2ORInstructions(this);
        group3Instructions = new Group3Instructions(this);
        memoryReferenceInstructions = new MemoryReferenceInstructions(this);
        memoryManagementInstructions = new MemoryManagementInstructions(this);
        keyboardReaderInstructions = new KeyboardReaderInstructions(this);
        printerPunchInstructions = new PrinterPunchInstructions(this);
        linePrinterInstructions = new LinePrinterInstructions(this);
        interruptInstructions = new InterruptInstructions(this);
        privilegedNoOperationInstruction = new PrivilegedNoOperationInstruction(this);
        rx8eInstructions = new RX8EInstructions(this);
        rk8eInstructions = new RK8EInstructions(this);

        Memory = new Memory();
        Interrupts = new Interrupts(this);
        Registry = new Registry();
    }

    public IRegistry Registry { get; }

    public IInterrupts Interrupts { get; }

    public IKeyboardReader KeyboardReader { get; private set; }

    public IPrinterPunch PrinterPunch { get; private set; }

    public ILinePrinter LinePrinter { get; private set; }

    public IRX8E RX8E { get; private set; }

    public IRK8E RK8E { get; private set; }

    public IMemory Memory { get; }

    public IInstruction Instruction { get; private set; }

    public int InstructionCounter { get; private set; }

    public void Attach(IRK8E peripheral)
    {
        RK8E = peripheral;

        rk8eDeviceId = peripheral.DeviceId;
    }

    public void Attach(IRX8E peripheral)
    {
        RX8E = peripheral;

        rx8eDeviceId = peripheral.DeviceId;
    }

    public void Attach(ILinePrinter peripheral)
    {
        LinePrinter = peripheral;

        linePrinterDeviceId = peripheral.DeviceId;
    }

    public void Attach(IPrinterPunch peripheral)
    {
        PrinterPunch = peripheral;

        printerPunchDeviceId = peripheral.DeviceId;
    }

    public void Attach(IKeyboardReader peripheral)
    {
        KeyboardReader = peripheral;

        keyboardReaderDeviceId = peripheral.DeviceId;
    }

    public void Clear()
    {
        KeyboardReader.Clear();
        PrinterPunch.Clear();
        LinePrinter.Clear();
        Registry.AC.Clear();
        Interrupts.ClearUser();
        Interrupts.Disable();
        RX8E?.Initialize();
    }

    public void Halt() => running = false;

    public void Run()
    {
        running = true;

        Log.Information($"CONT @ {Registry.PC}");

        try
        {
            while (running)
            {
                InstructionCounter++;

                KeyboardReader.Tick();
                PrinterPunch.Tick();
                LinePrinter.Tick();
                RX8E?.Tick();
                RK8E?.Tick();

                Interrupts.Interrupt();

                Instruction = Fetch(Registry.PC.Content);

                if (Log.IsEnabled(LogEventLevel.Debug))
                {
                    Log.Debug($"{Registry.PC.IF}{Registry.PC.Address.ToOctalString()}  {Registry.AC.Link} {Registry.AC.Accumulator.ToOctalString()}  {Registry.MQ.Content.ToOctalString()}  {Instruction}");
                }

                Registry.PC.Increment();

                Instruction.Execute();

                if (breakpoints.Count > 0 && breakpoints.Exists(b => b.Check(this)))
                {
                    if (Debugger.IsAttached)
                    {
                        Debugger.Break();
                    }

                    break;
                }
            }
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, $"Caught Exception when executing instruction: {Instruction}");

            throw;
        }
        finally
        {
            running = false;

            Log.Information($"HLT @ {Registry.PC}");

            Log.Debug(Registry.ToString());
        }
    }

    public void SetBreakpoint(Breakpoint breakpoint) => breakpoints.Add(breakpoint);

    public IInstruction Fetch(int address)
    {
        var data = Memory.Read(address);

        var instruction = ((data & INSTRUCTION_TYPE_MASK) switch
        {
            MCI when (data & GROUP) == 0 => group1Instructions.LoadAddress(address),
            MCI when (data & GROUP_3) == GROUP_3 => group3Instructions,
            MCI when (data & GROUP_2_AND) == GROUP_2_AND => group2AndInstructions,
            MCI => group2OrInstructions,
            IOT when (data & MEMORY_MANAGEMENT_MASK) == MEMORY_MANAGEMENT => memoryManagementInstructions,
            IOT when (data & INTERRUPT_MASK) == 0 => interruptInstructions,
            IOT when (data & IO) >> 3 == keyboardReaderDeviceId => keyboardReaderInstructions,
            IOT when (data & IO) >> 3 == printerPunchDeviceId => printerPunchInstructions,
            IOT when (data & IO) >> 3 == linePrinterDeviceId => linePrinterInstructions,
            IOT when (data & IO) >> 3 == rx8eDeviceId => rx8eInstructions,
            IOT when (data & IO) >> 3 == rk8eDeviceId => rk8eInstructions,
            IOT => privilegedNoOperationInstruction,
            _ => memoryReferenceInstructions.LoadAddress(address),
        }).LoadData(data);

        return instruction;
    }
}
