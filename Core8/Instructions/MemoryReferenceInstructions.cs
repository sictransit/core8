using Core8.Enums;
using Core8.Extensions;
using Core8.Interfaces;

namespace Core8.Instructions.Abstract
{
    public abstract class MemoryReferenceInstructions : InstructionBase
    {
        private readonly uint data;

        protected MemoryReferenceInstructions(uint data) 
        {
            this.data = data;
        }

        public InstructionName OpCode => (InstructionName)(Data & Masks.OP_CODE);

        public AddressingModes AddressingMode => (AddressingModes)(Data & Masks.ADDRESSING_MODE);

        public override string ToString()
        {
            var mode = AddressingMode != 0 ? AddressingMode.ToString() : string.Empty;

            return $"{Data.ToOctalString()} {OpCode} {mode} {(Data & Masks.ADDRESS_WORD).ToOctal().ToString()}";
        }

        protected uint GetAddress(IEnvironment environment)
        {
            if (environment is null)
            {
                throw new System.ArgumentNullException(nameof(environment));
            }

            var location = AddressingMode.HasFlag(AddressingModes.Z) ? (environment.Processor.CurrentAddress & Masks.ADDRESS_PAGE) | (Data & Masks.ADDRESS_WORD) : Data & Masks.ADDRESS_WORD;

            return AddressingMode.HasFlag(AddressingModes.I) ? environment.Memory.Read(location) : location;
        }
    }
}
