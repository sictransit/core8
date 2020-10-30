using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using Serilog;

namespace Core8.Model.Instructions
{
    internal class PrivilegedNoOperationInstruction : PrivilegedInstructionsBase
    {
        public PrivilegedNoOperationInstruction(ICPU cpu) : base(cpu)
        {
        }

        protected override string OpCodeText => "NOP";

        protected override void PrivilegedExecute()
        {
            Log.Warning(this.ToString());
        }
    }
}
