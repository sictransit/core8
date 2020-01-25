using Core8.Model.Extensions;

namespace Core8.Model.Register
{
    public class SR : RegisterBase
    {
        public void SetSR(uint data)
        {
            Set(data & Masks.MEM_WORD);
        }

        public uint Get => Data & Masks.MEM_WORD;

        public override string ToString()
        {
            return string.Format($"{base.ToString()} {Data.ToOctalString()}");
        }
    }
}
