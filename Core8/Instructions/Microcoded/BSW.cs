using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions.Microcoded
{
    public class BSW : MicrocodedInstruction
    {
        public BSW() : base(0, Masks.GROUP_1_BSW)
        {

        }

        public override void Execute(ICore core)
        {
            var acc = core.Registers.LINK_AC.Accumulator;

            var result = (acc & Masks.AC_HIGH) >> 6 | (acc & Masks.AC_LOW) << 6;

            core.Registers.LINK_AC.SetAccumulator(result);
        }
    }
}
