namespace Core8.Register
{
    public class LINK_AC : RegisterBase
    {
        public uint Link => (Data & Masks.LINK) >> 12;

        public uint Accumulator => Data & Masks.AC;

        public void Clear()
        {
            SetLink(0);
            SetAccumulator(0);
        }

        public void SetAccumulator(uint value)
        {
            Data = ((Link << 12) | (value & Masks.AC));
        }

        public void SetLink(uint value)
        {
            Data = (((value & Masks.FLAG) << 12) | Accumulator);
        }

        public void Set(uint value)
        { 
            Data =  (value & (Masks.LINK | Masks.AC));
        }
    }
}
