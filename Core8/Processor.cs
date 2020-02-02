using Core8.Model;
using Core8.Model.Extensions;
using Core8.Model.Instructions;
using Core8.Model.Interfaces;
using Serilog;
using System;
using System.Diagnostics;

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

        private readonly Group1Instructions group1Instructions;
        private readonly Group2ANDInstructions group2ANDInstructions;
        private readonly Group2ORInstructions group2ORInstructions;
        private readonly Group3Instructions group3Instructions;
        private readonly MemoryReferenceInstructions memoryReferenceInstructions;
        private readonly MemoryManagementInstructions memoryManagementInstructions;
        private readonly KeyboardInstructions keyboardInstructions;
        private readonly TeleprinterInstructions teleprinterInstructions;
        private readonly InterruptInstructions interruptInstructions;

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
        }

        public bool InterruptsEnabled { get; private set; }

        public bool InterruptPending => InterruptsEnabled | interruptDelay;

        public bool InterruptRequested => teleprinter.InterruptRequested | userInterruptRequested;

        public bool InterruptsInhibited { get; private set; }

        public void SingleStep(bool state)
        {
            singleStep = state;
        }

        public void Clear()
        {
            teleprinter.Clear();
            registers.LINK_AC.Clear();
            registers.MQ.Clear();
            DisableInterrupts();
        }

        public void Halt()
        {
            running = false;
        }

        public void EnableInterrupts()
        {
            interruptDelay = true;
        }

        public void DisableInterrupts()
        {
            interruptDelay = InterruptsEnabled = userInterruptRequested = false;
        }

        public void InhibitInterrupts()
        {
            InterruptsInhibited = true;
        }

        public void ResumeInterrupts()
        {
            InterruptsInhibited = false;
        }

        public void Run()
        {
            running = true;

            Log.Information($"CONT @ {registers.IF_PC}");

            while (running)
            {
                FetchAndExecute();

                if (singleStep)
                {
                    break;
                }
            }

            Log.Information($"HLT @ {registers.IF_PC}");
        }

        public void FetchAndExecute()
        {
            var enableInterrupts = interruptDelay;

            var instruction = Fetch(registers.IF_PC.IF, registers.IF_PC.Address);

            registers.IF_PC.Increment();

            if (registers.UF.Data != 0 && instruction?.Privileged == true)
            {
                userInterruptRequested = true;
            }
            else
            {
                if (instruction != null)
                {
                    if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
                    {
                        Log.Debug(instruction.ToString());
                    }

                    instruction.Execute();
                }

                if (enableInterrupts && interruptDelay)
                {
                    interruptDelay = false;
                    InterruptsEnabled = true;
                }

                if (InterruptsEnabled && InterruptRequested && !InterruptsInhibited)
                {
                    Interrupt();
                }
            }
        }

        private void Interrupt()
        {
            DisableInterrupts();

            if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
            {
                Log.Debug("Interrupt!");
            }

            memory.Write(0, 0, registers.IF_PC.Address); // JMS 0000

            registers.SF.SetIF(registers.IF_PC.IF);
            registers.SF.SetDF(registers.DF.Data);
            registers.SF.SetUF(registers.UF.Data);

            registers.DF.Clear();
            registers.IB.Clear();
            registers.UF.Clear();
            registers.UB.Clear();

            registers.IF_PC.SetIF(0);

            registers.IF_PC.SetPC(1);
        }

        public IInstruction Debug8(uint field, uint address)
        {
            return Debug10(field.ToDecimal(), address.ToDecimal());
        }

        public IInstruction Debug10(uint field, uint address)
        {
            return Fetch(field, address);
        }

        private IInstruction Fetch(uint field, uint address)
        {
            var data = memory.Read(field, address);

            var instruction = Decode(data);

            if (instruction != null)
            {
                instruction.Load(field, address, data);
            }
            else
            {
                Log.Warning($"[{address.ToOctalString()}] NOP {data.ToOctalString()}");
            }

            return instruction;
        }

        private IInstruction Decode(uint data)
        {
            return (data & Masks.OP_CODE) switch
            {
                Masks.MCI when (data & Masks.GROUP) == 0 => group1Instructions,
                Masks.MCI when ((data & Masks.GROUP_3) == Masks.GROUP_3) && ((data & Masks.GROUP_3_EAE) == 0) => group3Instructions,
                Masks.MCI when (data & Masks.GROUP_3) == Masks.GROUP_3 => null,
                Masks.MCI when (data & Masks.GROUP_2_AND) == Masks.GROUP_2_AND => group2ANDInstructions,
                Masks.MCI => group2ORInstructions,
                Masks.IOT when (data & Masks.MEMORY_MANAGEMENT) == Masks.MEMORY_MANAGEMENT => memoryManagementInstructions,
                Masks.IOT when (data & Masks.INTERRUPT_MASK) == 0 => interruptInstructions,
                Masks.IOT when (data & Masks.IO) >> 3 == 3 => keyboardInstructions,
                Masks.IOT when (data & Masks.IO) >> 3 == 4 => teleprinterInstructions,
                Masks.IOT => null,
                _ => memoryReferenceInstructions,
            };
        }

    }
}
