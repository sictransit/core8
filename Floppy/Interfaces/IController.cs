using Core8.Floppy.Declarations;
using Core8.Floppy.States.Abstract;

namespace Core8.Floppy.Interfaces
{
    internal interface IController
    {
        void SetState(StateBase state);

        void SetCommandRegister(int acc);

        void Load(byte unit, byte[] disk = null);

        void WriteSector();

        void ReadSector();

        ControllerFunction CurrentFunction { get; }

        void SetInterrupts(int acc);

        bool IRQ { get; }

        int LCD(int acc);

        int XDR(int acc);

        bool SND();

        bool STR();

        bool SER();

        bool MaintenanceMode { get; }

        int[] Buffer { get; }

        void Tick();

        void SetSectorAddress(int sector);

        void SetTrackAddress(int track);
    }
}
