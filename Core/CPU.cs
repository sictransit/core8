using Core8.Extensions;
using Core8.Model;
using Core8.Model.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Core8.Model.Registers;

namespace Core8.Core
{
    public class CPU : ICPU
    {
        private volatile bool running;

        private bool singleStep;

        private bool debug;

        private readonly HashSet<int> breakpoints = new HashSet<int>();

        private readonly InstructionSet instructionSet;

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

            instructionSet = new InstructionSet(this);
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
            string registerAC = null;

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

                    Interrupts.Interrupt();

                    Teletype.Tick();
                    FloppyDrive?.Tick();

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
                Masks.MCI when (data & Masks.GROUP) == 0 => instructionSet.Group1,
                Masks.MCI when (data & Masks.GROUP_3) == Masks.GROUP_3 => instructionSet.Group3,
                Masks.MCI when (data & Masks.GROUP_2_AND) == Masks.GROUP_2_AND => instructionSet.Group2AND,
                Masks.MCI => instructionSet.Group2OR,
                Masks.IOT when (data & Masks.FLOPPY) == Masks.FLOPPY => instructionSet.FloppyDrive,
                Masks.IOT when (data & Masks.MEMORY_MANAGEMENT) == Masks.MEMORY_MANAGEMENT => instructionSet.MemoryManagement,
                Masks.IOT when (data & Masks.INTERRUPT_MASK) == 0 => instructionSet.Interrupt,
                Masks.IOT when (data & Masks.IO) >> 3 == 3 => instructionSet.Keyboard,
                Masks.IOT when (data & Masks.IO) >> 3 == 4 => instructionSet.Teleprinter,
                Masks.IOT => instructionSet.PrivilegedNOP,
                _ => instructionSet.MemoryReference,
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
