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

        private readonly InstructionFactory instructionFactory;

        public Processor(IMemory memory, IRegisters registers, ITeleprinter teleprinter)
        {
            this.memory = memory ?? throw new ArgumentNullException(nameof(memory));
            this.registers = registers ?? throw new ArgumentNullException(nameof(registers));
            this.teleprinter = teleprinter ?? throw new ArgumentNullException(nameof(teleprinter));

            teleprinter.SetIRQHook(RequestInterrupt);

            instructionFactory = new InstructionFactory(this, memory, registers, teleprinter);
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

                if (cnt++ > 100)
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

            var address = registers.IF_PC.Address;

            var data = memory.Read(address);

            registers.IF_PC.Increment();

            var instruction = instructionFactory.Fetch(address, data);

            if (instruction != null)
            {
                Log.Debug(instruction.ToString());

                instruction.Execute();
            }
            else
            {
                Log.Debug($"[{address.ToOctalString()}] NOP {data.ToOctalString()}");
            }

            if (enableInterrupts && interruptDelay)
            {
                interruptDelay = false;

                InterruptsEnabled = true;
            }

            if (InterruptRequested && InterruptsEnabled)
            {
                DisableInterrupts();

                memory.Write(0, registers.IF_PC.Address); // JMS 0000

                registers.IF_PC.Set(1);
            }
        }

    }
}
