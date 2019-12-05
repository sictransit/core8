namespace Core8.Interfaces
{
    public interface IProcessor
    {
        void Run();

        void Halt();

        uint CurrentAddress { get; }
    }
}
