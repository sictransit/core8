namespace Core8.Register
{
    public class LINK_AC : RegisterBase
    {
        public ushort Link => (ushort)((Data & Masks.LINK) >> 12);

        public ushort Accumulator => (ushort)(Data & Masks.AC);

        public void Clear()
        {
            SetLink(0);
            SetAccumulator(0);
        }

        public void SetAccumulator(ushort value)
        {
            Data = (ushort)((Link << 12) | (value & Masks.AC));
        }

        public void SetLink(ushort value)
        {
            Data = (ushort)(((value & Masks.FLAG) << 12) | Accumulator);
        }

        public void Set(ushort value)
        { 
            Data = (ushort) (value & (Masks.LINK | Masks.AC));
        }
    }
}
