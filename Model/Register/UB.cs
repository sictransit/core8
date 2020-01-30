using Core8.Model.Extensions;
using Core8.Model.Register.Abstract;

namespace Core8.Model.Register
{
    public class UB : RegisterBase
    {
        public void SetUB(uint value)
        {
            Set(value & Masks.UB);
        }

        public override string ToString()
        {
            return string.Format($"{base.ToString()} {Data.ToOctalString()}");
        }
    }
}
