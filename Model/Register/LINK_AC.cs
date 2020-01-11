using Core8.Model.Extensions;
using Serilog;

namespace Core8.Model.Register
{
    public class LINK_AC : RegisterBase
    {
        public uint Link => (Data & Masks.LINK) >> 12;

        public uint Accumulator => Data & Masks.AC;

        public void RAR()
        {
            Set(((Data >> 1) & Masks.AC) + ((Data & Masks.FLAG) << 12));
        }

        public void RAL()
        {
            Set(((Data << 1) & Masks.AC_LINK) + ((Data & Masks.LINK) >> 12));
        }

        public void SetAccumulator(uint value)
        {
            Set((Link << 12) | (value & Masks.AC));
        }

        public void SetLink(uint value)
        {
            Set(((value & Masks.FLAG) << 12) | Accumulator);
        }

        public void Set(uint value)
        {
            Data = value & Masks.AC_LINK;

            Log.Debug(this.ToString());
        }

        public override string ToString()
        {
            return string.Format($"[LINK_AC] {Link} {Accumulator.ToOctalString()}");
        }
    }
}
