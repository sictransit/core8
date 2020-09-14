using Core8.Model.Register.Abstract;

namespace Core8.Model.Register
{
    public class SwitchRegister : RegisterBase
    {
        public void SetSR(int data) => Content = data & Masks.MEM_WORD;

        protected override string ShortName => "SR";
    }
}
