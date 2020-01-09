using Core8.Model;
using Core8.Model.Enums;
using Core8.Model.Extensions;
using Core8.Model.Instructions;
using Core8.Model.Instructions.Abstract;
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

        public Processor(IMemory memory, IRegisters registers, IKeyboard keyboard, ITeleprinter teleprinter)
        {
            this.memory = memory;
            this.registers = registers;
            this.keyboard = keyboard;
            this.teleprinter = teleprinter;
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

            var instruction = Decode(address, data, this, memory, registers, keyboard, teleprinter);

            if (instruction != null)
            {
                Log.Debug(instruction.ToString());

                instruction.Execute();
            }
            else
            {
                Log.Warning($"[{address.ToOctalString()}] NOP {data.ToOctalString()}");
            }
        }

        public static InstructionBase Decode(uint address, uint data, IProcessor processor, IMemory memory, IRegisters registers, IKeyboard keyboard, ITeleprinter teleprinter)
        {
            return ((InstructionClass)(data & Masks.OP_CODE)) switch
            {
                InstructionClass.MCI when (data & Masks.GROUP) == 0 => new Group1Instruction(address, data, registers),
                InstructionClass.MCI when (data & Masks.GROUP_2_AND) == Masks.GROUP_2_AND => new Group2ANDInstruction(address, data, processor, registers),
                InstructionClass.MCI => new Group2ORInstruction(address, data, processor, registers),
                InstructionClass.IOT when ((data & Masks.IO) >> 3) == keyboard.Id => new KeyboardInstruction(address, data, registers, keyboard),
                InstructionClass.IOT when ((data & Masks.IO) >> 3) == teleprinter.Id => new TeleprinterInstruction(address, data, registers, teleprinter),
                InstructionClass.IOT => null,
                _ => new MemoryReferenceInstruction(address, data, memory, registers),
            };
        }
    }
}
