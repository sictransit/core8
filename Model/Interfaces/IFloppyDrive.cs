namespace Core8.Model.Interfaces
{
    public interface IFloppyDrive
    {
        bool InterruptRequested { get; }

        void Load(byte unit, byte[] disk);

        void LoadCommandRegister(int accumulator);

        int TransferDataRegister(int accumulator);

        bool SkipTransferRequest();

        bool SkipNotDone();

        bool SkipError();

        void Initialize();
    }
}
