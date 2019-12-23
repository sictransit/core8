namespace Core8.Model.Interfaces
{
    public interface IHardware
    {
        void Tick();

        bool InterruptRequested { get; }

        IMemory Memory { get; }

        IProcessor Processor { get; }

        IRegisters Registers { get; }

        IKeyboard Keyboard { get; }

        ITeleprinter Teleprinter { get; }
    }
}
