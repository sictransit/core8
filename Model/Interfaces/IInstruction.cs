namespace Core8.Model.Interfaces
{
    public interface IInstruction
    {
        uint Address { get; }

        uint Data { get; }

        void Load(uint address, uint data);

        void Execute();
    }
}
