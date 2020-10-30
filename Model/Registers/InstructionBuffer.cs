using Core8.Model.Registers.Abstract;

namespace Core8.Model.Registers
{
    public class InstructionBuffer : RegisterBase
    {
        protected override string ShortName => "IB";

        protected override int Digits => 1;

        public void SetIB(int value) => Content = value & 0b_111;
    }
}
