﻿using Core8.Model;
using Core8.Model.Extensions;
using Core8.Model.Instructions;
using Core8.Model.Interfaces;
using Serilog;
using System;
using System.Collections.Generic;

namespace Core8
{
    public class Processor : IProcessor
    {
        private volatile bool running = false;

        private bool interruptDelay = false;

        private bool userInterruptRequested = false;

        private bool singleStep = false;

        private readonly IMemory memory;

        private readonly IRegisters registers;

        private readonly ITeleprinter teleprinter;

        private readonly HashSet<uint> breakpoints = new HashSet<uint>();

        private readonly Group1Instructions group1Instructions;
        private readonly Group2ANDInstructions group2ANDInstructions;
        private readonly Group2ORInstructions group2ORInstructions;
        private readonly Group3Instructions group3Instructions;
        private readonly MemoryReferenceInstructions memoryReferenceInstructions;
        private readonly MemoryManagementInstructions memoryManagementInstructions;
        private readonly KeyboardInstructions keyboardInstructions;
        private readonly TeleprinterInstructions teleprinterInstructions;
        private readonly InterruptInstructions interruptInstructions;
        private readonly NoOperationInstruction noOperationInstruction;

        public Processor(IMemory memory, IRegisters registers, ITeleprinter teleprinter)
        {
            this.memory = memory ?? throw new ArgumentNullException(nameof(memory));
            this.registers = registers ?? throw new ArgumentNullException(nameof(registers));
            this.teleprinter = teleprinter ?? throw new ArgumentNullException(nameof(teleprinter));

            group1Instructions = new Group1Instructions(registers);
            group2ANDInstructions = new Group2ANDInstructions(this, registers);
            group2ORInstructions = new Group2ORInstructions(this, registers);
            group3Instructions = new Group3Instructions(registers);
            memoryReferenceInstructions = new MemoryReferenceInstructions(this, memory, registers);
            memoryManagementInstructions = new MemoryManagementInstructions(this, memory, registers);
            keyboardInstructions = new KeyboardInstructions(registers, teleprinter);
            teleprinterInstructions = new TeleprinterInstructions(registers, teleprinter);
            interruptInstructions = new InterruptInstructions(this, registers);
            noOperationInstruction = new NoOperationInstruction(registers);
        }

        public bool InterruptsEnabled { get; private set; }

        public bool InterruptPending => InterruptsEnabled | interruptDelay;

        public bool InterruptRequested => teleprinter.InterruptRequested | userInterruptRequested;

        public bool InterruptsInhibited { get; private set; }

        public bool UserInterruptRequested => userInterruptRequested;

        public void SingleStep(bool state)
        {
            singleStep = state;
        }

        public void Clear()
        {
            teleprinter.Clear();
            registers.LINK_AC.Clear();
            userInterruptRequested = false;
            DisableInterrupts();
        }

        public void Halt()
        {
            running = false;
        }

        public void EnableInterrupts(bool delay = true)
        {
            if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
            {
                Log.Debug($"Interrupts enabled (delay: {delay}, irq: {InterruptRequested})");
            }

            if (delay)
            {
                interruptDelay = true;
            }
            else
            {
                InterruptsEnabled = true;
            }
        }

        public void DisableInterrupts()
        {
            if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
            {
                Log.Debug($"Interrupts disbled (irq: {InterruptRequested})");
            }

            InterruptsEnabled = false;
        }

        public void InhibitInterrupts()
        {
            if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
            {
                Log.Debug($"Interrupts inhibited (irq: {InterruptRequested})");
            }

            InterruptsInhibited = true;
        }

        public void ResumeInterrupts()
        {
            if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
            {
                Log.Debug($"Interrupts resumed (irq: {InterruptRequested})");
            }

            InterruptsInhibited = false;
        }

        public void Run()
        {
            running = true;

            Log.Information($"CONT @ {registers.IF_PC}");

            while (running)
            {
                if (breakpoints.Contains(registers.IF_PC.Data))
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

                    running = false;
                }

                if (singleStep)
                {
                    break;
                }
            }

            running = false;

            Log.Information($"HLT @ {registers.IF_PC}");
        }

        public void FetchAndExecute()
        {
            if (InterruptsEnabled && InterruptRequested && !InterruptsInhibited)
            {
                Interrupt();
            }

            if (interruptDelay)
            {
                interruptDelay = false;
                InterruptsEnabled = true;

                if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
                {
                    Log.Debug($"Interrupts enabled (irq: {InterruptRequested})");
                }
            }

            var instruction = Fetch(registers.IF_PC.IF_PC);

            registers.IF_PC.Increment();

            if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
            {
                Log.Debug(instruction.ToString());
            }

            instruction.Execute();

            if (instruction.UserModeInterrupt)
            {
                userInterruptRequested = true;

                if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
                {
                    Log.Debug($"User interrupt set (irq: {InterruptRequested})");
                }
            }
        }

        private void Interrupt()
        {
            if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
            {
                Log.Debug("Interrupt!");
            }

            memory.Write(0, registers.IF_PC.Address); // JMS 0000

            registers.SF.SetIF(registers.IF_PC.IF);
            registers.SF.SetDF(registers.DF.Data);
            registers.SF.SetUF(registers.UF.Data);

            registers.DF.Clear();
            registers.IB.Clear();
            registers.UF.Clear();
            registers.UB.Clear();

            registers.IF_PC.SetIF(0);

            registers.IF_PC.SetPC(1);

            DisableInterrupts();
        }

        public IInstruction Debug8(uint address)
        {
            return Debug10(address.ToDecimal());
        }

        public IInstruction Debug10(uint address)
        {
            return Fetch(address);
        }

        private IInstruction Fetch(uint address)
        {
            var data = memory.Read(address);

            var instruction = Decode(data);

            return instruction.Load(address, data);
        }

        private IInstruction Decode(uint data)
        {
            return (data & Masks.OP_CODE) switch
            {
                Masks.MCI when (data & Masks.GROUP) == 0 => group1Instructions,
                Masks.MCI when ((data & Masks.GROUP_3) == Masks.GROUP_3) && ((data & Masks.GROUP_3_EAE) == 0) => group3Instructions,
                Masks.MCI when (data & Masks.GROUP_3) == Masks.GROUP_3 => noOperationInstruction,
                Masks.MCI when (data & Masks.GROUP_2_AND) == Masks.GROUP_2_AND => group2ANDInstructions,
                Masks.MCI => group2ORInstructions,
                Masks.IOT when (data & Masks.MEMORY_MANAGEMENT) == Masks.MEMORY_MANAGEMENT => memoryManagementInstructions,
                Masks.IOT when (data & Masks.INTERRUPT_MASK) == 0 => interruptInstructions,
                Masks.IOT when (data & Masks.IO) >> 3 == 3 => keyboardInstructions,
                Masks.IOT when (data & Masks.IO) >> 3 == 4 => teleprinterInstructions,
                Masks.IOT => noOperationInstruction,
                _ => memoryReferenceInstructions,
            };
        }

        public void SetBreakpoint(uint address)
        {
            Log.Information($"Breakpoint set @ {address.ToOctalString(5)}");

            breakpoints.Add(address);
        }

        public void RemoveBreakpoint(uint address)
        {
            breakpoints.Remove(address);
        }

        public void RemoveAllBreakpoints()
        {
            breakpoints.Clear();
        }

        public void ClearUserInterrupt()
        {
            userInterruptRequested = false;

            if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
            {
                Log.Debug($"User interrupt cleared (irq: {InterruptRequested})");
            }
        }
    }
}
