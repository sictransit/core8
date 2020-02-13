using Core8.Model.Interfaces;

namespace Core8.Model.Instructions.Abstract
{
    public abstract class TeleptypeInstructionsBase : PrivilegedInstructionsBase
    {
        protected TeleptypeInstructionsBase(ICPU cpu) : base(cpu)
        {
        }

        protected ITeletype Teletype => CPU.Teletype;
    }
}
