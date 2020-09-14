using Core8.Model.Register.Abstract;

namespace Core8.Floppy.Registers
{
    internal class ErrorCodeRegister : RegisterBase
    {
        protected override string ShortName => "EC";

        public void SetEC(int value) => Content = value & 0b_000_011_111_111;
    }
}
