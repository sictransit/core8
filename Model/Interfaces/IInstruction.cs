namespace Core8.Model.Interfaces
{
    public interface IInstruction
    {
        int Address { get; }

        int Data { get; }

        IInstruction Load(int address, int data);

        void Execute();
    }
}
