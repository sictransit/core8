using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions.Microcoded
{
    public class IAC : MicrocodedInstruction
    {
        public IAC() : base(0, Masks.GROUP_1_IAC)
        {

        }
        public override void Execute(ICore core)
        {
            var acc = core.Registers.LINK_AC.Data;

            core.Registers.LINK_AC.Set(acc + 1);
        }
    }
}
