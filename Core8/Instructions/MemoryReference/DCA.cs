using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions.MemoryReference
{
    public class DCA : MemoryReferenceInstruction
    {
        public DCA(uint data) : base(data)
        {

        }

        protected override void ExecuteInternal(ICore core)
        {
            if (core is null)
            {
                throw new System.ArgumentNullException(nameof(core));
            }

            core.Memory.Write(GetAddress(core), core.Registers.LINK_AC.Accumulator);

            core.Registers.LINK_AC.SetAccumulator(0);
        }
    }
}
