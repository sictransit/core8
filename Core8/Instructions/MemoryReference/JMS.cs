using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions.MemoryReference
{
    public class JMS : MemoryReferenceInstruction
    {
        public JMS(uint data) : base(data)
        {

        }

        protected override void ExecuteInternal(ICore core)
        {
            if (core is null)
            {
                throw new System.ArgumentNullException(nameof(core));
            }

            core.Memory.Write(GetAddress(core), core.Registers.IF_PC.Address);

            core.Registers.IF_PC.Set(GetAddress(core));

        }
    }
}
