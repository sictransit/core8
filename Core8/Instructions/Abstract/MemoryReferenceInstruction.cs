using Core8.Enums;
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

        public override string ToString()
        {
            var mode = AddressingMode != 0 ? AddressingMode.ToString() : string.Empty;

            return $"{Data.ToOctalString()} {OpCode} {mode} {(Data & Masks.ADDRESS_WORD).ToOctal().ToString()}";
        }

        protected uint GetAddress(ICore core)
        {
            if (core is null)
            {
                throw new System.ArgumentNullException(nameof(core));
            }

            var location = AddressingMode.HasFlag(AddressingModes.Z) ? (core.Registers.IF_PC.Page << 7) | (Data & Masks.ADDRESS_WORD) : Data & Masks.ADDRESS_WORD;

            return AddressingMode.HasFlag(AddressingModes.I) ? core.Memory.Read(location) : location;
        }
    }
}
