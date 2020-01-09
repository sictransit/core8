using Core8.Model.Enums;
using Core8.Model.Extensions;
using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;
using System.Linq;

namespace Core8.Model.Instructions
{
    public class MemoryReferenceInstruction : InstructionBase
    {
        private readonly IMemory memory;
        private readonly IRegisters registers;

        public MemoryReferenceInstruction(uint address, uint data, IMemory memory, IRegisters registers) : base(address, data)
        {
            this.memory = memory;
            this.registers = registers;
        }

        private MemoryReferenceOpCode OpCode => (MemoryReferenceOpCode)(Data & Masks.OP_CODE);

        private AddressingModes AddressingModes => (AddressingModes)(Data & Masks.ADDRESSING_MODE);

        protected override string OpCodeText => string.Join(" ", (new[] { OpCode.ToString(), Indirect ? "I" : null, Zero ? "Z" : null }).Where(x => !string.IsNullOrEmpty(x)));

        private bool Indirect => AddressingModes.HasFlag(AddressingModes.I);

        private bool Zero => !AddressingModes.HasFlag(AddressingModes.Z);

        public uint Location => Zero ? (Data & Masks.ADDRESS_WORD) : (Address & Masks.ADDRESS_PAGE) | (Data & Masks.ADDRESS_WORD);

        public override void Execute()
        {
            var address = Indirect ? memory.Read(Location, true) : Location;

            switch (OpCode)
            {
                case MemoryReferenceOpCode.AND:
                    AND(address);
                    break;
                case MemoryReferenceOpCode.DCA:
                    DCA(address);
                    break;
                case MemoryReferenceOpCode.ISZ:
                    ISZ(address);
                    break;
                case MemoryReferenceOpCode.JMP:
                    JMP(address);
                    break;
                case MemoryReferenceOpCode.JMS:
                    JMS(address);
                    break;
                case MemoryReferenceOpCode.TAD:
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

            value = (value + 1) & Masks.MEM_WORD;

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

        public void JMS(uint address)
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

        public override string ToString()
        {
            return $"{base.ToString()} ({Location.ToOctalString()})";
        }
    }

}
