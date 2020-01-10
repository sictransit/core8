using Core8.Model.Instructions;
using Core8.Model.Interfaces;

namespace Core8
{
    public class InstructionSet
    {
        public InstructionSet(IProcessor processor, IMemory memory, IRegisters registers, IKeyboard keyboard, ITeleprinter teleprinter)
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

            if (keyboard is null)
            {
                throw new System.ArgumentNullException(nameof(keyboard));
            }

            if (teleprinter is null)
            {
                throw new System.ArgumentNullException(nameof(teleprinter));
            }

            Group1 = new Group1Instructions(registers);
            Group2AND = new Group2ANDInstructions(processor, registers);
            Group2OR = new Group2ORInstructions(processor, registers);
            MemoryReference = new MemoryReferenceInstructions(memory, registers);
            Keyboard = new KeyboardInstructions(registers, keyboard);
            Teleprinter = new TeleprinterInstructions(registers, teleprinter);
        }

        public IInstruction Group1 { get; private set; }
        public IInstruction Group2AND { get; private set; }
        public IInstruction Group2OR { get; private set; }
        public IInstruction Keyboard { get; private set; }
        public IInstruction MemoryReference { get; private set; }
        public IInstruction Teleprinter { get; private set; }

    }
}
