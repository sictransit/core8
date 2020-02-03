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

        void EnableInterrupts(bool delay = true);

        void DisableInterrupts();

        void InhibitInterrupts();

        void ResumeInterrupts();

        void ClearUserInterrupt();

        bool InterruptsEnabled { get; }

        bool InterruptPending { get; }

        bool InterruptRequested { get; }

        bool UserInterruptRequested { get; }

        bool InterruptsInhibited { get; }

        IInstruction Debug8(uint field, uint address);

        IInstruction Debug10(uint field, uint address);

        void SingleStep(bool state);
    }
}
