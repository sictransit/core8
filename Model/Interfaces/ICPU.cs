namespace Core8.Model.Interfaces
{
    public interface ICPU
    {
        void Run();

        void Halt();

        void Clear();

        void SetBreakpoint(int address);

        void RemoveBreakpoint(int address);

        void RemoveAllBreakpoints();

        IInstruction Debug8(int address);

        IInstruction Debug10(int address);

        void SingleStep(bool state);

        void Debug(bool state);

        IInterrupts Interrupts { get; }

        IRegisters Registers { get; }

        IMemory Memory { get; }

        ITeletype Teletype { get; }
    }
}
