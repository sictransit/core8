namespace Core8.Model.Interfaces
{
    public interface IProcessor
    {
        void Run();

        void Halt();

        void EnableInterrupts();

        bool InterruptsEnabled { get; }

        void DisableInterrupts();
    }
}
