using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions.Microcoded
{
    public class CLA : MicrocodedInstruction
    {
        public CLA() : base(0, Masks.GROUP_1_CLA)
        {

        }

        public override void Execute(ICore core)
        {
            core.Registers.LINK_AC.SetAccumulator(0);
        }
    }
}
