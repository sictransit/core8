using Core8.Model.Extensions;
using Core8.Model.Register.Abstract;

namespace Core8.Model.Register
{
    public class UserFlag : RegisterBase
    {
        protected override string ShortName => "UF";

        public void SetUF(uint value)
        {
            Set(value & Masks.UF);
        }

        public override string ToString()
        {
            return string.Format($"{base.ToString()} {Data.ToOctalString()}");
        }
    }
}
