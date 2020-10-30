using Core8.Model.Interfaces;

namespace Core8.Model.Instructions.Abstract
{
    internal abstract class PrivilegedInstructionsBase : InstructionsBase
    {
        protected PrivilegedInstructionsBase(ICPU cpu) : base(cpu)
        {
        }

        protected abstract void PrivilegedExecute();

        public override void Execute()
        {
            if (UF.Content == 0)  // Executive mode
            {
                PrivilegedExecute();
            }
            else // User mode
            {
                Interrupts.SetUser();
            }
        }
    }
}
