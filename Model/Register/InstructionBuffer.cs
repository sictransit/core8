using Core8.Model.Register.Abstract;

namespace Core8.Model.Register
{
    public class InstructionBuffer : RegisterBase
    {
        protected override string ShortName => "IB";

        protected override int Digits => 1;

        public void SetIB(int value) => Content = value & Masks.IB;
    }
}
