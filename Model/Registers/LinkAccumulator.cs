using Core8.Extensions;
using Core8.Model.Registers.Abstract;

namespace Core8.Model.Registers
{
    public class LinkAccumulator : RegisterBase
    {
        public int Link => (Content & Masks.LINK) >> 12;

        public int Accumulator => Content & Masks.AC;

        protected override string ShortName => "LAC";

        public void ByteSwap() => Content = (Content & Masks.LINK) | ((Content & Masks.AC_HIGH) >> 6) | ((Content & Masks.AC_LOW) << 6);

        public void RAR(bool twice = false)
        {
            Content = ((Content >> 1) & Masks.AC) | ((Content << 12) & Masks.LINK);

            if (twice)
            {
                RAR();
            }
        }

        public void RAL(bool twice = false)
        {
            Content = ((Content << 1) & Masks.AC_LINK) | ((Content >> 12) & Masks.FLAG);

            if (twice)
            {
                RAL();
            }
        }

        public void ComplementLink() => Content ^= Masks.LINK;

        public void ComplementAccumulator() => Content ^= Masks.AC;

        public void ClearAccumulator() => Content &= Masks.LINK;

        public void ClearLink() => Content &= Masks.AC;

        public void SetAccumulator(int value) => Content = (Content & Masks.LINK) | (value & Masks.AC);

        public void ORAccumulator(int value) => Content |= value & Masks.AC;

        public void ANDAccumulator(int value) => Content &= Masks.LINK | (Content & value);

        public void SetLink(int value) => Content = ((value & Masks.FLAG) << 12) | (Content & Masks.AC);

        public void AddWithCarry(int value) => Content = (Content + value) & Masks.AC_LINK;

        public override string ToString() => $"[{ShortName}] {Link} {Accumulator.ToOctalString()}";
    }
}
