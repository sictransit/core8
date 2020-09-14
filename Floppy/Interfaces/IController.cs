using Core8.Floppy.Registers;
using Core8.Floppy.States.Abstract;

namespace Core8.Floppy.Interfaces
{
    internal interface IController
    {
        void SetState(StateBase state);

        void Load(byte unit, byte[] disk = null);

        void WriteSector();

        void ReadSector();

        CommandRegister CR { get; }

        InterfaceRegister IR { get; }

        ErrorStatusRegister ES { get; }

        ErrorCodeRegister EC { get; }

        void SetInterrupts(int acc);

        bool IRQ { get; }

        void LCD(int acc);

        int XDR(int acc);

        bool SND();

        bool STR();

        bool SER();

        int[] Buffer { get; }

        void Tick();

        void SetSectorAddress(int sector);

        void SetTrackAddress(int track);
    }
}
