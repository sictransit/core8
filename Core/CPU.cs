using Core8.Extensions;
using Core8.Model;
using Core8.Model.Interfaces;
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

        private readonly HashSet<int> breakpoints = new();

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

            try
            {
                while (running)
                {
                    if (debug)
                    {
                        if (breakpoints.Contains(Registry.PC.Content))
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

                    var instruction = Fetch(Registry.PC.Content);

                    Registry.PC.Increment();

                    instruction.Execute();

                    if (debug)
                    {
                        Log.Debug(instruction.ToString());

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
