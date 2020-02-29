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

        void LoadCommandRegister(int accumulator);

        int TransferDataRegister(int accumulator);

        bool SkipNotDone();

        void Initialize();
    }
}
