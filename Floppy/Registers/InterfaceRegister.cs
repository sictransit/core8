using Core8.Model.Register.Abstract;

namespace Core8.Peripherals.Floppy.Registers
{
    internal class InterfaceRegister : RegisterBase
    {
        public void SetIR(int value) => Content = value & 0b_111_111_111_111;

        protected override string ShortName => "IR";
    }
}
