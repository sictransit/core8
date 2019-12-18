namespace Core8.Model.Interfaces
{
    public interface ITeleprinter : IIODevice, IOutputDevice
    {
        string Printout { get; }
    }
}
