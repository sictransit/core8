namespace Core8.Model.Interfaces
{
    public interface IMemory
    {
        uint Read(uint field, uint address, bool indirect = false);

        uint Examine(uint field, uint address);

        void Write(uint field, uint address, uint data);

        uint Fields { get; }

        uint MB { get; }
    }
}
