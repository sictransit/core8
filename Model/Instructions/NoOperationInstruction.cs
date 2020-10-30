using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using Serilog;

namespace Core8.Model.Instructions
{
    internal class NoOperationInstruction : InstructionsBase
    {
        public NoOperationInstruction(ICPU cpu) : base(cpu)
        {
        }

        protected override string OpCodeText => "NOP";

        public override void Execute()
        {
            Log.Warning(this.ToString());
        }
    }
}
