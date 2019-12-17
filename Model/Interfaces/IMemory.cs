namespace Core8.Model.Interfaces
{
    public interface IMemory
    {
        uint Read(uint address, bool examine = false);

        void Write(uint address, uint data);

        uint Size { get; }
    }
}
