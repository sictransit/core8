namespace Core8.Model.Interfaces
{
    public interface IMemory
    {
        uint Read(uint address, bool indirect = false);

        uint Examine(uint address);

        void Write(uint address, uint data);

        uint Size { get; }
    }
}
