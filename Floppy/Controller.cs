using Core8.Floppy.Declarations;
using Core8.Floppy.Interfaces;
using Core8.Floppy.Registers;
using Core8.Floppy.States.Abstract;
using Serilog;
using System;

namespace Core8.Floppy
{
    internal class Controller : IController
    {
        private StateBase state;

        public CommandRegister CR { get; }

        public InterfaceRegister IR { get; }

        public TrackAddressRegister TA { get; }

        public SectorAddressRegister SA { get; }

        public ErrorCodeRegister EC { get; }

        public ErrorStatusRegister ES { get; }

        private bool interruptsEnabled;

        private bool errorFlag;

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
            Log.Information($"Controller state transition: {this.state} -> {state}");

            this.state = state;
        }


        public int LCD(int acc) => state.LCD(acc);

        public int XDR(int acc) => state.XDR(acc);

        public bool SND() => state.SND();

        public bool STR() => state.STR();

        public void Tick() => state.Tick();

        public void ReadSector()
        {
            if (CR.EightBitMode)
            {
                throw new NotImplementedException();
            }

            var disk = disks[CR.UnitSelect];

            if (disk == null)
            {
                Log.Warning($"No disk in drive: {CR.UnitSelect}");

                errorFlag = true;

                return;
            }

            var bufferPointer = 0;

            for (int i = 0; i < 96; i++)
            {
                if ((i + 1) % 3 != 0)
                {
                    if (bufferPointer % 2 == 0)
                    {
                        Buffer[bufferPointer++] = (disk[BlockAddress + i] << 4) | ((disk[BlockAddress + i + 1] >> 4) & 0b_001_111);
                    }
                    else
                    {
                        Buffer[bufferPointer++] = ((disk[BlockAddress + i] & 0b_001_111) << 8) | (disk[BlockAddress + i + 1]);
                    }
                }
            }
        }

        public void WriteSector()
        {
            if (CR.EightBitMode)
            {
                throw new NotImplementedException();
            }

            var disk = disks[CR.UnitSelect];

            if (disk == null)
            {
                Log.Warning($"No disk in drive: {CR.UnitSelect}");

                errorFlag = true;

                return;
            }

            var block = new byte[DiskLayout.BlockSize];

            var position = 0;

            for (int i = 0; i < Buffer.Length; i++)
            {
                if (i % 2 == 0)
                {
                    block[position++] = (byte)(Buffer[i] >> 4);
                    block[position++] = (byte)((Buffer[i] << 4) | ((Buffer[i + 1] >> 8) & 0b_001_111));
                }
                else
                {
                    block[position++] = (byte)(Buffer[i] & 0b_011_111_111);
                }
            }

            Array.Copy(block, 0, disk, BlockAddress, block.Length);
        }


        private readonly byte[][] disks = new byte[2][];

        private int BlockAddress => TA.Content * DiskLayout.LastSector * DiskLayout.BlockSize + (SA.Content - 1) * DiskLayout.BlockSize;

        public bool IRQ => interruptsEnabled && state.IRQ;

        public void SetSectorAddress(int sector)
        {
            SA.SetSAR(sector);

            if (SA.Content < DiskLayout.FirstSector || SA.Content > DiskLayout.LastSector)
            {
                Log.Warning($"Bad sector address: {SA.Content}");
            }
        }

        public void SetTrackAddress(int track)
        {
            TA.SetTAR(track);

            if (TA.Content < DiskLayout.FirstTrack || TA.Content > DiskLayout.LastTrack)
            {
                Log.Warning($"Bad track address: {TA.Content}");
            }
        }

        public void Load(byte unit, byte[] disk = null)
        {
            this.disks[unit] = disk ?? new byte[(DiskLayout.LastTrack + 1) * DiskLayout.LastSector * DiskLayout.BlockSize];
        }

        public void SetInterrupts(int acc)
        {
            interruptsEnabled = (acc & 1) == 1;
        }

        public bool SER()
        {
            if (errorFlag)
            {
                errorFlag = CR.MaintenanceMode;

                return true;
            }

            return false;
        }
    }
}
