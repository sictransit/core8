using Core8.Peripherals.RX8E.Registers;
using Core8.Peripherals.RX8E.States.Abstract;

namespace Core8.Peripherals.RX8E.Interfaces;

public interface IController
{
    void SetState(StateBase state);

    void WriteSector();

    void ReadSector();

    CommandRegister CR { get; }

    InterfaceRegister IR { get; }

    ErrorStatusRegister ES { get; }

    ErrorRegister ER { get; }

    void SetTransferRequest(bool state);

    void SetDone(bool state);

    void SetError(bool state);

    void SetInterrupts(int acc);

    int[] Buffer { get; }

    void SetSectorAddress(int sector);

    void SetTrackAddress(int track);
}
