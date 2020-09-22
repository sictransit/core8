using Core8.Model.Registers.Abstract;

namespace Core8.Model.Registers
{
    public class UserBuffer : RegisterBase
    {
        protected override string ShortName => "UB";

        protected override int Digits => 1;

        public void SetUB(int value) => Content = value & Masks.UB;
    }
}
