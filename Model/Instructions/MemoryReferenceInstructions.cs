using Core8.Model.Enums;
using Core8.Model.Extensions;
using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;
using System.Linq;

namespace Core8.Model.Instructions
{
    public class MemoryReferenceInstructions : InstructionsBase
    {
        private readonly IMemory memory;

        public MemoryReferenceInstructions(IMemory memory, IRegisters registers) : base(registers)
        {
            this.memory = memory;
        }

        private MemoryReferenceOpCode OpCode => (MemoryReferenceOpCode)(Data & Masks.OP_CODE);

        private AddressingModes AddressingModes => (AddressingModes)(Data & Masks.ADDRESSING_MODE);

        protected override string OpCodeText => string.Join(" ", (new[] { OpCode.ToString(), Indirect ? "I" : null, Zero ? "Z" : null }).Where(x => !string.IsNullOrEmpty(x)));

        private bool Indirect => AddressingModes.HasFlag(AddressingModes.I);

        private bool Zero => !AddressingModes.HasFlag(AddressingModes.Z);

        public uint Location => Zero ? (Data & Masks.ADDRESS_WORD) : (Address & Masks.ADDRESS_PAGE) | (Data & Masks.ADDRESS_WORD);

        protected override void Execute()
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
            Registers.LINK_AC.SetAccumulator(memory.Read(address) & Registers.LINK_AC.Accumulator);
        }

        private void DCA(uint address)
        {
            memory.Write(address, Registers.LINK_AC.Accumulator);

            Registers.LINK_AC.SetAccumulator(0);
        }

        private void ISZ(uint address)
        {
            memory.Write(address, (memory.Read(address) + 1) & Masks.MEM_WORD);

            if (memory.MB == 0)
            {
                Registers.IF_PC.Increment();
            }
        }

        private void JMP(uint address)
        {
            Registers.IF_PC.Set(address);
        }

        public void JMS(uint address)
        {
            memory.Write(address, Registers.IF_PC.Address);

            Registers.IF_PC.Set(address + 1);
        }

        private void TAD(uint address)
        {
            Registers.LINK_AC.Set(Registers.LINK_AC.Accumulator + memory.Read(address));
        }

        public override string ToString()
        {
            return $"{base.ToString()} ({Location.ToOctalString()})";
        }
    }

}
