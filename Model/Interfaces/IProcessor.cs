namespace Core8.Model.Interfaces
{
    public interface IProcessor
    {
        void Run();

        void Halt();

        void Clear();

        void EnableInterrupts();

        bool InterruptsEnabled { get; }

        bool InterruptsPending { get; }

        bool InterruptRequested { get; }

        void DisableInterrupts();

        IInstruction Debug8(uint address);

        IInstruction Debug10(uint address);
    }
}
