using Core8.Model;
using Core8.Model.Extensions;
using Core8.Model.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;

namespace Core8
{
    public class CPU : ICPU
    {
        private volatile bool running = false;

        private bool singleStep = false;

        private bool debug = false;

        private readonly HashSet<int> breakpoints = new HashSet<int>();

        private readonly InstructionSet instructionSet;

        public CPU(ITeletype teletype)
        {
            Teletype = teletype ?? throw new ArgumentNullException(nameof(teletype));

            Memory = new Memory();
            Registers = new Registers();
            FloppyDrive = new FloppyDrive();

            Interrupts = new Interrupts(this);

            instructionSet = new InstructionSet(this);
        }

        public IInterrupts Interrupts { get; private set; }

        public IRegisters Registers { get; private set; }

        public ITeletype Teletype { get; private set; }

        public IFloppyDrive FloppyDrive { get; private set; }

        public IMemory Memory { get; private set; }

        public void Clear()
        {
            Teletype.Clear();
            Registers.AC.Clear();
            Interrupts.ClearUser();
            Interrupts.Disable();
            FloppyDrive.Initialize();
        }

        public void Halt()
        {
            running = false;
        }

        public void Run()
        {
            running = true;

            Log.Information($"CONT @ {Registers.PC} (dbg: {debug})");

            string interrupts = null;
            string floppy = null;
            string teletype = null;

            int tick = 0;

            try
            {
                while (running)
                {
                    if (debug)
                    {
                        var f = FloppyDrive.ToString();
                        if (f != floppy)
                        {
                            floppy = f;
                            Log.Information(floppy);
                        }

                        var i = Interrupts.ToString();
                        if (i != interrupts)
                        {
                            interrupts = i;
                            Log.Debug(interrupts);
                        }

                        var t = Teletype.ToString();
                        if (t != teletype)
                        {
                            teletype = t;
                            Log.Debug(teletype);
                        }

                        if (breakpoints.Contains(Registers.PC.Content))
                        {
                            Log.Information($"Breakpoint hit!");

                            break;
                        }

                        if (singleStep)
                        {
                            break;
                        }
                    }

                    Interrupts.Interrupt();

                    if (tick++ > 100)
                    {
                        Teletype.Tick();
                        FloppyDrive.Tick();
                        tick = 0;
                    }

                    var instruction = Fetch(Registers.PC.Content);

                    Registers.PC.Increment();

                    if (debug)
                    {
                        Log.Debug(instruction.ToString());
                    }

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

                Log.Information($"HLT @ {Registers.PC}");
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
                Masks.MCI when ((data & Masks.GROUP_3) == Masks.GROUP_3) && ((data & Masks.GROUP_3_EAE) == 0) => instructionSet.Group3,
                Masks.MCI when (data & Masks.GROUP_3) == Masks.GROUP_3 => instructionSet.NOP,
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
