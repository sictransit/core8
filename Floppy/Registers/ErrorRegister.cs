using Core8.Model.Registers.Abstract;

namespace Core8.Peripherals.Floppy.Registers
{
    internal class ErrorRegister : RegisterBase
    {
        protected override string ShortName => "ER";

        public override void Set(int value) => Content = value & 0b_000_011_111_111;
    }
}
