namespace Core8.Model.Interfaces
{
    public interface ITeletype : IIODevice
    {
        void Type(byte c);

        void Clear();

        bool InputFlag { get; }

        void ClearInputFlag();

        bool OutputFlag { get; }

        void ClearOutputFlag();

        void SetOutputFlag();

        byte InputBuffer { get; }

        string Printout { get; }

        string PunchedTape { get; }

        void SetDeviceControl(int data);

        void MountPaperTape(byte[] chars);

        void RemovePaperTape();
    }
}
