namespace Core8.Model.Interfaces
{
    public interface IProcessor
    {
        void Run();

        void Halt();

        void Clear();

        void SetBreakpoint(uint address);

        void RemoveBreakpoint(uint address);

        void RemoveAllBreakpoints();

        IInstruction Debug8(uint address);

        IInstruction Debug10(uint address);

        void SingleStep(bool state);

        IInterrupts Interrupts { get; }

        IRegisters Registers { get; }

        IMemory Memory { get; }

        ITeleprinter Teleprinter { get; }
    }
}
