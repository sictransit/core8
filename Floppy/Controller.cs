using Core8.Floppy.Declarations;
using Core8.Floppy.Interfaces;
using Core8.Floppy.States.Abstract;
using Serilog;
using System;

namespace Core8.Floppy
{
    internal class Controller : IController
    {
        private StateBase state;

        private int commandRegister;

        private bool interruptsEnabled;

        private bool errorFlag;

        public Controller()
        {
            Buffer = new int[64];
        }

        public int[] Buffer { get; }

        public ControllerFunction CurrentFunction => (ControllerFunction)(commandRegister & 0b_000_000_001_110);

        public void SetCommandRegister(int acc)
        {
            commandRegister = acc & 0b_000_011_111_110;
        }

        public void SetState(StateBase state)
        {
            Log.Information($"Controller state transition: {this.state} -> {state}");

            this.state = state;
        }

        public bool MaintenanceMode => (commandRegister & 0b_000_010_000_000) != 0;

        private bool EightBitMode => (commandRegister & 0b_000_001_000_000) != 0;

        public int LCD(int acc) => state.LCD(acc);

        public int XDR(int acc) => state.XDR(acc);

        public bool SND() => state.SND();

        public bool STR() => state.STR();

        public void Tick() => state.Tick();

        public void ReadSector()
        {
            if (EightBitMode)
            {
                throw new NotImplementedException();
            }

            if (disk[UnitSelect] == null)
            {
                Log.Warning($"No disk in drive: {UnitSelect}");

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
                        Buffer[bufferPointer++] = (disk[UnitSelect][BlockAddress + i] << 4) | ((disk[UnitSelect][BlockAddress + i + 1] >> 4) & 0b_001_111);
                    }
                    else
                    {
                        Buffer[bufferPointer++] = ((disk[UnitSelect][BlockAddress + i] & 0b_001_111) << 8) | (disk[UnitSelect][BlockAddress + i + 1]);
                    }
                }
            }
        }

        public void WriteSector()
        {
            if (EightBitMode)
            {
                throw new NotImplementedException();
            }

            if (disk[UnitSelect] == null)
            {
                Log.Warning($"No disk in drive: {UnitSelect}");

                errorFlag = true;

                return;
            }


            var block = new byte[BlockSize];

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

            Array.Copy(block, 0, disk[UnitSelect], BlockAddress, block.Length);
        }

        private int trackAddress = 0;
        private int sectorAddress = 0;

        private const int FirstTrack = 0;
        private const int LastTrack = 76;
        private const int FirstSector = 1;
        private const int LastSector = 26;
        private const int BlockSize = 128;

        private readonly byte[][] disk = new byte[2][];

        private int BlockAddress => trackAddress * LastSector * BlockSize + (sectorAddress - 1) * BlockSize;

        private int UnitSelect => (commandRegister & 0b_000_000_010_000) >> 4;

        public bool IRQ => interruptsEnabled && state.IRQ;

        public void SetSectorAddress(int sector)
        {
            sectorAddress = sector & 0b_000_001_111_111;

            if (sectorAddress < FirstSector || sectorAddress > LastSector)
            {
                Log.Warning($"Bad sector address: {sectorAddress}");
            }
        }

        public void SetTrackAddress(int track)
        {
            trackAddress = track & 0b_000_011_111_111;

            if (trackAddress < FirstTrack || trackAddress > LastTrack)
            {
                Log.Warning($"Bad track address: {trackAddress}");
            }
        }

        public void Load(byte unit, byte[] disk = null)
        {
            this.disk[unit] = disk ?? new byte[(LastTrack + 1) * LastSector * BlockSize];
        }

        public void SetInterrupts(int acc)
        {
            interruptsEnabled = (acc & 1) == 1;
        }

        public bool SER()
        {
            if (errorFlag)
            {
                errorFlag = MaintenanceMode;

                return true;
            }

            return false;
        }
    }
}
