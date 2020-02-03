namespace Core8.Model.Interfaces
{
    public interface IInstruction
    {
        uint Field { get; }

        uint Address { get; }

        uint Data { get; }

        IInstruction Load(uint field, uint address, uint data);

        void Execute();

        bool UserModeInterrupt { get; }
    }
}
