using Core8.Model;
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

        private bool singleStep = false;

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
        private readonly PrivilegedNoOperationInstruction privilegedNoOperationInstruction;

        public Processor(IMemory memory, IRegisters registers, ITeleprinter teleprinter)
        {
            Memory = memory ?? throw new ArgumentNullException(nameof(memory));
            Teleprinter = teleprinter ?? throw new ArgumentNullException(nameof(teleprinter));
            Registers = registers ?? throw new ArgumentNullException(nameof(registers));

            Interrupts = new Interrupts(registers, memory, teleprinter);

            group1Instructions = new Group1Instructions(this);
            group3Instructions = new Group3Instructions(this);
            memoryReferenceInstructions = new MemoryReferenceInstructions(this);
            noOperationInstruction = new NoOperationInstruction(this);

            group2ANDInstructions = new Group2ANDInstructions(this);
            group2ORInstructions = new Group2ORInstructions(this);
            memoryManagementInstructions = new MemoryManagementInstructions(this);
            keyboardInstructions = new KeyboardInstructions(this);
            teleprinterInstructions = new TeleprinterInstructions(this);
            interruptInstructions = new InterruptInstructions(this);
            privilegedNoOperationInstruction = new PrivilegedNoOperationInstruction(this);
        }

        public IInterrupts Interrupts { get; private set; }

        public IRegisters Registers { get; private set; }

        public ITeleprinter Teleprinter { get; private set; }

        public IMemory Memory { get; private set; }

        public void SingleStep(bool state)
        {
            singleStep = state;
        }

        public void Clear()
        {
            Teleprinter.Clear();
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
                if (breakpoints.Contains(Registers.PC.Data))
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

            var instruction = Fetch(Registers.PC.IF_PC);

            Registers.PC.Increment();

            if (Log.IsEnabled(Serilog.Events.LogEventLevel.Debug))
            {
                Log.Debug(instruction.ToString());
            }

            instruction.Execute();
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
            var data = Memory.Read(address);

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
                Masks.IOT => privilegedNoOperationInstruction,
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
    }
}
