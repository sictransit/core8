namespace Core8.Model.Interfaces
{
    public interface IInstructionSet
    {
        IInstruction Decode(int data);
    }
}