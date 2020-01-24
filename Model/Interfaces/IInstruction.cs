namespace Core8.Model.Interfaces
{
    public interface IInstruction
    {
        uint Field { get; }

        uint Address { get; }

        uint Data { get; }

        void Load(uint field, uint address, uint data);

        void Execute();
    }
}
