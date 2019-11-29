using Core8.Enum;
using Core8.Instructions.Abstract;
using Core8.Instructions.MemoryReference;
using Core8.Instructions.Microcoded;
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

        public uint InstructionField => registers.IF_PC.IF;

        public uint ProgramCounterPage => registers.IF_PC.Page;

        public uint ProgramCounterWord => registers.IF_PC.Word;

        public uint Accumulator => registers.LINK_AC.Accumulator;

        public bool Halted { get; private set; }

        public void Deposit(InstructionBase instruction)
        {
            ram.Load(registers.IF_PC.Address, instruction);

            registers.IF_PC.Increment();
        }

        public void Deposit(uint data)
        {
            ram.Write(registers.IF_PC.Address, data & Masks.MEM_WORD);

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

                var instructionName = (InstructionName)opCode;

                var instructions = instructionName == InstructionName.Microcoded ? DecodeMicrocodedInstruction(data) : DecodeMemoryReferenceInstruction(instructionName, data);

                registers.IF_PC.Increment();

                foreach (var instruction in instructions)
                {
                    instruction.Execute(core);
                }
            }
        }

        private IEnumerable<InstructionBase> DecodeMemoryReferenceInstruction(InstructionName name, uint data)
        {
            var address = data & Masks.ADDRESS_WORD;

            switch (name)
            {
                case InstructionName.AND:
                    yield return new AND(address);
                    break;
                case InstructionName.TAD:
                    yield return new TAD(address);
                    break;
                case InstructionName.ISZ:
                    yield return new ISZ(address);
                    break;
                case InstructionName.DCA:
                    yield return new DCA(address);
                    break;
                case InstructionName.JMS:
                    yield return new JMS(address);
                    break;
                case InstructionName.JMP:
                    yield return new JMP(address);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private IEnumerable<InstructionBase> DecodeMicrocodedInstruction(uint data)
        {
            if ((data & Masks.GROUP_2) != 0)
            {
                if ((data & Masks.GROUP_2_HLT) == Masks.GROUP_2_HLT)
                {
                    yield return new HLT();
                }
                else
                {
                    throw new NotImplementedException(nameof(data));
                }
            }
            else if ((data & Masks.GROUP_1) != 0)
            {
                if ((data & Masks.GROUP_1_CLA) != 0)
                {
                    yield return new CLA();
                }

                if ((data & Masks.GROUP_1_CLL) != 0)
                {
                    yield return new CLL();
                }

                if ((data & Masks.GROUP_1_CMA) != 0)
                {
                    yield return new CMA();
                }

                if ((data & Masks.GROUP_1_CML) != 0)
                {
                    yield return new CML();
                }

                if ((data & Masks.GROUP_1_IAC) != 0)
                {
                    yield return new IAC();
                }

                if ((data & Masks.GROUP_1_RAR) != 0)
                {
                    yield return new RAR();
                }

                if ((data & Masks.GROUP_1_RAL) != 0)
                {
                    yield return new RAL();
                }

                if ((data & Masks.GROUP_1_RTR) != 0)
                {
                    yield return new RAR();
                    yield return new RAR();
                }

                if ((data & Masks.GROUP_1_RTL) != 0)
                {
                    yield return new RAL();
                    yield return new RAL();
                }

                if ((data & Masks.GROUP_1_BSW) != 0)
                {
                    yield return new BSW();
                }

            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(data));
            }
        }
    }

    
}
