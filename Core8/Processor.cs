using Core8.Enums;
using Core8.Extensions;
using Core8.Interfaces;
using Serilog;
using System;
using System.Threading;

namespace Core8
{
    public class Processor : IProcessor
    {
        private volatile bool halted;

        private readonly IMemory memory;
        private readonly IRegisters registers;
        private readonly IKeyboard keyboard;
        private readonly ITeleprinter teleprinter;

        private readonly InstructionSet instructionSet;

        public Processor(IMemory memory, IRegisters registers, IKeyboard keyboard, ITeleprinter teleprinter)
        {
            instructionSet = new InstructionSet(this, memory, registers, keyboard, teleprinter);

            this.memory = memory;
            this.registers = registers;
            this.keyboard = keyboard;
            this.teleprinter = teleprinter;
        }

        public uint CurrentAddress { get; private set; }

        public void Halt()
        {
            halted = true;
        }

        public void Run()
        {
            halted = false;

            Log.Information("RUN");

            while (!halted)
            {
                CurrentAddress = registers.IF_PC.Address;

                var instruction = memory.Read(CurrentAddress);

                registers.IF_PC.Increment();

                try
                {
                    Execute(instruction);
                }
                catch (NotImplementedException)
                {
                    Log.Warning($"[{CurrentAddress.ToOctalString()}] NOP {instruction.ToOctalString()}");

                }


                keyboard.Tick();
                teleprinter.Tick();

                Thread.Sleep(0);
            }

            Log.Information("HLT");
        }

        private void Execute(uint data)
        {
            var opCode = (data & Masks.OP_CODE);

            var instructionName = (InstructionClass)opCode;

            switch (instructionName)
            {
                case InstructionClass.MCI:
                    ExecuteMicrocode(data);
                    break;
                case InstructionClass.IOT:
                    ExecuteIO(data);
                    break;
                default:
                    ExecuteMemoryReference(data);
                    break;
            }
        }


        private void ExecuteMemoryReference(uint data)
        {
            instructionSet.MemRef.Execute(data);
        }

        private void ExecuteIO(uint data)
        {
            if ((data & Masks.KEYBOARD_INSTRUCTION_FLAGS) == Masks.KEYBOARD_INSTRUCTION_FLAGS)
            {
                instructionSet.Keyboard.Execute(data);
            }
            else
            {
                instructionSet.Teleprinter.Execute(data);
            }
        }

        private void ExecuteMicrocode(uint data)
        {
            if ((data & Masks.GROUP) == 0) // Group #1
            {
                instructionSet.Group1.Execute(data);
            }
            else if ((data & Masks.GROUP_2_PRIV) != 0)
            {
                instructionSet.Group2Priv.Execute(data);
            }
            else if ((data & Masks.GROUP_2_AND) == Masks.GROUP_2_AND)
            {
                instructionSet.Group2AND.Execute(data);
            }
            else
            {
                instructionSet.Group2OR.Execute(data);
            }
        }
    }
}
