using Core8.Model.Registers.Abstract;

namespace Core8.Model.Registers;

public class SwitchRegister : RegisterBase
{
    protected override string ShortName => "SR";

    public override void Set(int value) => Content = value & 0b_111_111_111_111;
}
