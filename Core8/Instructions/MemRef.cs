using Core8.Enums;
using Core8.Interfaces;
using System;

namespace Core8.Instructions.Abstract
{
    public class MemRef : InstructionBase
    {
        private readonly IProcessor processor;
        private readonly IMemory memory;

        public MemRef(IRegisters registers, IProcessor processor, IMemory memory) : base(registers)
        {
            this.processor = processor;
            this.memory = memory;
        }

        public override void Execute(uint data)
        {
            var opCode = (MemoryReferenceInstruction)(data & Masks.OP_CODE);
            var addressingMode = (AddressingModes)(data & Masks.ADDRESSING_MODE);

            var location = addressingMode.HasFlag(AddressingModes.Z) ? (processor.CurrentAddress & Masks.ADDRESS_PAGE) | (data & Masks.ADDRESS_WORD) : data & Masks.ADDRESS_WORD;

            var address = addressingMode.HasFlag(AddressingModes.I) ? memory.Read(location) : location;

            switch (opCode)
            {
                case MemoryReferenceInstruction.AND:
                    AND(address);
                    break;
                case MemoryReferenceInstruction.DCA:
                    DCA(address);
                    break;
                case MemoryReferenceInstruction.ISZ:
                    ISZ(address);
                    break;
                case MemoryReferenceInstruction.JMP:
                    JMP(address);
                    break;
                case MemoryReferenceInstruction.JMS:
                    JMS(address);
                    break;
                case MemoryReferenceInstruction.TAD:
                    TAD(address);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void AND(uint address)
        {
            var value = memory.Read(address);

            var ac = Registers.LINK_AC.Accumulator;

            Registers.LINK_AC.SetAccumulator(value & ac);
        }

        private void DCA(uint address)
        {
            memory.Write(address, Registers.LINK_AC.Accumulator);

            Registers.LINK_AC.SetAccumulator(0);
        }

        private void ISZ(uint address)
        {
            var value = memory.Read(address);

            value = value + 1 & Masks.MEM_WORD;

            memory.Write(address, value);

            if (value == 0)
            {
                Registers.IF_PC.Increment();
            }
        }

        private void JMP(uint address)
        {
            Registers.IF_PC.Set(address);
        }

        private void JMS(uint address)
        {
            var pc = Registers.IF_PC.Address;

            memory.Write(address, pc);

            Registers.IF_PC.Set(address + 1);
        }

        private void TAD(uint address)
        {
            var value = memory.Read(address);

            var ac = Registers.LINK_AC.Accumulator;

            Registers.LINK_AC.Set(ac + value);
        }
    }

}
