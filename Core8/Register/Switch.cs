namespace Core8.Register
{
    public class Switch : RegisterBase
    {
        public void Set(uint data)
        {
            Data = data & Masks.MEM_WORD;
        }

        public uint Get => Data & Masks.MEM_WORD;
    }
}
