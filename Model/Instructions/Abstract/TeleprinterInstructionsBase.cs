using Core8.Model.Interfaces;

namespace Core8.Model.Instructions.Abstract
{
    public abstract class TeleprinterInstructionsBase : PrivilegedInstructionsBase
    {
        protected TeleprinterInstructionsBase(IProcessor processor) : base(processor)
        {
        }

        protected ITeleprinter Teleprinter => Processor.Teleprinter;
    }
}
