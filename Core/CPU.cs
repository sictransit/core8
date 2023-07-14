using Core8.Extensions;
using Core8.Model;
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
        private volatile bool running;

        private bool singleStep;

        private bool debug;

        private readonly List<Func<ICPU, bool>> breakpoints = new();

        public CPU(ITeletype teletype, IFloppyDrive floppy)
        {
            Teletype = teletype ?? throw new ArgumentNullException(nameof(teletype));

            Memory = new Memory();

            FloppyDrive = floppy;

            Interrupts = new Interrupts(this);

            InstructionSet = new InstructionSet(this);

            Registry = new Registry();
        }

        public IRegistry Registry { get; }

        public IInstructionSet InstructionSet { get; }

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

            return InstructionSet.Decode(data).Load(address, data);
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
    }
}
