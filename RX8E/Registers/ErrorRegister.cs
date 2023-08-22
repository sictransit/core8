using Core8.Model.Registers.Abstract;

namespace Core8.Peripherals.RX8E.Registers;

internal class ErrorRegister : RegisterBase
{
    protected override string ShortName => "ER";

    public override void Set(int value) => Content = value & 0b_000_011_111_111;
}
