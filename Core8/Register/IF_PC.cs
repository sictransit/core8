namespace Core8.Register
{
    public class IF_PC : RegisterBase
    {
        public uint Address => (Data & Masks.MEM_WORD);

        public uint IF => ((Data & Masks.IF) >> 12);

        public uint Page => ((Data & Masks.PAGE) >> 7);

        public uint Word => (Data & Masks.WORD);

        public void Increment()
        {
            Data = (Data & Masks.IF | Data + 1 & Masks.MEM_WORD);
        }

        public void Reset()
        {
            Data = 0;
        }

        public void Jump(uint address)
        {
            Data = (Data & Masks.IF) | (address & Masks.MEM_WORD);
        }
    }
}
