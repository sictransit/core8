namespace Core8.Model.Interfaces
{
    public interface IFloppyDrive : IRequestsInterrupts
    {
        void Tick();

        void Load(byte unit, byte[] disk = null);

        void LoadCommandRegister(int accumulator);

        int TransferDataRegister(int accumulator);

        void SetInterrupts(int accumulator);

        bool SkipTransferRequest();

        bool SkipNotDone();

        bool SkipError();

        void Initialize();
    }
}
