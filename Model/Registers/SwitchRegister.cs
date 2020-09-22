using Core8.Model.Registers.Abstract;

namespace Core8.Model.Registers
{
    public class SwitchRegister : RegisterBase
    {
        public void SetSR(int data) => Content = data & Masks.MEM_WORD;

        protected override string ShortName => "SR";
    }
}
