using Core8.Model.Interfaces;
using Core8.Model.Registers;
using System;
using System.Linq;

namespace Core8.Model.Instructions.Abstract
{
    public abstract class InstructionsBase : IInstruction
    {
        protected InstructionsBase(ICPU cpu)
        {
            CPU = cpu;
        }

        public int Data { get; private set; }

        public abstract void Execute();

        protected virtual string ExtendedAddress => "     ";

        protected abstract string OpCodeText { get; }

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

        public virtual IInstruction LoadData(int data)
        {
            Data = data;

            return this;
        }

        protected static string SplitOpCodes(Enum opCodes) => string.Join(' ', opCodes.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).OrderBy(s => s));

        public override string ToString()
        {
            return $"{ExtendedAddress}  {OpCodeText.Trim()}";
        }
    }
}
