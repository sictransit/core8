using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;

namespace Core8.Model.Instructions
{
    public class PrivilegedNoOperationInstruction : PrivilegedInstructionsBase
    {
        public PrivilegedNoOperationInstruction(ICPU cpu) : base(cpu)
        {
        }

        protected override string OpCodeText => "NOP";

        protected override void PrivilegedExecute()
        {
        }
    }
}
