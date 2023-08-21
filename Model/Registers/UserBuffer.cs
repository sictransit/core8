using Core8.Model.Registers.Abstract;

namespace Core8.Model.Registers;

public class UserBuffer : RegisterBase
{
    protected override string ShortName => "UB";

    public override void Set(int value) => Content = value & 0b_001;

    protected override int Digits => 1;
}
