using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions.Microcoded
{
    public class G2A : MicrocodedInstruction
    {
        public G2A(uint data) : base(1, (data & Masks.GROUP_2_AND_FLAGS))
        { }

        public G2A(bool cla, bool spa, bool sna, bool szl) : this((cla ? 1u : 0u) << 7 | (spa ? 1u : 0u) << 6 | (sna ? 1u : 0u) << 5 | (szl ? 1u : 0u) << 4 | Masks.GROUP_2_AND)
        {

        }

        public bool CLA => ((Data >> 7) & Masks.FLAG) == 1;

        public bool SPA => ((Data >> 6) & Masks.FLAG) == 1;

        public bool SNA => ((Data >> 5) & Masks.FLAG) == 1;

        public bool SZL => ((Data >> 4) & Masks.FLAG) == 1;

        public override void Execute(ICore core)
        {
            bool result = false;

            result |= SPA && ((core.Registers.LINK_AC.Accumulator & Masks.AC_SIGN) == 0);
            result |= SNA && (core.Registers.LINK_AC.Accumulator != 0);
            result |= SZL && (core.Registers.LINK_AC.Link == 0);

            if (result)
            {
                core.Registers.IF_PC.Increment();
            }

            if (CLA)
            {
                core.Registers.LINK_AC.Clear();
            }
        }
    }
}
