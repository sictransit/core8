using Core8.Floppy;
using Core8.Floppy.Declarations;
using Core8.Floppy.Interfaces;
using Core8.Floppy.States;
using Core8.Model.Interfaces;
using Serilog;
using System;

namespace Core8
{
    public class FloppyDrive : IFloppyDrive
    {
        private readonly IDrive drive;
        private readonly IController controller;

        public FloppyDrive()
        {
            controller = new RX8E();
            drive = new RX01();

            controller.SetState(new Idle(controller, drive));
        }

        public void SetCommandRegister(int acc)
        {
            commandRegister = acc & 0b_000_011_111_110;
        }

        private const int MIN_TRACK = 0;
        private const int MAX_TRACK = 76;
        private const int MIN_SECTOR = 1;
        private const int MAX_SECTOR = 26;
        private const int BLOCK_SIZE = 128;

        private const int ERROR_STATUS_INIT_DONE = 1 << 2;
        private const int ERROR_STATUS_DEVICE_READY = 1 << 7;

        private const int ERROR_CODE_BAD_TRACK = 0b_100_000;
        private const int ERROR_CODE_BAD_SECTOR = 0b_111_000;
        private const int ERROR_CODE_NO_DISK_IN_DRIVE = 0b_001_001_000;

        private const int TRACK_MASK = 0b_000_001_111_111;
        private const int SECTOR_MASK = 0b_000_000_011_111;


        [Flags]
        private enum ErrorStatusFlags
        {
            InitializationDone = ERROR_STATUS_INIT_DONE,
            DeviceReady = ERROR_STATUS_DEVICE_READY
        }

        private DateTime controllerDoneAt;

        private bool interruptsEnabled;

        private int commandRegister;

        private int interfaceRegister;

        private int errorStatusRegister;

        private int errorCodeRegister;

        private readonly int[] buffer = new int[64];

        private int bufferPointer;

        private readonly byte[][] disk = new byte[2][];

        private ControllerFunction Function => (ControllerFunction)(commandRegister & 0b_000_000_001_110);

        private bool EightBitMode => (commandRegister & 0b_000_001_000_000) != 0;

        private bool MaintenanceMode => (commandRegister & 0b_000_010_000_000) != 0;

        private int UnitSelect => (commandRegister & 0b_000_000_010_000) >> 4;

        private bool doneFlag;

        private bool errorFlag;

        private bool runningFlag;

        private bool transferRequestFlag;

        private bool initializingFlag;

        private bool expectDataFlag;

        public bool InterruptRequested => interruptsEnabled && doneFlag;

        private int sectorAddress;

        private int trackAddress;

        private int BlockAddress => trackAddress * MAX_SECTOR * BLOCK_SIZE + (sectorAddress - 1) * BLOCK_SIZE;

        public void Tick() => controller.Tick();

        public void Load(byte unit, byte[] disk)
        {
            this.disk[unit] = disk;
        }

        public int LoadCommandRegister(int accumulator) => controller.LCD(accumulator);

        private void ReadBlock()
        {
            if (EightBitMode)
            {
                throw new NotImplementedException();
            }

            var bufferPointer = 0;

            for (int i = 0; i < 96; i++)
            {
                if ((i + 1) % 3 != 0)
                {
                    if (bufferPointer % 2 == 0)
                    {
                        buffer[bufferPointer++] = (disk[UnitSelect][BlockAddress + i] << 4) | ((disk[UnitSelect][BlockAddress + i + 1] >> 4) & 0b_001_111);
                    }
                    else
                    {
                        buffer[bufferPointer++] = ((disk[UnitSelect][BlockAddress + i] & 0b_001_111) << 8) | (disk[UnitSelect][BlockAddress + i + 1]);
                    }
                }
            }
        }

        private void WriteBlock()
        {
            if (EightBitMode)
            {
                throw new NotImplementedException();
            }

            var block = new byte[BLOCK_SIZE];

            var position = 0;

            for (int i = 0; i < buffer.Length; i++)
            {
                if (i % 2 == 0)
                {
                    block[position++] = (byte)(buffer[i] >> 4);
                    block[position++] = (byte)((buffer[i] << 4) | ((buffer[i + 1] >> 8) & 0b_001_111));
                }
                else
                {
                    block[position++] = (byte)(buffer[i] & 0b_011_111_111);
                }
            }

            Array.Copy(block, 0, disk[UnitSelect], BlockAddress, block.Length);
        }

        public int TransferDataRegister(int accumulator)
        {
            if (!MaintenanceMode && !doneFlag)
            {
                switch (Function)
                {
                    case ControllerFunction.FillBuffer:
                    case ControllerFunction.ReadSector:
                    case ControllerFunction.WriteSector:
                    case ControllerFunction.WriteDeletedDataSector:
                        interfaceRegister = accumulator;
                        runningFlag = true;
                        break;
                    case ControllerFunction.EmptyBuffer:
                        runningFlag = true;
                        break;
                    default:
                        runningFlag = false;
                        break;
                }
            }

            if (EightBitMode)
            {
                throw new NotImplementedException();
            }
            else
            {
                return interfaceRegister;
            }
        }

        //public int TransferDataRegister(int accumulator)
        //{
        //    errorStatusRegister &= ERROR_STATUS_INIT_DONE;

        //    switch (state)
        //    {
        //        case ControllerState.Idle:
        //            break;

        //        case ControllerState.FillBuffer:
        //            interfaceRegister = accumulator;
        //            FillBuffer();
        //            break;

        //        case ControllerState.EmptyBuffer:
        //            EmptyBuffer();
        //            break;

        //        case ControllerState.WriteSector:
        //            interfaceRegister = accumulator;
        //            SetSector();
        //            state = ControllerState.WriteTrack;
        //            break;

        //        case ControllerState.WriteTrack:
        //            interfaceRegister = accumulator;
        //            SetTrack();
        //            break;

        //        case ControllerState.ReadSector:
        //            interfaceRegister = accumulator;
        //            SetSector();
        //            state = ControllerState.ReadTrack;
        //            break;

        //        case ControllerState.ReadTrack:
        //            interfaceRegister = accumulator;
        //            SetTrack();
        //            break;

        //        default:
        //            throw new InvalidOperationException(state.ToString());
        //    }

        //    return interfaceRegister;
        //}

        public void Initialize()
        {
            controller.SetState(new Initialize(controller, drive));
        }

        private void InitializeDone()
        {
            initializingFlag = false;
            doneFlag = true;
            errorFlag = false;

            interruptsEnabled = false;
            errorStatusRegister = 0;
            interfaceRegister = 0;
            commandRegister = 0;
            bufferPointer = 0;

            trackAddress = 1;
            sectorAddress = 1;
        }

        private void SetSector()
        {
            sectorAddress = interfaceRegister & SECTOR_MASK;

            transferRequestFlag = true;
        }

        private void SetTrack()
        {
            trackAddress = interfaceRegister & TRACK_MASK;

            if (trackAddress < MIN_TRACK || trackAddress > MAX_TRACK)
            {
                Log.Warning($"Bad track address: {trackAddress}");

                //SetDone(0, ERROR_CODE_BAD_TRACK);
            }
            else if (sectorAddress < MIN_SECTOR || sectorAddress > MAX_SECTOR)
            {
                Log.Warning($"Bad sector address: {sectorAddress}");

                //SetDone(0, ERROR_CODE_BAD_SECTOR);
            }
            else if (disk[UnitSelect] == null)
            {
                Log.Warning($"No disk in drive: {UnitSelect}");

                //SetDone(0, ERROR_CODE_NO_DISK_IN_DRIVE);
            }
            else
            {
                if (trackAddress == MIN_TRACK)
                {
                    Log.Warning("@ track #0");
                }

                controllerDoneAt = DateTime.UtcNow + Latencies.AverageAccessTime;
            }
        }

        private bool FillBuffer()
        {
            if (EightBitMode)
            {
                throw new NotImplementedException();
            }

            buffer[bufferPointer++] = interfaceRegister;

            return bufferPointer < buffer.Length;
        }

        private void EmptyBuffer()
        {
            if (EightBitMode)
            {
                throw new NotImplementedException();
            }

            if (bufferPointer >= buffer.Length)
            {
                controllerDoneAt = DateTime.UtcNow;
            }
            else
            {
                interfaceRegister = buffer[bufferPointer++];

                transferRequestFlag = true;
            }
        }

        public bool SkipTransferRequest() => controller.STR();

        public bool SkipError()
        {
            if (errorFlag)
            {
                errorFlag = MaintenanceMode;

                return true;
            }

            return false;
        }

        public bool SkipNotDone() => controller.SND();

        public void SetInterrupts(int accumulator)
        {
            interruptsEnabled = (accumulator & 1) == 1;
        }

        public override string ToString()
        {
            return $"[RX01] {Function} dn={(doneFlag ? 1 : 0)} tr={(transferRequestFlag ? 1 : 0)} er={(errorFlag ? 1 : 0)} md={(EightBitMode ? 8 : 12)} mnt={(MaintenanceMode ? 1 : 0)} unt={UnitSelect} trk={trackAddress} sc={sectorAddress} bp={bufferPointer}";
        }
    }
}