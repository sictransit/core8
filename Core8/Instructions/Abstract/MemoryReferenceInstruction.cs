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

        public InstructionName OpCode => (InstructionName)(Data & Masks.OP_CODE);

        public AddressingModes AddressingMode => (AddressingModes)(Data & Masks.ADDRESSING_MODE);

        public uint Address => Data & Masks.ADDRESS_WORD;

        public override string ToString()
        {
            var mode = AddressingMode != 0 ? AddressingMode.ToString() : string.Empty;

            return $"{Data.ToOctalString()} {OpCode} {mode} {Address.ToOctal().ToString()}";
        }

        protected uint GetAddress(ICore core)
        {
            var location = AddressingMode.HasFlag(AddressingModes.Z) ? (core.Registers.IF_PC.Page << 7) | (Address & Masks.ADDRESS_WORD) : Address & Masks.ADDRESS_WORD;

            return AddressingMode.HasFlag(AddressingModes.I) ? core.Memory.Read(location) : location;
        }
    }
}
