using Core8.Model.Interfaces;

namespace Core8.Model.Instructions
{
    public class InstructionFactory
    {
       

        public InstructionFactory(IProcessor processor, IMemory memory, IRegisters registers, ITeleprinter teleprinter)
        {
            if (processor is null)
            {
                throw new System.ArgumentNullException(nameof(processor));
            }

            if (memory is null)
            {
                throw new System.ArgumentNullException(nameof(memory));
            }

            if (registers is null)
            {
                throw new System.ArgumentNullException(nameof(registers));
            }

            if (teleprinter is null)
            {
                throw new System.ArgumentNullException(nameof(teleprinter));
            }

           
        }

    }
}
