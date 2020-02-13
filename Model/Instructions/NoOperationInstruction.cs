using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;

namespace Core8.Model.Instructions
{
    public class NoOperationInstruction : InstructionsBase
    {
        public NoOperationInstruction(ICPU cpu) : base(cpu)
        {
        }

        protected override string OpCodeText => "NOP";

        public override void Execute()
        {
        }
    }
}
