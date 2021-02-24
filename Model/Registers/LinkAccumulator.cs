using Core8.Extensions;
using Core8.Model.Registers.Abstract;

namespace Core8.Model.Registers
{
    public class LinkAccumulator : RegisterBase
    {
        public int Link => (Content & 0b_1_000_000_000_000) >> 12;

        public int Accumulator => Content & 0b_0_111_111_111_111;

        protected override string ShortName => "LAC";

        public void ByteSwap() => Content = (Content & 0b_1_000_000_000_000) | ((Content & 0b_0_111_111_000_000) >> 6) | ((Content & 0b_0_000_000_111_111) << 6);

        public void RAR()
        {
            Content = ((Content >> 1) & 0b_0_111_111_111_111) | ((Content << 12) & 0b_1_000_000_000_000);
        }

        public void RAL()
        {
            Content = ((Content << 1) & 0b_1_111_111_111_111) | ((Content >> 12) & 0b_001);
        }

        public void ComplementLink() => Content ^= 0b_1_000_000_000_000;

        public void ComplementAccumulator() => Content ^= 0b_0_111_111_111_111;

        public void ClearAccumulator() => Content &= 0b_1_000_000_000_000;

        public void ClearLink() => Content &= 0b_0_111_111_111_111;

        public void SetAccumulator(int value) => Content = (Content & 0b_1_000_000_000_000) | (value & 0b_0_111_111_111_111);

        public void ORAccumulator(int value) => Content |= value & 0b_0_111_111_111_111;

        public void ANDAccumulator(int value) => Content &= 0b_1_000_000_000_000 | (Content & value);

        public void SetLink(int value) => Content = ((value & 0b_001) << 12) | (Content & 0b_0_111_111_111_111);

        public void AddWithCarry(int value) => Content = (Content + value) & 0b_1_111_111_111_111;

        public override void Set(int value) => Content = value & 0b_1_111_111_111_111;

        public override string ToString() => $"[{ShortName}] {Link} {Accumulator.ToOctalString()}";
    }
}
