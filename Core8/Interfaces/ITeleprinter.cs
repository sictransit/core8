namespace Core8.Interfaces
{
    public interface ITeleprinter : IIODevice
    {
        void Print(byte c);

        bool IsFlagSet { get; }

        void ClearFlag();

        string Printout { get; }
    }
}
