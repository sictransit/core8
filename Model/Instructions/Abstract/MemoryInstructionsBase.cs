using Core8.Model.Interfaces;

namespace Core8.Model.Instructions.Abstract;

public abstract class MemoryInstructionsBase : InstructionsBase
{
    private int Address { get; set; }

    protected int Word => Data & 0b_000_001_111_111;

    protected int Page => Address & 0b_111_110_000_000;

    protected int Field => Address & 0b_111_000_000_000_000;

    protected IMemory Memory => CPU.Memory;

    protected MemoryInstructionsBase(ICPU cpu) : base(cpu)
    {

    }

    public IInstruction LoadAddress(int address)
    {
        Address = address;

        return this;
    }
}
