using Core8.Extensions;
using Core8.Model.Interfaces;
using Core8.Model.Registers;

namespace Core8.Model.Instructions.Abstract
{
    internal abstract class InstructionsBase : IInstruction
    {
        protected InstructionsBase(ICPU cpu)
        {
            CPU = cpu;
        }

        public int Address { get; private set; }

        public int Data { get; private set; }

        public abstract void Execute();

        protected abstract string OpCodeText { get; }

        protected int Word => Data & Masks.ADDRESS_WORD;

        protected int Page => Address & Masks.ADDRESS_PAGE;

        protected int Field => Address & Masks.IF;

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
            return $"{Address.ToOctalString(5)}:{Data.ToOctalString()} {OpCodeText}";
        }
    }
}
