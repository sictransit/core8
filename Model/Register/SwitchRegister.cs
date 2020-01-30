using Core8.Model.Extensions;
using Core8.Model.Register.Abstract;

namespace Core8.Model.Register
{
    public class SwitchRegister : RegisterBase
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
