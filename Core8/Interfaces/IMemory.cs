namespace Core8.Interfaces
{
    public interface IMemory
    {
        uint Read(uint address);

        void Write(uint address, uint data);

        uint Size { get; }
    }
}
