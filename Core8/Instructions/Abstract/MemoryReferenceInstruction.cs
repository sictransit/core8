using Core8.Enum;
using Core8.Extensions;

namespace Core8.Instructions.Abstract
{
    public abstract class MemoryReferenceInstruction : InstructionBase
    {
        protected MemoryReferenceInstruction(uint opCode, uint address) : base(opCode & Masks.OP_CODE | address & Masks.ADDRESS_WORD)
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

    }

}
