using Core8.Model.Extensions;
using Core8.Model.Instructions;
using Core8.Model.Interfaces;
using Serilog;
using System;
using System.Threading;

namespace Core8
{
    public class Processor : IProcessor
    {
        private readonly ManualResetEvent running = new ManualResetEvent(false);

        private readonly ManualResetEvent interruptEnable = new ManualResetEvent(false);

        private readonly AutoResetEvent interruptDelay = new AutoResetEvent(false);

        private readonly IMemory memory;

        private readonly IRegisters registers;

        private readonly IKeyboard keyboard;

        private readonly ITeleprinter teleprinter;

        private readonly InstructionFactory instructionFactory;

        public Processor(IMemory memory, IRegisters registers, IKeyboard keyboard, ITeleprinter teleprinter)
        {
            this.memory = memory;
            this.registers = registers;
            this.keyboard = keyboard;
            this.teleprinter = teleprinter;

            instructionFactory = new InstructionFactory(this, memory, registers, keyboard, teleprinter);
        }

        public bool InterruptsEnabled => interruptEnable.WaitOne(TimeSpan.Zero);

        public void Halt()
        {
            running.Reset();
        }

        public void EnableInterrupts()
        {
            interruptDelay.Set();
        }

        public void DisableInterrupts()
        {
            interruptDelay.Reset();

            interruptEnable.Reset();
        }

        public void Run()
        {
            running.Set();

            Log.Information("RUN");

            var cnt = 0;

            while (running.WaitOne(TimeSpan.Zero))
            {
                FetchAndExecute();

                if (cnt++ > 100)
                {
                    teleprinter.Tick();
                    keyboard.Tick();

                    cnt = 0;
                }
            }

            Log.Information("HLT");
        }

        public void FetchAndExecute()
        {
            var address = registers.IF_PC.Address;

            var data = memory.Read(address);

            registers.IF_PC.Increment();

            if (instructionFactory.TryFetch(address, data, out var instruction))
            {
                Log.Debug(instruction.ToString());

                instruction.Execute();
            }
            else
            {
                Log.Warning($"[{address.ToOctalString()}] NOP {data.ToOctalString()}");
            }
        }

    }
}
