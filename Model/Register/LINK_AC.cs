using Core8.Model.Extensions;
using Core8.Model.Register.Abstract;

namespace Core8.Model.Register
{
    public class LINK_AC : RegisterBase
    {
        public uint Link => (Data & Masks.LINK) >> 12;

        public uint Accumulator => Data & Masks.AC;

        public void ByteSwap()
        {
            SetAccumulator(((Data & Masks.AC_HIGH) >> 6) | ((Data & Masks.AC_LOW) << 6));
        }

        public void RAR()
        {
            Set(((Data >> 1) & Masks.AC) | ((Data << 12) & Masks.LINK));
        }

        public void RAL()
        {
            Set(((Data << 1) & Masks.AC_LINK) | ((Data >> 12) & Masks.FLAG));
        }

        public void ComplementLink()
        {
            SetLink((Data >> 12) ^ Masks.FLAG);
        }

        public void ComplementAccumulator()
        {
            SetAccumulator(Data ^ Masks.AC);
        }

        public void ClearAccumulator()
        {
            Set(Data & Masks.LINK);
        }

        public void ClearLink()
        {
            Set(Data & Masks.AC);
        }

        public void SetAccumulator(uint value)
        {
            Set((Data & Masks.LINK) | (value & Masks.AC));
        }

        public void ORAccumulator(uint value)
        {
            Set(Data | (value & Masks.AC));
        }

        public void SetLink(uint value)
        {
            Set(((value & Masks.FLAG) << 12) | (Data & Masks.AC));
        }

        public void AddWithCarry(uint value)
        {
            Set((Data + value) & Masks.AC_LINK);
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
