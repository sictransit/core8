namespace Core8.Model.Interfaces
{
    public interface IInstruction
    {
        IInstruction Load(int address, int data);

        void Execute();
    }
}
