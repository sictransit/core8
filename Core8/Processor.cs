using Core8.Enum;
using Core8.Instructions.Abstract;
using Core8.Instructions.MemoryReference;
using Core8.Instructions.Microcoded;
using Core8.Interfaces;
using System;
using System.Collections.Generic;

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
            if ((data & Masks.GROUP) == 0) // Group #1
            {
                var microcode = data & Masks.GROUP_1;

                switch (microcode)
                {
                    case Masks.GROUP_1_CLA:
                        yield return new CLA();
                        break;

                    case Masks.GROUP_1_CLL:
                        yield return new CLL();
                        break;
                    case Masks.GROUP_1_CMA:
                        yield return new CMA();
                        break;
                    case Masks.GROUP_1_CML:
                        yield return new CML();
                        break;
                    case Masks.GROUP_1_IAC:
                        yield return new IAC();
                        break;
                    case Masks.GROUP_1_RAR:
                        yield return new RAR();
                        break;
                    case Masks.GROUP_1_RAL:
                        yield return new RAL();
                        break;
                    case Masks.GROUP_1_RTR:
                        yield return new RAR();
                        yield return new RAR();
                        break;
                    case Masks.GROUP_1_RTL:
                        yield return new RAL();
                        yield return new RAL();
                        break;
                    case Masks.GROUP_1_BSW:
                        yield return new BSW();
                        break;
                    default:
                        throw new NotImplementedException(nameof(data));
                }

            }
            else if ((data & Masks.GROUP_2_HLT) == Masks.GROUP_2_HLT)
            {
                yield return new HLT();
            }
            else if ((data & Masks.GROUP_2_OSR) == Masks.GROUP_2_OSR)
            {
                yield return new OSR();
            }
            else if ((data & Masks.GROUP_2_AND) == Masks.GROUP_2_AND)
            {
                yield return new G2A(data);
            }
            else
            {
                yield return new G2O(data);
            }
        }
    }


}
