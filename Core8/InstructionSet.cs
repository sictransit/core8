using Core8.Instructions;
using Core8.Instructions.Abstract;
using Core8.Interfaces;

namespace Core8
{
    public class InstructionSet
    {
        public InstructionSet(IProcessor processor, IMemory memory, IRegisters registers, IKeyboard keyboard, ITeleprinter teleprinter)
        {
            Teleprinter = new Instructions.Teleprinter(registers, teleprinter);
            Keyboard = new Instructions.Keyboard(registers, keyboard);
            Group1 = new Group1(registers);
            Group2AND = new Group2AND(registers);
            Group2OR = new Group2OR(registers);
            Group2Priv = new Group2Priv(registers, processor);
            MemRef = new MemRef(registers, processor, memory);
        }

        public Instructions.Teleprinter Teleprinter { get; }

        public Instructions.Keyboard Keyboard { get; }

        public Group1 Group1 { get; }

        public Group2AND Group2AND { get; }

        public Group2OR Group2OR { get; }

        public Group2Priv Group2Priv { get; }

        public MemRef MemRef { get; }

    }
}
