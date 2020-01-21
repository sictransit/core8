using Core8.Model;
using Core8.Model.Extensions;
using Core8.Model.Instructions;
using Core8.Model.Interfaces;
using Serilog;
using System;

namespace Core8
{
    public class Processor : IProcessor
    {
        private volatile bool running = false;

        private bool interruptDelay = false;

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

            teleprinter.SetIRQHook(RequestInterrupt);

            group1Instructions = new Group1Instructions(registers);
            group2ANDInstructions = new Group2ANDInstructions(this, registers);
            group2ORInstructions = new Group2ORInstructions(this, registers);
            group3Instructions = new Group3Instructions(registers);
            memoryReferenceInstructions = new MemoryReferenceInstructions(memory, registers);
            memoryManagementInstructions = new MemoryManagementInstructions(memory, registers);
            keyboardInstructions = new KeyboardInstructions(registers, teleprinter);
            teleprinterInstructions = new TeleprinterInstructions(registers, teleprinter);
            interruptInstructions = new InterruptInstructions(this, registers);
        }

        public bool InterruptsEnabled { get; set; }

        public bool InterruptRequested { get; set; }

        public bool InterruptsPending => InterruptsEnabled || interruptDelay;

        public void Clear()
        {
            teleprinter.Clear();
            registers.LINK_AC.Clear();
            registers.MQ.Clear();
            DisableInterrupts();
        }

        private void RequestInterrupt(bool state)
        {
            InterruptRequested = state;
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
            interruptDelay = false;

            InterruptsEnabled = false;

            InterruptRequested = false;
        }

        public void Run()
        {
            running = true;

            Log.Information("RUN");

            var cnt = 0;

            while (running)
            {
                FetchAndExecute();

                if (cnt++ > 100) // A delay is needed. 100 ticks? 1000 ticks? Some other solution? Don't know yet.
                {
                    teleprinter.Tick();

                    cnt = 0;
                }
            }

            Log.Information("HLT");
        }

        public void FetchAndExecute()
        {
            var enableInterrupts = interruptDelay;

            var instruction = Fetch(registers.IF_PC.Address);

            registers.IF_PC.Increment();

            if (instruction != null)
            {
                Execute(instruction);
            }

            if (enableInterrupts && interruptDelay)
            {
                interruptDelay = false;

                InterruptsEnabled = true;
            }

            if (InterruptRequested && InterruptsEnabled)
            {
                DisableInterrupts();

                memory.Write(0, 0, registers.IF_PC.Address); // JMS 0000

                registers.IF_PC.SetPC(1);
            }
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
            var data = memory.Read(0, address);

            var instruction = Decode(data);

            if (instruction != null)
            {
                instruction.Load(address, data);
            }
            else
            {
                Log.Debug($"[{address.ToOctalString()}] NOP {data.ToOctalString()}");
            }

            return instruction;
        }

        private void Execute(IInstruction instruction)
        {
            Log.Debug(instruction.ToString());

            instruction.Execute();
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
