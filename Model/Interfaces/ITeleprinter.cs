namespace Core8.Model.Interfaces
{
    public interface ITeleprinter : IIODevice, IOutputDevice
    {
        void Print(byte data);

        string Printout { get; }
    }
}
