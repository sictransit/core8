namespace Core8.Model.Interfaces
{
    public interface IInstruction
    {
        IInstruction Load(int address, int data);

        int Data { get; }

        void Execute();
    }
}
