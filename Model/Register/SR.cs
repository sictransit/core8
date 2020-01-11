using Core8.Model.Extensions;

namespace Core8.Model.Register
{
    public class SR : RegisterBase
    {
        public void Set(uint data)
        {
            Data = data & Masks.MEM_WORD;
        }

        public uint Get => Data & Masks.MEM_WORD;

        public override string ToString()
        {
            return string.Format($"[SR] {Data.ToOctalString()}");
        }
    }
}
