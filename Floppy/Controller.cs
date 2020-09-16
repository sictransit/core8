﻿using Core8.Floppy.Declarations;
using Core8.Floppy.Interfaces;
using Core8.Floppy.Media;
using Core8.Floppy.Registers;
using Core8.Floppy.States.Abstract;
using Serilog;
using System;
using System.Linq;

namespace Core8.Floppy
{
    internal class Controller : IController
    {
        private StateBase state;

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

        public int[] Buffer { get; }

        public void SetState(StateBase state)
        {
            Log.Debug($"Controller state transition: {this.state} -> {state}");

            this.state = state;
        }

        public void SetTransferRequest(bool state) => transferRequestFlag = state;

        public void SetDone(bool state) => doneFlag = state;

        public void SetError(bool state) => errorFlag = state;

        public void LCD(int acc) => state.LCD(acc);

        public int XDR(int acc) => state.XDR(acc);

        public void Tick() => state.Tick();

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

                        EC.SetEC(ErrorCodes.SeekFailed);
                    }
                }
                else
                {
                    Log.Warning($"Bad track address: {TA.Content}");

                    EC.SetEC(ErrorCodes.BadTrackAddress);
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

                var bufferPointer = 0;

                for (int i = 0; i < 96; i += 3)
                {
                    Buffer[bufferPointer++] = sector.Data[i] << 4 | sector.Data[i + 1] >> 4;
                    Buffer[bufferPointer++] = (sector.Data[i + 1] & 0b_001_111) << 8 | sector.Data[i + 2];
                }
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

                var position = 0;

                // byte:  0  1  2  3  4  5
                // word: 00 01 11 22 23 33
                for (int i = 0; i < Buffer.Length; i++)
                {
                    if (i % 2 == 0)
                    {
                        sector.Data[position++] = (byte)(Buffer[i] >> 4);
                        sector.Data[position++] = (byte)(((Buffer[i] & 0b_001_111) << 4) | ((Buffer[i + 1] >> 8) & 0b_001_111));
                    }
                    else
                    {
                        sector.Data[position++] = (byte)(Buffer[i] & 0b_011_111_111);
                    }
                }
            }
            else
            {
                Log.Warning("Failed to retrieve sector.");
            }
        }

        private readonly Disk[] disks = new Disk[2];

        public bool IRQ => interruptsEnabled && (Done || Error);

        public void SetSectorAddress(int sector) => SA.SetSAR(sector);

        public void SetTrackAddress(int track) => TA.SetTAR(track);

        public void Load(byte unit, byte[] data = null)
        {
            var disk = new Disk(unit);

            if (data != null)
            {
                disk.LoadFromArray(data);
            }

            this.disks[unit] = disk;
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

            return $"[{GetType().Name}] {state} {string.Join(',', flags)} dsk={CR.UnitSelect}:{TA.Content}:{SA.Content}";
        }
    }
}
