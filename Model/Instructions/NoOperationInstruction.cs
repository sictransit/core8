using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;

namespace Core8.Model.Instructions
{
    public class NoOperationInstruction : InstructionsBase
    {
        public NoOperationInstruction(IRegisters registers) : base(registers)
        {
        }

        protected override string OpCodeText => "NOP";

        public override void Execute()
        {

        }
    }
}
