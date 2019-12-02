using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions.MemoryReference
{
    public class AND : MemoryReferenceInstruction
    {
        public AND(uint data) : base(data)
        {

        }

        protected override void ExecuteInternal(ICore core)
        {
            var memory = core.Memory.Read(GetAddress(core));

            var ac = core.Registers.LINK_AC.Accumulator;

            core.Registers.LINK_AC.SetAccumulator(memory & ac);
        }
    }
}
