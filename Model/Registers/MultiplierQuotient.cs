using Core8.Model.Registers.Abstract;

namespace Core8.Model.Registers;

public class MultiplierQuotient : RegisterBase
{
    protected override string ShortName => "MQ";

    public override void Set(int value) => Content = value & 0b_111_111_111_111;
}
