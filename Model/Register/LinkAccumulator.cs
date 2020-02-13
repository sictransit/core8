using Core8.Model.Extensions;
using Core8.Model.Register.Abstract;

namespace Core8.Model.Register
{
    public class LinkAccumulator : RegisterBase
    {
        public int Link => (Content & Masks.LINK) >> 12;

        public int Accumulator => Content & Masks.AC;

        protected override string ShortName => "LAC";

        public void ByteSwap()
        {
            SetAccumulator(((Content & Masks.AC_HIGH) >> 6) | ((Content & Masks.AC_LOW) << 6));
        }

        public void RAR()
        {
            Set(((Content >> 1) & Masks.AC) | ((Content << 12) & Masks.LINK));
        }

        public void RAL()
        {
            Set(((Content << 1) & Masks.AC_LINK) | ((Content >> 12) & Masks.FLAG));
        }

        public void ComplementLink()
        {
            SetLink((Content >> 12) ^ Masks.FLAG);
        }

        public void ComplementAccumulator()
        {
            SetAccumulator(Content ^ Masks.AC);
        }

        public void ClearAccumulator()
        {
            Set(Content & Masks.LINK);
        }

        public void ClearLink()
        {
            Set(Content & Masks.AC);
        }

        public void SetAccumulator(int value)
        {
            Set((Content & Masks.LINK) | (value & Masks.AC));
        }

        public void ORAccumulator(int value)
        {
            SetAccumulator(Content | value);
        }

        public void ANDAccumulator(int value)
        {
            SetAccumulator(Content & value);
        }

        public void SetLink(int value)
        {
            Set(((value & Masks.FLAG) << 12) | (Content & Masks.AC));
        }

        public void AddWithCarry(int value)
        {
            Set((Content + value) & Masks.AC_LINK);
        }

        public void IncrementWithCarry()
        {
            AddWithCarry(1);
        }

        public override string ToString()
        {
            return string.Format($"{base.ToString()} {Link} {Accumulator.ToOctalString()}");
        }
    }
}
