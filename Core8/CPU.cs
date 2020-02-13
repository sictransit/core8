﻿using Core8.Model;
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

        private readonly HashSet<int> breakpoints = new HashSet<int>();

        private readonly InstructionSet instructionSet;

        public CPU(ITeletype teletype)
        {
            Teletype = teletype ?? throw new ArgumentNullException(nameof(teletype));

            Memory = new Memory();
            Registers = new Registers();
            Interrupts = new Interrupts(this);

            instructionSet = new InstructionSet(this);
        }

        public IInterrupts Interrupts { get; private set; }

        public IRegisters Registers { get; private set; }

        public ITeletype Teletype { get; private set; }

        public IMemory Memory { get; private set; }

        public void SingleStep(bool state)
        {
            singleStep = state;
        }

        public void Clear()
        {
            Teletype.Clear();
            Registers.AC.Clear();
            Interrupts.ClearUser();
            Interrupts.Disable();
        }

        public void Halt()
        {
            running = false;
        }

        public void Run()
        {
            running = true;

            Log.Information($"CONT @ {Registers.PC}");

            while (running)
            {
                if (breakpoints.Count != 0 && breakpoints.Contains(Registers.PC.Content))
                {
                    Log.Information($"Breakpoint hit!");

                    break;
                }

                try
                {
                    FetchAndExecute();
                }
                catch (Exception ex)
                {
                    Log.Fatal($"Caught Exception in CPU: {ex.ToString()}");

                    throw;
                }

                if (singleStep)
                {
                    break;
                }
            }

            running = false;

            Log.Information($"HLT @ {Registers.PC}");
        }

        public void FetchAndExecute()
        {
            Interrupts.Interrupt();

            var instruction = Fetch(Registers.PC.Content);

            Registers.PC.Increment();

            if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
            {
                Log.Debug(instruction.ToString());
            }

            instruction.Execute();
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

            var instruction = Decode(data);

            return instruction.Load(address, data);
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
        }

        public void RemoveBreakpoint(int address)
        {
            breakpoints.Remove(address);
        }

        public void RemoveAllBreakpoints()
        {
            breakpoints.Clear();
        }
    }
}