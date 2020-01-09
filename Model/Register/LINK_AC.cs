using Core8.Model.Extensions;

namespace Core8.Model.Register
{
    public class LINK_AC : RegisterBase
    {
        public uint Link => (Data & Masks.LINK) >> 12;

        public uint Accumulator => Data & Masks.AC;


        public void SetAccumulator(uint value)
        {
            SetRegister((Link << 12) | (value & Masks.AC));
        }

        public void SetLink(uint value)
        {
            SetRegister(((value & Masks.FLAG) << 12) | Accumulator);
        }

        public void Set(uint value)
        {
            SetRegister(value & (Masks.LINK | Masks.AC));
        }

        public override string ToString()
        {
            return string.Format($"[LINK_AC] {Link} {Accumulator.ToOctalString()}");
        }
    }
}
