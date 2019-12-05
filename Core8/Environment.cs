using Core8.Interfaces;

namespace Core8
{
    public class Environment : IEnvironment
    {
        public Environment(IProcessor processor, IMemory memory, IRegisters registers, IKeyboard keyboard, ITeleprinter teleprinter)
        {
            Memory = memory;
            Processor = processor;
            Registers = registers;
            Keyboard = keyboard;
            Teleprinter = teleprinter;
        }

        public IMemory Memory { get; }

        public IProcessor Processor { get; }

        public IRegisters Registers { get; }

        public IKeyboard Keyboard { get; }

        public ITeleprinter Teleprinter { get; }
    }
}
