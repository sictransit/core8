namespace Core8.Interfaces
{
    public interface ITeleprinter
    {
        void Tick();

        void Print(byte c);

        bool IsFlagSet { get; }

        void ClearFlag();

        string Printout { get; }
    }
}
