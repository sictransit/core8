using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions.MemoryReference
{
    public class JMP : MemoryReferenceInstruction
    {
        public JMP(uint data) : base(data)
        {

        }

        protected override void ExecuteInternal(ICore core)
        {
            if (core is null)
            {
                throw new System.ArgumentNullException(nameof(core));
            }

            var address = GetAddress(core);

            core.Registers.IF_PC.Set(address);
        }
    }
}
