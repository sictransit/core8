using Core8.Model.Interfaces;

namespace Core8.Model.Instructions.Abstract
{
    public abstract class TeletypeInstructionsBase : PrivilegedInstructionsBase
    {
        protected TeletypeInstructionsBase(ICPU cpu) : base(cpu)
        {
        }

        protected ITeletype Teletype => CPU.Teletype;
    }
}
