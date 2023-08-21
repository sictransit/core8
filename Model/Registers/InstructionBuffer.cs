using Core8.Model.Registers.Abstract;

namespace Core8.Model.Registers;

public class InstructionBuffer : RegisterBase
{
    protected override string ShortName => "IB";

    public override void Set(int value) => Content = value & 0b_111;

    protected override int Digits => 1;
}
