namespace Core8.Model.Interfaces
{
    public interface ITeletype
    {
        void Tick();

        void Type(byte c);

        void Clear();

        bool InputFlag { get; set; }

        bool OutputFlag { get; set; }

        bool InterruptRequested { get; }

        byte InputBuffer { get; }

        string Printout { get; }

        void SetDeviceControl(int data);

        void MountPaperTape(byte[] chars);
    }
}
