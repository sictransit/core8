namespace Core8.Interfaces
{
    public interface ITeleprinter : IIODevice
    {
        void Print(byte c);       

        string Printout { get; }
    }
}
