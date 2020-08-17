using Core8.Model.Interfaces;
using Serilog;
using System;

namespace Core8
{
    public class FloppyDrive : IFloppyDrive
    {
        public static TimeSpan InitializeTime => TimeSpan.FromMilliseconds(1800);
        public static TimeSpan ReadStatusTime => TimeSpan.FromMilliseconds(250);
        public static TimeSpan CommandTime => TimeSpan.FromMilliseconds(100);
        public static TimeSpan AverageAccessTime => TimeSpan.FromMilliseconds(488);

        public const int FILL_BUFFER = 0 << 1;
        public const int EMPTY_BUFFER = 1 << 1;
        public const int WRITE_SECTOR = 2 << 1;
        public const int READ_SECTOR = 3 << 1;
        public const int NO_OPERATION = 4 << 1;
        public const int READ_STATUS = 5 << 1;
        public const int WRITE_DELETED_DATA_SECTOR = 6 << 1;
        public const int READ_ERROR_REGISTER = 7 << 1;

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

        private enum ControllerFunction
        {
            FillBuffer = FILL_BUFFER,
            EmptyBuffer = EMPTY_BUFFER,
            WriteSector = WRITE_SECTOR,
            ReadSector = READ_SECTOR,
            NoOperation = NO_OPERATION,
            ReadStatus = READ_STATUS,
            WriteDeletedDataSector = WRITE_DELETED_DATA_SECTOR,
            ReadErrorRegister = READ_ERROR_REGISTER
        }

        public enum ControllerState
        {
            Idle,
            Initialize,
            FillBuffer,
            EmptyBuffer,
            WriteSector,
            WriteTrack,
            ReadSector,
            ReadTrack,
            ReadStatus,
            Done,
        }

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

        private ControllerState state;

        private ControllerFunction FunctionSelect => (ControllerFunction)Function;

        private int Function => (commandRegister & 0b_000_000_001_110);

        private bool EightBitMode => (commandRegister & 0b_000_001_000_000) != 0;

        private bool MaintenanceMode => (commandRegister & 0b_000_010_000_000) != 0;

        private int UnitSelect => (commandRegister & 0b_000_000_010_000) >> 4;

        private bool doneFlag;

        private bool errorFlag;

        private bool transferRequestFlag;        

        public bool InterruptRequested => interruptsEnabled && doneFlag;

        private int sectorAddress;

        private int trackAddress;

        private int BlockAddress => trackAddress * MAX_SECTOR * BLOCK_SIZE + (sectorAddress - 1) * BLOCK_SIZE;

        

        public void Tick()
        {
            if (state != ControllerState.Idle && DateTime.UtcNow > controllerDoneAt)
            {
                ControllerAction();

                //controllerDoneAt = DateTime.MaxValue; // Ugly! Check state instead?
            }
        }

        private void SetDone(int errorStatus = 0, int errorCode = 0, bool readErrorRegister = false)
        {
            if (errorCode != 0)
            {
                errorFlag = true;
            }

            errorCodeRegister = errorCode;

            errorStatusRegister |= errorStatus;

            errorStatusRegister |= (disk[UnitSelect] != null ? ERROR_STATUS_DEVICE_READY : 0);

            if (!MaintenanceMode)
            {
                interfaceRegister = readErrorRegister ? errorCodeRegister : errorStatusRegister;
            }

            SetState(ControllerState.Idle);

            doneFlag = true;
        }

        private void SetState(ControllerState state)
        {
            this.state = state;
        }

        public void Load(byte unit, byte[] disk)
        {
            this.disk[unit] = disk;
        }

        public int LoadCommandRegister(int accumulator)
        {
            if (state != ControllerState.Idle)
            {
                return accumulator;
            }

            bufferPointer = 0;

            commandRegister = accumulator & 0b_000_011_111_110;

            if (MaintenanceMode)
            {
                SetDone(0, 0);

                return interfaceRegister;
            }

            interfaceRegister = accumulator;

            switch (Function)
            {
                case FILL_BUFFER:
                    SetState(ControllerState.FillBuffer);
                    errorStatusRegister &= ERROR_STATUS_INIT_DONE;
                    transferRequestFlag = true;
                    break;
                case EMPTY_BUFFER:
                    SetState(ControllerState.EmptyBuffer);
                    errorStatusRegister &= ERROR_STATUS_INIT_DONE;
                    transferRequestFlag = true;
                    break;
                case WRITE_SECTOR:
                    SetState(ControllerState.WriteSector);
                    errorStatusRegister &= ERROR_STATUS_INIT_DONE;
                    transferRequestFlag = true;
                    break;
                case READ_SECTOR:
                    SetState(ControllerState.ReadSector);
                    errorStatusRegister &= ERROR_STATUS_INIT_DONE;
                    transferRequestFlag = true;
                    break;
                case NO_OPERATION:
                    SetState(ControllerState.Done);
                    controllerDoneAt = DateTime.UtcNow;
                    break;
                case READ_STATUS:
                    SetState(ControllerState.ReadStatus);
                    controllerDoneAt = DateTime.UtcNow + ReadStatusTime;
                    break;
                case WRITE_DELETED_DATA_SECTOR:
                    throw new NotImplementedException();
                case READ_ERROR_REGISTER:
                    SetDone(0, 0, true);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return 0;
        }

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

        private void ControllerAction()
        {
            switch (state)
            {
                case ControllerState.FillBuffer:
                case ControllerState.EmptyBuffer:
                    SetDone();
                    break;
                case ControllerState.Initialize:
                    ReadBlock();
                    SetDone(ERROR_STATUS_INIT_DONE | ERROR_STATUS_DEVICE_READY, 0);
                    break;
                case ControllerState.Done:
                    SetDone();
                    break;
                case ControllerState.ReadTrack:
                    ReadBlock();
                    SetDone();
                    break;
                case ControllerState.WriteTrack:
                    WriteBlock();
                    SetDone();
                    break;
                case ControllerState.ReadStatus:
                    SetDone();
                    break;
                default:
                    throw new InvalidOperationException(state.ToString());
            }
        }

        public int TransferDataRegister(int accumulator)
        {
            errorStatusRegister &= ERROR_STATUS_INIT_DONE;

            switch (state)
            {
                case ControllerState.Idle:
                    break;

                case ControllerState.FillBuffer:
                    interfaceRegister = accumulator;
                    FillBuffer();
                    break;

                case ControllerState.EmptyBuffer:
                    EmptyBuffer();
                    break;

                case ControllerState.WriteSector:
                    interfaceRegister = accumulator;
                    SetSector();
                    state = ControllerState.WriteTrack;
                    break;

                case ControllerState.WriteTrack:
                    interfaceRegister = accumulator;
                    SetTrack();
                    break;

                case ControllerState.ReadSector:
                    interfaceRegister = accumulator;
                    SetSector();
                    state = ControllerState.ReadTrack;
                    break;

                case ControllerState.ReadTrack:
                    interfaceRegister = accumulator;
                    SetTrack();
                    break;

                default:
                    throw new InvalidOperationException(state.ToString());
            }

            return interfaceRegister;
        }

        public void Initialize()
        {
            if (disk[UnitSelect] == null)
            {
                return;
            }

            SetState(ControllerState.Initialize);

            doneFlag = false;
            errorFlag = false;

            interruptsEnabled = false;
            errorStatusRegister = 0;
            interfaceRegister = 0;
            commandRegister = 0;
            bufferPointer = 0;

            trackAddress = 1;
            sectorAddress = 1;

            controllerDoneAt = DateTime.UtcNow + InitializeTime;
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

                SetDone(0, ERROR_CODE_BAD_TRACK);
            }
            else if (sectorAddress < MIN_SECTOR || sectorAddress > MAX_SECTOR)
            {
                Log.Warning($"Bad sector address: {sectorAddress}");

                SetDone(0, ERROR_CODE_BAD_SECTOR);
            }
            else if (disk[UnitSelect] == null)
            {
                Log.Warning($"No disk in drive: {UnitSelect}");

                SetDone(0, ERROR_CODE_NO_DISK_IN_DRIVE);
            }
            else
            {
                if (trackAddress == MIN_TRACK)
                {
                    Log.Warning("@ track #0");
                }

                controllerDoneAt = DateTime.UtcNow + AverageAccessTime;
            }
        }

        private void FillBuffer()
        {
            if (EightBitMode)
            {
                throw new NotImplementedException();
            }

            buffer[bufferPointer++] = interfaceRegister;

            if (bufferPointer >= buffer.Length)
            {
                controllerDoneAt = DateTime.UtcNow;
            }
            else
            {
                transferRequestFlag = true;
            }
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

        public bool SkipTransferRequest()
        {
            if (transferRequestFlag)
            {
                transferRequestFlag = MaintenanceMode;

                return true;
            }

            return false;
        }

        public bool SkipError()
        {
            if (errorFlag)
            {
                errorFlag = MaintenanceMode;

                return true;
            }

            return false;
        }

        public bool SkipNotDone()
        {
            if (doneFlag)
            {
                doneFlag = false;

                return true;
            }

            return false;
        }

        public void SetInterrupts(int accumulator)
        {
            interruptsEnabled = (accumulator & 1) == 1;
        }

        public override string ToString()
        {
            return $"[RX01] {FunctionSelect} dn={(doneFlag ? 1 : 0)} tr={(transferRequestFlag ? 1 : 0)} er={(errorFlag ? 1 : 0)} md={(EightBitMode ? 8 : 12)} mnt={(MaintenanceMode ? 1 : 0)} unt={UnitSelect} trk={trackAddress} sc={sectorAddress} bp={bufferPointer}";
        }
    }
}