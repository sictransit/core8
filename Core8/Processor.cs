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

        private readonly ManualResetEvent interruptRequested = new ManualResetEvent(false);

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

        public bool InterruptsEnabled => interruptEnable.WaitOne(TimeSpan.Zero);

        public bool InterruptRequested => interruptRequested.WaitOne(TimeSpan.Zero);

        public void Clear()
        {
            teleprinter.Clear();
            registers.LINK_AC.Clear();
            registers.MQ.Clear();
            DisableInterrupts();
        }

        private void RequestInterrupt()
        {
            interruptRequested.Set();
        }

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

            interruptRequested.Reset();
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

                    cnt = 0;
                }
            }

            Log.Information("HLT");
        }

        public void FetchAndExecute()
        {
            var enableInterrupts = interruptDelay.WaitOne(TimeSpan.Zero);

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

            if (enableInterrupts)
            {
                interruptEnable.Set();
            }
        }

    }
}
