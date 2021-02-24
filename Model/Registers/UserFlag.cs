using Core8.Model.Registers.Abstract;

namespace Core8.Model.Registers
{
    public class UserFlag : RegisterBase
    {
        protected override string ShortName => "UF";

        public override void Set(int value) => Content = value & 0b_001;

        protected override int Digits => 1;

        public bool ExecutiveMode => Content == 0;
    }
}
