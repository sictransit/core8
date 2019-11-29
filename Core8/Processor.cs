using Core8.Instructions;
using Core8.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core8
{
    public class Processor : IProcessor
    {
        private readonly Memory ram;
        private readonly Registers registers;

        private readonly ICore core;

        public Processor(Memory memory)
        {
            ram = memory;
            registers = new Registers();

            core = new Core(this, ram, registers);
        }

        public ushort InstructionField => registers.IF_PC.IF;

        public ushort ProgramCounterPage => registers.IF_PC.Page;

        public ushort ProgramCounterWord => registers.IF_PC.Word;

        public ushort Accumulator => registers.LINK_AC.Accumulator;

        public bool Halted { get; private set; }

        public void Deposit(InstructionBase instruction)
        {
            ram.Load(registers.IF_PC.Address, instruction);

            registers.IF_PC.Increment();
        }

        public void Deposit(ushort data)
        {
            ram.Write(registers.IF_PC.Address, (ushort)(data & Masks.MEM_WORD));

            registers.IF_PC.Increment();
        }

        public void Halt()
        {
            Halted = true;
        }

        public void Run()
        {
            registers.IF_PC.Reset();

            Halted = false;

            while (!Halted)
            {
                var data = ram.Read(registers.IF_PC.Address);

                var opCode = (data & Masks.OP_CODE) >> 9;

                InstructionBase instruction;

                switch (opCode)
                {
                    case 0b_000:
                        instruction = new AND((ushort)(data & Masks.ADDRESS_WORD));
                        break;
                    case 0b_001:
                        instruction = new TAD((ushort)(data & Masks.ADDRESS_WORD));
                        break;
                    case 0b_111 when (data & Masks.I_MODE) != 0:
                        instruction = new HLT();
                        break;
                    default:
                        throw new NotImplementedException();
                }

                registers.IF_PC.Increment();

                instruction.Execute(core);
            }
        }
    }
}
