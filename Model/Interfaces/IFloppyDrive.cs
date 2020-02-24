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

        void Load(byte[] disk);

        void LoadCommandRegister(int data);

        void TransferDataRegister();

        bool SkipNotDone();

    }
}
