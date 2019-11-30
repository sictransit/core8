using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8.Instructions.Microcoded
{
    public class G2O : MicrocodedInstruction
    {
        public G2O(uint data) : base(1, (data & Masks.GROUP_2_OR_FLAGS))
        { }

        public G2O(bool cla, bool sma, bool sza, bool snl) : this((cla ? 1u : 0u) << 7 | (sma ? 1u : 0u) << 6 | (sza ? 1u : 0u) << 5 | (snl ? 1u : 0u) << 4)
        {

        }

        public bool CLA => ((Data >> 7) & Masks.FLAG) == 1;

        public bool SMA => ((Data >> 6) & Masks.FLAG) == 1;

        public bool SZA => ((Data >> 5) & Masks.FLAG) == 1;

        public bool SNL => ((Data >> 4) & Masks.FLAG) == 1;

        public override void Execute(ICore core)
        {
            bool result = false;

            result |= SMA && ((core.Registers.LINK_AC.Accumulator & Masks.AC_SIGN) != 0);
            result |= SZA && (core.Registers.LINK_AC.Accumulator == 0);
            result |= SNL && (core.Registers.LINK_AC.Link != 0);

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
