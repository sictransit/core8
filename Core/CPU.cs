using Core8.Extensions;
using Core8.Model;
using Core8.Model.Instructions;
using Core8.Model.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

    //private const int LINE_PRINTER_DEVICE = 54; // device 66: serial line printer

    // TODO: Create class, include hit count. How to break on instruction and then continue e.g. 1000 more?
    private readonly List<Func<ICPU, bool>> breakpoints = new();

    private readonly FixedDiskInstructions fixedDiskInstructions;
    private readonly FloppyDriveInstructions floppyDriveInstructions;
    private readonly Group1Instructions group1Instructions;
    private readonly Group2ANDInstructions group2AndInstructions;
    private readonly Group2ORInstructions group2OrInstructions;
    private readonly Group3Instructions group3Instructions;
    private readonly InterruptInstructions interruptInstructions;
    private readonly KeyboardReaderInstructions keyboardReaderInstructions;
    private readonly MemoryManagementInstructions memoryManagementInstructions;
    private readonly MemoryReferenceInstructions memoryReferenceInstructions;
    private readonly PrinterPunchInstructions printerPunchInstructions;
    private readonly PrivilegedNoOperationInstruction privilegedNoOperationInstruction;

    private bool debug;

    private int fixedDiskDeviceId = -1;

    private int floppyDriveDeviceId = -1;

    private int keyboardReaderDeviceId = -1;

    private int printerPunchDeviceId = -1;

    private volatile bool running;

    private bool singleStep;

    private (int address, int data) waitingLoopCap;

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
        interruptInstructions = new InterruptInstructions(this);
        privilegedNoOperationInstruction = new PrivilegedNoOperationInstruction(this);
        floppyDriveInstructions = new FloppyDriveInstructions(this);
        fixedDiskInstructions = new FixedDiskInstructions(this);

        Memory = new Memory();
        Interrupts = new Interrupts(this);
        Registry = new Registry();
    }

    public IRegistry Registry { get; }

    public IInterrupts Interrupts { get; }

    public IKeyboardReader KeyboardReader { get; private set; }

    public IPrinterPunch PrinterPunch { get; private set; }

    public IFloppyDrive FloppyDrive { get; private set; }

    public IFixedDisk FixedDisk { get; private set; }

    public IMemory Memory { get; }

    public IInstruction Instruction { get; private set; }

    public int InstructionCounter { get; private set; }

    public void Attach(IFixedDisk peripheral)
    {
        FixedDisk = peripheral;

        fixedDiskDeviceId = peripheral.DeviceId;
    }

    public void Attach(IFloppyDrive peripheral)
    {
        FloppyDrive = peripheral;

        floppyDriveDeviceId = peripheral.DeviceId;
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
        Registry.AC.Clear();
        Interrupts.ClearUser();
        Interrupts.Disable();
        FloppyDrive?.Initialize();
    }

    public void Halt() => running = false;

    public void Run()
    {
        var debugPC = 0;
        var debugIF = 0;

        running = true;

        Log.Information($"CONT @ {Registry.PC} (dbg: {debug})");

        InstructionCounter = 0;

        try
        {
            while (running)
            {
                InstructionCounter++;

                KeyboardReader.Tick();
                PrinterPunch.Tick();
                FloppyDrive?.Tick();
                FixedDisk?.Tick();

                Interrupts.Interrupt();

                if (debug)
                {
                    debugPC = Registry.PC.Address;
                    debugIF = Registry.PC.IF;
                }

                Instruction = Fetch(Registry.PC.Content);

                if (debug)
                {
                    Log.Debug($"{debugIF}{debugPC.ToOctalString()}  {Registry.AC.Link} {Registry.AC.Accumulator.ToOctalString()}  {Registry.MQ.Content.ToOctalString()}  {Instruction}");

                    if (breakpoints.Any(b => b(this)) || singleStep)
                    {
                        if (Debugger.IsAttached)
                        {
                            Debugger.Break();
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                Registry.PC.Increment();

                Instruction.Execute();
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

    public void SetBreakpoint(Func<ICPU, bool> breakpoint)
    {
        breakpoints.Add(breakpoint);

        Debug(true);
    }

    public void Debug(bool state) => debug = state;

    public void SingleStep(bool state)
    {
        singleStep = state;

        Debug(state || breakpoints.Any());
    }

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
            //      IOT when (data & IO) >> 3 == LINE_PRINTER_DEVICE => teleprinterInstructions,
            IOT when (data & IO) >> 3 == floppyDriveDeviceId => floppyDriveInstructions,
            IOT when (data & IO) >> 3 == fixedDiskDeviceId => fixedDiskInstructions,
            IOT => privilegedNoOperationInstruction,
            _ => memoryReferenceInstructions.LoadAddress(address),
        }).LoadData(data);

        if (address % 2 == 0) // To avoid looping over e.g. SDN/JMP as fast as the host CPU can manage.
        {
            if (waitingLoopCap == (address, data))
            {
                Thread.Sleep(0);
            }
            else
            {
                waitingLoopCap = (address, data);
            }
        }

        return instruction;
    }
}
