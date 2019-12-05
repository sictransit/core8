namespace Core8.Interfaces
{
    public interface IEnvironment
    {
        IMemory Memory { get; }

        IProcessor Processor { get; }

        IRegisters Registers { get; }

        IKeyboard Keyboard { get; }

        ITeleprinter Teleprinter { get; }

        void Tick();
    }
}
