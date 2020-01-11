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

        internal MemoryReferenceInstructions(IMemory memory, IRegisters registers) : base(registers)
        {
            this.memory = memory;
        }

        private MemoryReferenceOpCode OpCode => (MemoryReferenceOpCode)(Data & Masks.OP_CODE);

        private AddressingModes AddressingModes => (AddressingModes)(Data & Masks.ADDRESSING_MODE);

        protected override string OpCodeText => string.Join(" ", (new[] { OpCode.ToString(), Indirect ? "I" : null, Zero ? "Z" : null }).Where(x => !string.IsNullOrEmpty(x)));

        private bool Indirect => AddressingModes.HasFlag(AddressingModes.I);

        private bool Zero => !AddressingModes.HasFlag(AddressingModes.Z);

        public uint Location => Zero ? (Data & Masks.ADDRESS_WORD) : (Address & Masks.ADDRESS_PAGE) | (Data & Masks.ADDRESS_WORD);

        public override void Execute()
        {
            var operand = Indirect ? memory.Read(Location, true) : Location;

            switch (OpCode)
            {
                case MemoryReferenceOpCode.AND:
                    AND(operand);
                    break;
                case MemoryReferenceOpCode.DCA:
                    DCA(operand);
                    break;
                case MemoryReferenceOpCode.ISZ:
                    ISZ(operand);
                    break;
                case MemoryReferenceOpCode.JMP:
                    JMP(operand);
                    break;
                case MemoryReferenceOpCode.JMS:
                    JMS(operand);
                    break;
                case MemoryReferenceOpCode.TAD:
                    TAD(operand);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void AND(uint operand)
        {
            Registers.LINK_AC.SetAccumulator(memory.Read(operand) & Registers.LINK_AC.Accumulator);
        }

        private void DCA(uint operand)
        {
            memory.Write(operand, Registers.LINK_AC.Accumulator);

            Registers.LINK_AC.SetAccumulator(0);
        }

        private void ISZ(uint operand)
        {
            memory.Write(operand, (memory.Read(operand) + 1) & Masks.MEM_WORD);

            if (memory.MB == 0)
            {
                Registers.IF_PC.Increment();
            }
        }

        private void JMP(uint operand)
        {
            Registers.IF_PC.Set(operand);
        }

        public void JMS(uint operand)
        {
            memory.Write(operand, Registers.IF_PC.Address);

            Registers.IF_PC.Set(operand + 1);
        }

        private void TAD(uint operand)
        {
            Registers.LINK_AC.Set(Registers.LINK_AC.Data + memory.Read(operand));
        }

        public override string ToString()
        {
            return $"{base.ToString()} ({Location.ToOctalString()})";
        }
    }

}
