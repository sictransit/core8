using Core8.Model;
using Core8.Model.Enums;
using Core8.Model.Extensions;
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

        private readonly InstructionSet instructionSet;

        public Processor(IMemory memory, IRegisters registers, IKeyboard keyboard, ITeleprinter teleprinter)
        {
            this.memory = memory;
            this.registers = registers;
            this.keyboard = keyboard;
            this.teleprinter = teleprinter;

            instructionSet = new InstructionSet(this, memory, registers, keyboard, teleprinter);
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

            while (running.WaitOne(TimeSpan.Zero))
            {
                FetchAndExecute();

                teleprinter.Tick();
                keyboard.Tick();
            }

            Log.Information("HLT");
        }

        public void FetchAndExecute()
        {
            var address = registers.IF_PC.Address;

            var data = memory.Read(address);

            registers.IF_PC.Increment();

            var instruction = Decode(data, instructionSet);

            if (instruction != null)
            {
                instruction.LoadAndExecute(address, data);

                Log.Debug(instruction.ToString());
            }
            else
            {
                Log.Warning($"[{address.ToOctalString()}] NOP {data.ToOctalString()}");
            }
        }

        public static IInstruction Decode(uint data, InstructionSet instructionSet)
        {
            if (instructionSet is null)
            {
                throw new ArgumentNullException(nameof(instructionSet));
            }

            return ((InstructionClass)(data & Masks.OP_CODE)) switch
            {
                InstructionClass.MCI when (data & Masks.GROUP) == 0 => instructionSet.Group1,
                InstructionClass.MCI when (data & Masks.GROUP_2_AND) == Masks.GROUP_2_AND => instructionSet.Group2AND,
                InstructionClass.MCI => instructionSet.Group2OR,
                InstructionClass.IOT when ((data & Masks.IO) >> 3) == 3 => instructionSet.Keyboard,
                InstructionClass.IOT when ((data & Masks.IO) >> 3) == 4 => instructionSet.Teleprinter,
                InstructionClass.IOT => null,
                _ => instructionSet.MemoryReference,
            };
        }
    }
}
