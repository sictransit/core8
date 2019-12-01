using Core8.Enum;
using Core8.Extensions;
using Core8.Interfaces;

namespace Core8.Instructions.Abstract
{
    public abstract class MemoryReferenceInstruction : InstructionBase
    {
        protected MemoryReferenceInstruction(uint data) : base(data)
        {

        }

        public InstructionName OpCode => (InstructionName) (Data & Masks.OP_CODE);

        public AddressingModes AddressingMode => (AddressingModes)(Data & Masks.ADDRESSING_MODE);

        public uint Address => Data & Masks.ADDRESS_WORD;

        public override string ToString()
        {
            var mode = AddressingMode != 0 ? AddressingMode.ToString() : string.Empty;

            return $"{Data.ToOctal().ToString("d4")} {OpCode} {mode} {Address.ToOctal().ToString()}";
        }

        protected uint GetAddress(IRegisters registers)
        {
            if (AddressingMode.HasFlag(AddressingModes.Z))
            {
                return (registers.IF_PC.Page << 7) | (Address & Masks.ADDRESS_WORD);
            }

            return Address & Masks.ADDRESS_WORD;
        }


    }

}
