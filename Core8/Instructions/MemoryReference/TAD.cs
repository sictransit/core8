using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions.MemoryReference
{
    public class TAD : MemoryReferenceInstruction
    {
        public TAD(uint data) : base(data)
        {

        }

        protected override void ExecuteInternal(ICore core)
        {
            if (core is null)
            {
                throw new System.ArgumentNullException(nameof(core));
            }

            var memory = core.Memory.Read(GetAddress(core));

            var ac = core.Registers.LINK_AC.Data;

            core.Registers.LINK_AC.Set(ac + memory);
        }
    }
}
