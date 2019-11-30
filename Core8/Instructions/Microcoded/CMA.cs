using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions.Microcoded
{
    public class CMA : MicrocodedInstruction
    {
        public CMA() : base(0, Masks.GROUP_1_CMA)
        {

        }
        public override void Execute(ICore core)
        {
            var acc = core.Registers.LINK_AC.Accumulator;

            core.Registers.LINK_AC.SetAccumulator(acc % Masks.AC);
        }
    }
}
