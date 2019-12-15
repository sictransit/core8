using Core8.Interfaces;

namespace Core8
{
    public class Hardware : IHardware
    {
        public Hardware(IProcessor processor, IMemory memory, IRegisters registers, IKeyboard keyboard, ITeleprinter teleprinter)
        {
            Processor = processor;
            Memory = memory;
            Registers = registers;
            Keyboard = keyboard;
            Teleprinter = teleprinter;
        }

        public IMemory Memory { get; }

        public IProcessor Processor { get; }

        public IRegisters Registers { get; }

        public IKeyboard Keyboard { get; }

        public ITeleprinter Teleprinter { get; }

        public void Tick()
        {
            Teleprinter.Tick();
            Keyboard.Tick();
        }
    }
}
