using Core8.Model.Extensions;

namespace Core8.Model.Register
{
    public class IB : RegisterBase
    {
        public void SetIB(uint value)
        {
            Set(value & Masks.IB);
        }

        public override string ToString()
        {
            return string.Format($"{base.ToString()} {Data.ToOctalString()}");
        }
    }
}
