using Core8.Enums;
using Core8.Extensions;
using Core8.Interfaces;
using System;

namespace Core8.Instructions.Abstract
{
    public class MemoryReferenceInstructions : InstructionBase
    {
        private readonly IProcessor processor;
        private readonly IMemory memory;
        private readonly IRegisters registers;

        public MemoryReferenceInstructions(IProcessor processor, IRegisters registers, IMemory memory)
        {
            this.processor = processor;
            this.registers = registers;
            this.memory = memory;
        }

        public void Execute(uint data)
        {
            var opCode = (InstructionName)(data & Masks.OP_CODE);
            var addressingMode = (AddressingModes)(data & Masks.ADDRESSING_MODE);

            var location = addressingMode.HasFlag(AddressingModes.Z) ? (processor.CurrentAddress & Masks.ADDRESS_PAGE) | (data & Masks.ADDRESS_WORD) : data & Masks.ADDRESS_WORD;

            var address = addressingMode.HasFlag(AddressingModes.I) ? memory.Read(location) : location;

            switch (opCode)
            {
                case InstructionName.AND:
                    AND(address);
                    break;
                case InstructionName.DCA:
                    DCA(address); 
                    break;
                case InstructionName.ISZ:
                    ISZ(address);
                    break;
                case InstructionName.JMP:
                    JMP(address); 
                    break;
                case InstructionName.JMS:
                    JMS(address); 
                    break;
                case InstructionName.TAD:
                    TAD(address); 
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void AND(uint address)
        {
            var value = memory.Read(address);

            var ac = registers.LINK_AC.Accumulator;

            registers.LINK_AC.SetAccumulator(value & ac);
        }

        private void DCA(uint address)
        {
            memory.Write(address, registers.LINK_AC.Accumulator);

            registers.LINK_AC.SetAccumulator(0);
        }

        private void ISZ(uint address)
        {
            var value = memory.Read(address);

            value = value + 1 & Masks.MEM_WORD;

            memory.Write(address, value);

            if (value == 0)
            {
                registers.IF_PC.Increment();
            }
        }

        private void JMP(uint address)
        {
            registers.IF_PC.Set(address);
        }

        private void JMS(uint address)
        {
            var pc = registers.IF_PC.Address;

            memory.Write(address, pc);

            registers.IF_PC.Set(address + 1);
        }

        private void TAD(uint address)
        {
            var value = memory.Read(address);

            var ac = registers.LINK_AC.Accumulator;

            registers.LINK_AC.Set(ac + value);
        }
    }

}
