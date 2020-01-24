using Core8.Model.Extensions;

namespace Core8.Model.Register
{
    public class IB : RegisterBase
    {
        public uint DF => Data & Masks.IB_DF;

        public uint IF => (Data & Masks.IB_IF) >> 3;

        public void SetDF(uint value)
        {
            Data = (Data & Masks.IB_IF) | (value & Masks.IB_DF);
        }

        public void SetIF(uint value)
        {
            Data = ((value << 3) & Masks.IB_IF) | (Data & Masks.IB_DF);
        }

        public override string ToString()
        {
            return string.Format($"[IB] {Data.ToOctalString()}");
        }
    }
}
