using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;

namespace Core8.Model.Instructions
{
    public class PrivilegedNoOperationInstruction : PrivilegedInstructionsBase
    {
        public PrivilegedNoOperationInstruction(IProcessor processor) : base(processor)
        {
        }

        protected override string OpCodeText => "NOP";

        protected override void PrivilegedExecute()
        {
        }
    }
}
