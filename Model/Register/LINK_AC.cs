using Core8.Model.Extensions;

namespace Core8.Model.Register
{
    public class LINK_AC : RegisterBase
    {
        public uint Link => (Data & Masks.LINK) >> 12;

        public uint Accumulator => Data & Masks.AC;

        public void RAR()
        {
            Data = ((Data >> 1) & Masks.AC) | ((Data << 12) & Masks.LINK);
        }

        public void RAL()
        {
            Data = ((Data << 1) & Masks.AC_LINK) | ((Data >> 12) & Masks.FLAG);
        }

        public void ComplementLink()
        {
            SetLink((Data >> 12) ^ Masks.FLAG);
        }

        public void ComplementAccumulator()
        {
            SetAccumulator(Data ^ Masks.AC);
        }

        public void SetAccumulator(uint value)
        {
            Data = (Data & Masks.LINK) | (value & Masks.AC);
        }

        public void SetLink(uint value)
        {
            Data = ((value & Masks.FLAG) << 12) | (Data & Masks.AC);
        }

        public void AddWithCarry(uint value)
        {
            Data = (Data + value) & Masks.AC_LINK;
        }

        public void IncrementWithCarry()
        {
            Data = (Data + 1) & Masks.AC_LINK;
        }

        public override string ToString()
        {
            return string.Format($"[LINK_AC] {Link} {Accumulator.ToOctalString()}");
        }
    }
}
