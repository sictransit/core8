using Core8.Model.Register.Abstract;

namespace Core8.Model.Register
{
    public class UserBuffer : RegisterBase
    {
        protected override string ShortName => "UB";

        protected override int Digits => 1;

        public void SetUB(int value) => Content = value & Masks.UB;
    }
}
