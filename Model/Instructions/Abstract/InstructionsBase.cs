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

        public int Address { get; private set; }

        public int Data { get; private set; }

        public abstract void Execute();

        protected abstract string OpCodeText { get; }

        protected ICPU CPU { get; }

        protected IInterrupts Interrupts => CPU.Interrupts;

        protected LinkAccumulator AC => CPU.AC;

        protected InstructionFieldProgramCounter PC => CPU.PC;

        protected SwitchRegister SR => CPU.SR;

        protected MultiplierQuotient MQ => CPU.MQ;

        protected DataField DF => CPU.DF;

        protected InstructionBuffer IB => CPU.IB;

        protected UserBuffer UB => CPU.UB;

        protected UserFlag UF => CPU.UF;

        protected SaveField SF => CPU.SF;

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
