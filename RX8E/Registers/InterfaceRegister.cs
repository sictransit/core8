using Core8.Model.Registers.Abstract;

namespace Core8.Peripherals.RX8E.Registers
{
    internal class InterfaceRegister : RegisterBase
    {
        protected override string ShortName => "IR";

        public override void Set(int value) => Content = value & 0b_111_111_111_111;
    }
}
