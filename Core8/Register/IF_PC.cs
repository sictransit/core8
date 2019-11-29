namespace Core8.Register
{
    public class IF_PC : RegisterBase
    {
        public ushort Address => (ushort)(Data & Masks.MEM_WORD);

        public ushort IF => (ushort)((Data & Masks.IF) >> 12);

        public ushort Page => (ushort)((Data & Masks.PAGE) >> 7);

        public ushort Word => (ushort)(Data & Masks.WORD);

        public void Increment()
        {
            Data = (ushort)(Data & Masks.IF | Data + 1 & Masks.MEM_WORD);
        }

        public void Reset()
        {
            Data = 0;
        }
    }
}
