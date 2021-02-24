using Core8.Extensions;
using Core8.Peripherals.Floppy.Declarations;
using Core8.Peripherals.Floppy.Interfaces;
using Core8.Peripherals.Floppy.Media;
using Core8.Peripherals.Floppy.Registers;
using Core8.Peripherals.Floppy.States.Abstract;
using Serilog;
using System;
using System.Linq;

namespace Core8.Peripherals.Floppy
{
    internal class Controller : IController
    {
        private StateBase currentState;

        public CommandRegister CR { get; }

        public InterfaceRegister IR { get; }

        private TrackAddressRegister TA { get; }

        private SectorAddressRegister SA { get; }

        public ErrorCodeRegister EC { get; }

        public ErrorStatusRegister ES { get; }

        private bool interruptsEnabled;

        private bool doneFlag;
        private bool transferRequestFlag;
        private bool errorFlag;

        public bool Done => doneFlag || CR.MaintenanceMode;

        public bool TransferRequest => transferRequestFlag || CR.MaintenanceMode;

        public bool Error => errorFlag || CR.MaintenanceMode;

        public Controller()
        {
            Buffer = new int[64];

            CR = new CommandRegister();
            IR = new InterfaceRegister();
            TA = new TrackAddressRegister();
            SA = new SectorAddressRegister();
            ES = new ErrorStatusRegister();
            EC = new ErrorCodeRegister();
        }

        public int[] Buffer { get; private set; }

        public void SetState(StateBase state)
        {
            Log.Debug($"Controller state transition: {currentState} -> {state}");

            currentState = state;
        }

        public void SetTransferRequest(bool state) => transferRequestFlag = state;

        public void SetDone(bool state) => doneFlag = state;

        public void SetError(bool state) => errorFlag = state;

        public void LCD(int acc) => currentState.LCD(acc);

        public int XDR(int acc) => currentState.XDR(acc);

        public void Tick()
        {
            Ticks++;

            currentState.Tick();
        }

        private bool TryRetrieveSector(out Sector sector)
        {
            sector = null;

            var disk = disks[CR.UnitSelect];

            if (disk != null)
            {
                if (disk.Tracks.TryGetValue(TA.Content, out var track))
                {
                    Log.Debug($"Found: {track}");

                    if (!track.Sectors.TryGetValue(SA.Content, out sector))
                    {
                        Log.Warning($"Bad sector address: {SA.Content}");

                        EC.Set(ErrorCodes.SEEK_FAILED);
                    }
                }
                else
                {
                    Log.Warning($"Bad track address: {TA.Content}");

                    EC.Set(ErrorCodes.BAD_TRACK_ADDRESS);
                }
            }
            else
            {
                Log.Warning($"No disk in drive: {CR.UnitSelect}");

                ES.SetWriteProtect(true);
            }

            errorFlag = sector == null;

            return !errorFlag;
        }

        public void ReadSector()
        {
            if (CR.EightBitMode)
            {
                throw new NotImplementedException();
            }

            if (TryRetrieveSector(out var sector))
            {
                Log.Debug($"Found: {sector}");

                Buffer = sector.Data.Pack(Buffer.Length).ToArray();
            }
            else
            {
                Log.Warning("Failed to retrieve sector.");
            }
        }

        public void WriteSector()
        {
            if (CR.EightBitMode)
            {
                throw new NotImplementedException();
            }

            if (TryRetrieveSector(out var sector))
            {
                Log.Debug($"Found: {sector}");

                sector.Data = Buffer.Unpack(96, sector.Data.Length).ToArray();
            }
            else
            {
                Log.Warning("Failed to retrieve sector.");
            }
        }

        private readonly Disk[] disks = new Disk[2];

        public bool IRQ => interruptsEnabled && (Done || Error);

        public int Ticks { get; private set; }

        public void SetSectorAddress(int sector) => SA.Set(sector);

        public void SetTrackAddress(int track) => TA.Set(track);

        public void Load(byte unit, byte[] data = null)
        {
            var disk = new Disk(unit);

            disk.Format();

            if (data != null)
            {
                disk.LoadFromArray(data);
            }

            disks[unit] = disk;
        }

        public void SetInterrupts(int acc) => interruptsEnabled = (acc & 1) == 1;

        public bool SER()
        {
            if (Error)
            {
                errorFlag = CR.MaintenanceMode;

                return true;
            }

            return false;
        }

        public bool SND()
        {
            if (Done)
            {
                doneFlag = CR.MaintenanceMode;

                return true;
            }

            return false;
        }


        public bool STR()
        {
            if (TransferRequest)
            {
                transferRequestFlag = CR.MaintenanceMode;

                return true;
            }

            return false;
        }

        public override string ToString()
        {
            var flags = new[] { Done ? "dne" : null, Error ? "err" : null, TransferRequest ? "tr" : null, CR.MaintenanceMode ? "mm" : null, CR.EightBitMode ? "8" : "12" }.Where(x => x != null);

            return $"[{GetType().Name}] {currentState} {string.Join(',', flags)} dsk={CR.UnitSelect}:{TA.Content}:{SA.Content} es={ES}";
        }
    }
}
