using Core8.Peripherals.RX8E.Registers;
using Core8.Peripherals.RX8E.States.Abstract;

namespace Core8.Peripherals.RX8E.Interfaces;

internal interface IController
{
    void SetState(StateBase state);

    void Load(byte unit, byte[] data = null);

    void WriteSector();

    void ReadSector();

    CommandRegister CR { get; }

    InterfaceRegister IR { get; }

    ErrorStatusRegister ES { get; }

    ErrorRegister ER { get; }

    bool Done { get; }

    bool Error { get; }

    bool TransferRequest { get; }

    void SetTransferRequest(bool state);

    void SetDone(bool state);

    void SetError(bool state);

    void SetInterrupts(int acc);

    bool IRQ { get; }

    void LCD(int acc);

    int XDR(int acc);

    bool SND();

    bool STR();

    bool SER();

    int[] Buffer { get; }

    int Ticks { get; }

    void Tick();

    void SetSectorAddress(int sector);

    void SetTrackAddress(int track);
}
