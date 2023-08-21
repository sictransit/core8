namespace Core8.Model.Interfaces;

public interface IInstruction
{
    IInstruction LoadData(int data);

    int Data { get; }

    void Execute();
}
