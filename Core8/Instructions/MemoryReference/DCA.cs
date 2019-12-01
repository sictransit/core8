using Core8.Enum;
using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions.MemoryReference
{
    public class DCA : MemoryReferenceInstruction
    {
        public DCA(uint data) : base(data)
        {

        }

        public override void Execute(ICore core)
        {
            core.Memory.Write(GetAddress(core.Registers), core.Registers.LINK_AC.Accumulator);

            core.Registers.LINK_AC.SetAccumulator(0);
        }
    }
}
