namespace Core8.Interfaces
{
    public interface IHardware
    {
        void Tick();

        IMemory Memory { get; }

        IProcessor Processor { get; }

        IRegisters Registers { get; }

        IKeyboard Keyboard { get; }

        ITeleprinter Teleprinter { get; }
    }
}
