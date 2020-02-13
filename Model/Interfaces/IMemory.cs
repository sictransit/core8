namespace Core8.Model.Interfaces
{
    public interface IMemory
    {
        int Read(int address, bool indirect = false);

        int Examine(int address);

        int Write(int address, int data);

        int Size { get; }
    }
}
