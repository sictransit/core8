using Core8.Model.Interfaces;

namespace Core8.Model.Instructions.Abstract
{
    internal abstract class TeletypeInstructionsBase : PrivilegedInstructionsBase
    {
        protected TeletypeInstructionsBase(ICPU cpu) : base(cpu)
        {
        }

        protected ITeletype Teletype => CPU.Teletype;
    }
}
