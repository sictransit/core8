using Core8.Extensions;
using Core8.Model;
using Core8.Model.Instructions;
using Core8.Model.Interfaces;
using Core8.Model.Registers;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Core8.Core
{
    public class CPU : ICPU
    {
        private volatile bool running;

        private bool singleStep;

        private bool debug;

        private readonly HashSet<int> breakpoints = new HashSet<int>();

        private readonly Group1Instructions group1Instructions;
        private readonly Group2ANDInstructions group2AndInstructions;
        private readonly Group2ORInstructions group2OrInstructions;
        private readonly Group3Instructions group3Instructions;
        private readonly MemoryReferenceInstructions memoryReferenceInstructions;
        private readonly MemoryManagementInstructions memoryManagementInstructions;
        private readonly KeyboardInstructions keyboardInstructions;
        private readonly TeleprinterInstructions teleprinterInstructions;
        private readonly InterruptInstructions interruptInstructions;
        private readonly NoOperationInstruction noOperationInstruction;
        private readonly PrivilegedNoOperationInstruction privilegedNoOperationInstruction;
        private readonly FloppyDriveInstructions floppyDriveInstructions;

        public CPU(ITeletype teletype, IFloppyDrive floppy)
        {
            Teletype = teletype ?? throw new ArgumentNullException(nameof(teletype));

            AC = new LinkAccumulator();
            PC = new InstructionFieldProgramCounter();
            SR = new SwitchRegister();
            MQ = new MultiplierQuotient();
            DF = new DataField();
            IB = new InstructionBuffer();
            UB = new UserBuffer();
            UF = new UserFlag();
            SF = new SaveField();

            Memory = new Memory();

            FloppyDrive = floppy;

            Interrupts = new Interrupts(this);

            group1Instructions = new Group1Instructions(this);
            group2AndInstructions = new Group2ANDInstructions(this);
            group2OrInstructions = new Group2ORInstructions(this);
            group3Instructions = new Group3Instructions(this);
            memoryReferenceInstructions = new MemoryReferenceInstructions(this);
            memoryManagementInstructions = new MemoryManagementInstructions(this);
            keyboardInstructions = new KeyboardInstructions(this);
            teleprinterInstructions = new TeleprinterInstructions(this);
            interruptInstructions = new InterruptInstructions(this);
            noOperationInstruction = new NoOperationInstruction(this);
            privilegedNoOperationInstruction = new PrivilegedNoOperationInstruction(this);
            floppyDriveInstructions = new FloppyDriveInstructions(this);
        }

        public LinkAccumulator AC { get; }
        public InstructionFieldProgramCounter PC { get; }
        public SwitchRegister SR { get; }
        public MultiplierQuotient MQ { get; }
        public DataField DF { get; }
        public InstructionBuffer IB { get; }
        public UserBuffer UB { get; }
        public UserFlag UF { get; }
        public SaveField SF { get; }

        public IInterrupts Interrupts { get; }

        public ITeletype Teletype { get; }

        public IFloppyDrive FloppyDrive { get; }

        public IMemory Memory { get; }

        public void Clear()
        {
            Teletype.Clear();
            AC.Clear();
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

            Log.Information($"CONT @ {PC} (dbg: {debug})");

            string interrupts = null;
            string floppy = null;

            try
            {
                while (running)
                {
                    if (debug)
                    {
                        if (breakpoints.Contains(PC.Content))
                        {
                            Log.Information("Breakpoint hit!");

                            if (Debugger.IsAttached)
                            {
                                Debugger.Break();
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (singleStep)
                        {
                            break;
                        }
                    }

                    Teletype.Tick();
                    FloppyDrive?.Tick();

                    Interrupts.Interrupt();

                    var instruction = Fetch(PC.Content);

                    PC.Increment();

                    instruction.Execute();

                    if (debug)
                    {
                        Log.Debug(instruction.ToString());

                        var f = FloppyDrive?.ToString();
                        if (f != floppy)
                        {
                            floppy = f;
                            Log.Information(instruction.ToString());
                            Log.Information(floppy);
                        }

                        var i = Interrupts.ToString();
                        if (i != interrupts)
                        {
                            interrupts = i;
                            Log.Information(interrupts);
                        }
                    }
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

                Log.Information($"HLT @ {PC}");
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

        private IInstruction Decode(int data)
        {
            return (data & Masks.OP_CODE) switch
            {
                Masks.MCI when (data & Masks.GROUP) == 0 => group1Instructions,
                Masks.MCI when (data & Masks.GROUP_3) == Masks.GROUP_3 => group3Instructions,
                Masks.MCI when (data & Masks.GROUP_2_AND) == Masks.GROUP_2_AND => group2AndInstructions,
                Masks.MCI => group2OrInstructions,
                Masks.IOT when (data & Masks.FLOPPY) == Masks.FLOPPY => floppyDriveInstructions,
                Masks.IOT when (data & Masks.MEMORY_MANAGEMENT) == Masks.MEMORY_MANAGEMENT => memoryManagementInstructions,
                Masks.IOT when (data & Masks.INTERRUPT_MASK) == 0 => interruptInstructions,
                Masks.IOT when (data & Masks.IO) >> 3 == 3 => keyboardInstructions,
                Masks.IOT when (data & Masks.IO) >> 3 == 4 => teleprinterInstructions,
                Masks.IOT => privilegedNoOperationInstruction,
                _ => memoryReferenceInstructions,
            };
        }

        public void SetBreakpoint(int address)
        {
            Log.Information($"Breakpoint set @ {address.ToOctalString(5)}");

            breakpoints.Add(address);

            debug = true;
        }

        public void RemoveBreakpoint(int address)
        {
            breakpoints.Remove(address);
        }

        public void RemoveAllBreakpoints()
        {
            breakpoints.Clear();
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
    }
}
