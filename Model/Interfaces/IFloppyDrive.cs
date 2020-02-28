using Core8.Model.Register;

namespace Core8.Model.Interfaces
{
    public interface IFloppyDrive
    {
        bool Done { get; }

        bool Error { get; }

        bool TransferRequest { get; }

        bool InterruptRequested { get; }

        void ClearDone();

        void ClearError();

        void ClearTransferRequest();

        void Load(byte unit, byte[] disk);

        void LoadCommandRegister(int data);

        void TransferDataRegister(LinkAccumulator linkAc);

        bool SkipNotDone();

        void Initialize();
    }
}
