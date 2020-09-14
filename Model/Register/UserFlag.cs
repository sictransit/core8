using Core8.Model.Register.Abstract;

namespace Core8.Model.Register
{
    public class UserFlag : RegisterBase
    {
        protected override string ShortName => "UF";

        protected override int Digits => 1;

        public void SetUF(int value) => Content = value & Masks.UF;
    }
}
