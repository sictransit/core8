using Core8.Model.Extensions;
using Core8.Model.Register.Abstract;

namespace Core8.Model.Register
{
    public class UserBuffer : RegisterBase
    {
        protected override string ShortName => "UB";

        public void SetUB(int value)
        {
            Content = value & Masks.UB;
        }

        public override string ToString()
        {
            return string.Format($"{base.ToString()} {Content.ToOctalString(1)}");
        }
    }
}
