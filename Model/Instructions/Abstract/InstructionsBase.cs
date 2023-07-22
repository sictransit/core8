using Core8.Extensions;
using Core8.Model.Interfaces;
using Core8.Model.Registers;

namespace Core8.Model.Instructions.Abstract
{
    public abstract class InstructionsBase : IInstruction
    {
        protected InstructionsBase(ICPU cpu)
        {
            CPU = cpu;
        }

        private int Address { get; set; }

        protected int Data { get; private set; }

        public abstract void Execute();

        protected abstract string OpCodeText { get; }

        protected int Word => Data & 0b_000_001_111_111;

        protected int Page => Address & 0b_111_110_000_000;

        protected int Field => Address & 0b_111_000_000_000_000;

        protected ICPU CPU { get; }

        protected IInterrupts Interrupts => CPU.Interrupts;

        private IRegistry Registry => CPU.Registry;

        protected LinkAccumulator AC => Registry.AC;

        protected InstructionFieldProgramCounter PC => Registry.PC;

        protected SwitchRegister SR => Registry.SR;

        protected MultiplierQuotient MQ => Registry.MQ;

        protected DataField DF => Registry.DF;

        protected InstructionBuffer IB => Registry.IB;

        protected UserBuffer UB => Registry.UB;

        protected UserFlag UF => Registry.UF;

        protected SaveField SF => Registry.SF;

        public IInstruction Load(int address, int data)
        {
            Address = address;
            Data = data;

            return this;
        }

        public override string ToString()
        {
            return $"{Data.ToOctalString()} {OpCodeText}";
        }
    }
}
