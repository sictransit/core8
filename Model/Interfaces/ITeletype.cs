using System.Threading;

namespace Core8.Model.Interfaces
{
    public interface ITeletype
    {
        void Read(byte c);

        void Type(byte c);

        void InitiateOutput();

        void Clear();

        void ClearInputFlag();

        void ClearOutputFlag();

        void SetInputFlag();

        void SetOutputFlag();

        bool InputFlag { get; }

        bool OutputFlag { get; }

        bool InterruptRequested { get; }

        byte InputBuffer { get; }

        byte OutputBuffer { get; }

        AutoResetEvent CachedDataAvailableEvent { get; }

        byte[] GetCachedOutput();

        string Printout { get; }

        void SetDeviceControls(uint data);

        void MountPaperTape(byte[] chars);
    }
}
