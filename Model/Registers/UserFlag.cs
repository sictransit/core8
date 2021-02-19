using Core8.Model.Registers.Abstract;

namespace Core8.Model.Registers
{
    public class UserFlag : RegisterBase
    {
        protected override string ShortName => "UF";

        protected override int Digits => 1;

        public void SetUF(int value) => Content = value & 0b_001;

        public bool ExecutiveMode => Content == 0;
    }
}
