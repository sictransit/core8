namespace Core8.Model.Interfaces
{
    public interface IProcessor
    {
        void Run();

        void Halt();

        void Clear();

        void EnableInterrupts();

        void DisableInterrupts();

        void PauseInterrupts();

        void ResumeInterrupts();

        bool InterruptsEnabled { get; }

        bool InterruptPending { get; }

        bool InterruptRequested { get; }

        bool DeviceInterruptRequested { get; }

        bool UserInterruptRequested { get; }        

        bool InterruptsPaused { get; }

        IInstruction Debug8(uint field, uint address);

        IInstruction Debug10(uint field, uint address);

        void SingleStep(bool state);
    }
}
