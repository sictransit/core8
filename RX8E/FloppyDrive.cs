using Core8.Extensions;
using Core8.Model;
using Core8.Model.Interfaces;
using Core8.Peripherals.RX8E.Declarations;
using Core8.Peripherals.RX8E.Interfaces;
using Core8.Peripherals.RX8E.Media;
using Core8.Peripherals.RX8E.Registers;
using Core8.Peripherals.RX8E.States;
using Core8.Peripherals.RX8E.States.Abstract;
using Serilog;
using System;
using System.Linq;

namespace Core8.Peripherals.RX8E;

public class FloppyDrive : IODevice, IController, IFloppyDrive
{
    private StateBase currentState;

    public CommandRegister CR { get; }

    public InterfaceRegister IR { get; }

    private TrackAddressRegister TA { get; }

    private SectorAddressRegister SA { get; }

    public ErrorRegister ER { get; }

    public ErrorStatusRegister ES { get; }

    private bool interruptsEnabled;

    private bool doneFlag;
    private bool transferRequestFlag;
    private bool errorFlag;

    public bool Done => doneFlag || CR.MaintenanceMode;

    public bool TransferRequest => transferRequestFlag || CR.MaintenanceMode;

    public bool Error => errorFlag || CR.MaintenanceMode;

    protected override int TickDelay => 3;

    public FloppyDrive(int deviceId = 61) : base(deviceId) // device 75: RX8E (floppy)
    {
        Buffer = new int[64];

        CR = new CommandRegister();
        IR = new InterfaceRegister();
        TA = new TrackAddressRegister();
        SA = new SectorAddressRegister();
        ES = new ErrorStatusRegister();
        ER = new ErrorRegister();

        Load(0);
        Load(1);

        Initialize();
    }

    public int[] Buffer { get; private set; }

    public void SetState(StateBase state)
    {
        Ticks = 0;

        currentState = state;
    }

    public void Initialize() => SetState(new Initialize(this));

    public void SetTransferRequest(bool state) => transferRequestFlag = state;

    public void SetDone(bool state) => doneFlag = state;

    public void SetError(bool state) => errorFlag = state;

    public void LoadCommandRegister(int acc) => currentState.LoadCommandRegister(acc);

    public int TransferDataRegister(int acc) => currentState.TransferDataRegister(acc);

    protected override bool RequestInterrupt => Done || Error;

    protected override bool InterruptEnable => interruptsEnabled;

    protected override void HandleTick()
    {
        currentState.Tick();
    }

    private bool TryRetrieveSector(out Sector sector)
    {
        sector = null;

        var disk = disks[CR.UnitSelect];

        if (disk != null)
        {
            if (disk.Tracks.TryGetValue(TA.Content, out Track track))
            {
                //Log.Debug($"Found: {track}");

                if (!track.Sectors.TryGetValue(SA.Content, out sector))
                {
                    Log.Warning($"Bad sector address: {SA.Content}");

                    ER.Set(ErrorCodes.SEEK_FAILED);
                }
            }
            else
            {
                Log.Warning($"Bad track address: {TA.Content}");

                ER.Set(ErrorCodes.BAD_TRACK_ADDRESS);
            }
        }
        else
        {
            Log.Warning($"No disk in drive: {CR.UnitSelect}");

            ES.SetWriteProtect(true);
        }

        errorFlag |= sector == null;

        return sector != null;
    }

    public void ReadSector()
    {
        if (CR.EightBitMode)
        {
            throw new NotImplementedException();
        }

        if (TryRetrieveSector(out var sector))
        {
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
            sector.Data = Buffer.Unpack(96, sector.Data.Length).ToArray();
        }
        else
        {
            Log.Warning("Failed to retrieve sector.");
        }
    }

    private readonly Disk[] disks = new Disk[2];


    public void SetSectorAddress(int sector) => SA.Set(sector);

    public void SetTrackAddress(int track) => TA.Set(track);

    public void Load(byte unit, byte[] data = null)
    {
        Disk disk = new(unit);

        disk.Format();

        if (data != null)
        {
            disk.LoadFromArray(data);
        }

        disks[unit] = disk;
    }

    public void SetInterrupts(int acc) => interruptsEnabled = (acc & 1) == 1;

    public bool SkipError()
    {
        if (Error)
        {
            errorFlag = CR.MaintenanceMode;

            return true;
        }

        return false;
    }

    public bool SkipNotDone()
    {
        if (Done)
        {
            doneFlag = CR.MaintenanceMode;

            return true;
        }

        return false;
    }


    public bool SkipTransferRequest()
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
        System.Collections.Generic.IEnumerable<string> flags = new[] { Done ? "dne" : null, Error ? "err" : null, TransferRequest ? "tr" : null, CR.MaintenanceMode ? "mm" : null, CR.EightBitMode ? "8" : "12" }.Where(x => x != null);

        return $"[{GetType().Name}] {currentState} {string.Join(',', flags)} dsk={CR.UnitSelect}:{TA.Content}:{SA.Content} es={ES}";
    }
}
