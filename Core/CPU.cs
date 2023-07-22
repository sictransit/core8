using Core8.Extensions;
using Core8.Model;
using Core8.Model.Instructions;
using Core8.Model.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Core8.Core
{
    public class CPU : ICPU
    {
        private readonly Group1Instructions group1Instructions;
        private readonly Group2ANDInstructions group2AndInstructions;
        private readonly Group2ORInstructions group2OrInstructions;
        private readonly Group3Instructions group3Instructions;
        private readonly MemoryReferenceInstructions memoryReferenceInstructions;
        private readonly MemoryManagementInstructions memoryManagementInstructions;
        private readonly KeyboardInstructions keyboardInstructions;
        private readonly TeleprinterInstructions teleprinterInstructions;
        private readonly InterruptInstructions interruptInstructions;
        private readonly PrivilegedNoOperationInstruction privilegedNoOperationInstruction;
        private readonly FloppyDriveInstructions floppyDriveInstructions;

        private const int IOT = 0b_110_000_000_000;
        private const int MCI = 0b_111_000_000_000;
        private const int IO = 0b_000_111_111_000;
        private const int GROUP = 0b_000_100_000_000;
        private const int GROUP_3 = 0b_111_100_000_001;
        private const int GROUP_2_AND = 0b_111_100_001_000;
        private const int FLOPPY = 0b_000_111_000_000;
        private const int MEMORY_MANAGEMENT = 0b_110_010_000_000;
        private const int INTERRUPT_MASK = 0b_000_111_111_000;

        private volatile bool running;

        private bool singleStep;

        private bool debug;

        private readonly List<Func<ICPU, bool>> breakpoints = new();

        public CPU(ITeletype teletype, IFloppyDrive floppy)
        {
            group1Instructions = new Group1Instructions(this);
            group2AndInstructions = new Group2ANDInstructions(this);
            group2OrInstructions = new Group2ORInstructions(this);
            group3Instructions = new Group3Instructions(this);
            memoryReferenceInstructions = new MemoryReferenceInstructions(this);
            memoryManagementInstructions = new MemoryManagementInstructions(this);
            keyboardInstructions = new KeyboardInstructions(this);
            teleprinterInstructions = new TeleprinterInstructions(this);
            interruptInstructions = new InterruptInstructions(this);
            privilegedNoOperationInstruction = new PrivilegedNoOperationInstruction(this);
            floppyDriveInstructions = new FloppyDriveInstructions(this);

            Teletype = teletype ?? throw new ArgumentNullException(nameof(teletype));

            Memory = new Memory();

            FloppyDrive = floppy;

            Interrupts = new Interrupts(this);

            Registry = new Registry();
        }

        public IRegistry Registry { get; }

        public IInterrupts Interrupts { get; }

        public ITeletype Teletype { get; }

        public IFloppyDrive FloppyDrive { get; }

        public IMemory Memory { get; }

        public int InstructionCounter { get; private set; }

        public void Clear()
        {
            Teletype.Clear();
            Registry.AC.Clear();
            Interrupts.ClearUser();
            Interrupts.Disable();
            FloppyDrive?.Initialize();
        }

        public void Halt()
        {
            running = false;
        }

        public void Run()
        {
            running = true;

            Log.Information($"CONT @ {Registry.PC} (dbg: {debug})");

            string interrupts = null;
            string floppy = null;
            string lac = null;

            InstructionCounter = 0;

            try
            {
                while (running)
                {
                    InstructionCounter++;

                    if (debug)
                    {
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

                    Teletype.Tick();
                    FloppyDrive?.Tick();

                    Interrupts.Interrupt();

                    var instruction = Fetch(Registry.PC.Content);

                    if (debug)
                    {
                        Log.Debug($"{Registry.PC.IF} {Registry.PC.Address.ToOctalString(4)}  {Registry.AC.Link} {Registry.AC.Accumulator.ToOctalString()}  {instruction}");

                        var f = FloppyDrive?.ToString();
                        if (f != floppy)
                        {
                            floppy = f;
                            Log.Information(floppy);
                        }

                        var i = Interrupts.ToString();
                        if (i != interrupts)
                        {
                            interrupts = i;
                            Log.Information(interrupts);
                        }
                    }

                    Registry.PC.Increment();

                    instruction.Execute();
                }
            }
            catch (Exception ex)
            {
                Log.Fatal($"Caught Exception in CPU: {ex}");

                throw;
            }
            finally
            {
                running = false;

                Log.Information($"HLT @ {Registry.PC}");

                Log.Debug(Registry.ToString());
            }
        }

        public IInstruction Debug8(int address)
        {
            return Debug10(address.ToDecimal());
        }

        public IInstruction Debug10(int address)
        {
            return Fetch(address);
        }

        private IInstruction Fetch(int address)
        {
            var data = Memory.Read(address);

            return Decode(data).Load(address, data);
        }

        public void SetBreakpoint(Func<ICPU, bool> breakpoint)
        {
            breakpoints.Add(breakpoint);

            Debug(true);
        }

        public void Debug(bool state)
        {
            debug = state;
        }

        public void SingleStep(bool state)
        {
            singleStep = state;
            debug |= state;
        }

        public IInstruction Decode(int data) =>
    (data & 0b_111_000_000_000) switch
    {
        MCI when (data & GROUP) == 0 => group1Instructions,
        MCI when (data & GROUP_3) == GROUP_3 => group3Instructions,
        MCI when (data & GROUP_2_AND) == GROUP_2_AND => group2AndInstructions,
        MCI => group2OrInstructions,
        IOT when (data & FLOPPY) == FLOPPY => floppyDriveInstructions,
        IOT when (data & MEMORY_MANAGEMENT) == MEMORY_MANAGEMENT => memoryManagementInstructions,
        IOT when (data & INTERRUPT_MASK) == 0 => interruptInstructions,
        IOT when (data & IO) >> 3 == 3 => keyboardInstructions,
        IOT when (data & IO) >> 3 == 4 => teleprinterInstructions,
        IOT => privilegedNoOperationInstruction,
        _ => memoryReferenceInstructions,
    };
    }
}
