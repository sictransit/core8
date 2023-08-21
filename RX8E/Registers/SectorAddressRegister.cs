using Core8.Model.Registers.Abstract;

namespace Core8.Peripherals.RX8E.Registers;

internal class SectorAddressRegister : RegisterBase
{
    protected override string ShortName => "SA";

    public override void Set(int value) => Content = value & 0b_000_001_111_111;
}
