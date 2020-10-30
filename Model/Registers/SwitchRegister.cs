using Core8.Model.Registers.Abstract;

namespace Core8.Model.Registers
{
    public class SwitchRegister : RegisterBase
    {
        public void SetSR(int data) => Content = data & 0b_111_111_111_111;

        protected override string ShortName => "SR";
    }
}
