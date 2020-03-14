using Core8.Model.Interfaces;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core8
{
    public class FloppyDrive : IFloppyDrive
    {
        private readonly TimeSpan initializeTime = TimeSpan.FromMilliseconds(1800);
        private readonly TimeSpan readStatusTime = TimeSpan.FromMilliseconds(250);
        public TimeSpan CommandTime => TimeSpan.FromMilliseconds(100);
        private readonly TimeSpan averageAccessTime = TimeSpan.FromMilliseconds(488);

        private const int FILL_BUFFER = 0;
        private const int EMPTY_BUFFER = 1;
        private const int WRITE_SECTOR = 2;
        private const int READ_SECTOR = 3;
        private const int NO_OPERATION = 4;
        private const int READ_STATUS = 5;
        private const int WRITE_DELETED_DATA_SECTOR = 6;
        private const int READ_ERROR_REGISTER = 7;

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
            Done,
        }

        [Flags]
        private enum ErrorStatusFlags
        {
            InitializationDone = ERROR_STATUS_INIT_DONE,
            DeviceReady = ERROR_STATUS_DEVICE_READY
        }

        private volatile int commandRegister;

        private volatile int interfaceRegister;

        private volatile int errorStatusRegister;

        private volatile int errorCodeRegister;

        private readonly int[] buffer = new int[64];

        private int bufferPointer;

        private readonly byte[][] disk = new byte[2][];

        public FloppyDrive()
        {
        }

        private ControllerState state;

        private int Function => (commandRegister & 0b_001_110) >> 1;

        private ControllerFunction FunctionSelect => (ControllerFunction)Function;

        private bool EightBitMode => (commandRegister & 0b_001_000_000) != 0;

        private int UnitSelect => (commandRegister >> 4) & 0b_001;

        private volatile bool doneFlag;

        public bool TransferRequest { get; private set; }

        public bool InterruptRequested => doneFlag;

        private int sectorAddress;

        private int trackAddress;

        private int BlockAddress => trackAddress * MAX_SECTOR * BLOCK_SIZE + (sectorAddress - 1) * BLOCK_SIZE;

        public bool Error { get; private set; }

        public void ClearError()
        {
            Error = false;
        }

        public void ClearDone()
        {
            doneFlag = false;
        }

        private void ScheduleDone()
        {
            SetState(ControllerState.Done);
            Task.Run(ControllerAction);
        }

        private void SetDone(int errorStatus = 0, int errorCode = 0)
        {
            if (errorCode != 0)
            {
                Error = true;
            }

            errorCodeRegister = errorCode;

            errorStatusRegister |= errorStatus;

            errorStatusRegister |= (disk[UnitSelect] != null ? ERROR_STATUS_DEVICE_READY : 0);

            interfaceRegister = errorStatusRegister;

            SetState(ControllerState.Idle);

            doneFlag = true;
        }

        private void SetState(ControllerState state)
        {
            this.state = state;
        }

        public void ClearTransferRequest()
        {
            TransferRequest = false;
        }

        private void SetTransferRequest()
        {
            TransferRequest = true;

            //Log.Information("TRQ set");
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

            ClearError();
            ClearDone();
            ClearTransferRequest();

            bufferPointer = 0;

            commandRegister = accumulator & 0b_000_011_111_110;
            interfaceRegister = accumulator;

            switch (Function)
            {
                case FILL_BUFFER:
                    SetState(ControllerState.FillBuffer);
                    errorStatusRegister &= ERROR_STATUS_INIT_DONE;
                    SetTransferRequest();
                    break;
                case EMPTY_BUFFER:
                    SetState(ControllerState.EmptyBuffer);
                    errorStatusRegister &= ERROR_STATUS_INIT_DONE;
                    SetTransferRequest();
                    break;
                case WRITE_SECTOR:
                    SetState(ControllerState.WriteSector);
                    errorStatusRegister &= ERROR_STATUS_INIT_DONE;
                    SetTransferRequest();
                    break;
                case READ_SECTOR:
                    SetState(ControllerState.ReadSector);
                    errorStatusRegister &= ERROR_STATUS_INIT_DONE;
                    SetTransferRequest();
                    break;
                case NO_OPERATION:
                    //SetState(ControllerState.Done);
                    //Task.Run(ControllerAction);
                    break;
                case READ_STATUS:
                    throw new NotImplementedException();
                case WRITE_DELETED_DATA_SECTOR:
                    throw new NotImplementedException();
                case READ_ERROR_REGISTER:
                    throw new NotImplementedException();
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
                case ControllerState.Initialize:
                    Thread.Sleep(initializeTime);
                    ReadBlock();
                    SetDone(ERROR_STATUS_INIT_DONE | ERROR_STATUS_DEVICE_READY, 0);
                    break;
                case ControllerState.Done:
                    Thread.Sleep(CommandTime);
                    SetDone();
                    break;
                case ControllerState.ReadTrack:
                    Thread.Sleep(averageAccessTime);
                    ReadBlock();
                    SetDone();
                    break;
                case ControllerState.WriteTrack:
                    Thread.Sleep(averageAccessTime);
                    WriteBlock();
                    SetDone();
                    break;
                default:
                    throw new InvalidOperationException(state.ToString());
            }
        }

        public int TransferDataRegister(int accumulator)
        {
            if (doneFlag)
            {
                return interfaceRegister;
            }

            errorStatusRegister &= ERROR_STATUS_INIT_DONE;

            switch (state)
            {
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
            SetState(ControllerState.Initialize);

            ClearDone();

            errorStatusRegister = 0;
            interfaceRegister = 0;
            commandRegister = 0;
            bufferPointer = 0;

            trackAddress = 1;
            sectorAddress = 1;

            Task.Run(ControllerAction);
        }

        private void SetSector()
        {
            sectorAddress = interfaceRegister & 0b_000_000_011_111;

            SetTransferRequest();
        }

        private void SetTrack()
        {
            trackAddress = interfaceRegister & 0b_000_001_111_111;

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
                Task.Run(ControllerAction);
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
                ScheduleDone();
            }
            else
            {
                SetTransferRequest();
            }
        }

        private void EmptyBuffer()
        {
            if (EightBitMode)
            {
                throw new NotImplementedException();
            }

            interfaceRegister = buffer[bufferPointer++];

            if (bufferPointer >= buffer.Length)
            {
                ScheduleDone();
            }
            else
            {
                SetTransferRequest();
            }
        }

        public bool SkipTransferRequest()
        {
            if (TransferRequest)
            {
                ClearTransferRequest();

                return true;
            }

            return false;
        }

        public bool SkipError()
        {
            if (Error)
            {
                ClearError();

                return true;
            }

            return false;
        }

        public bool SkipNotDone()
        {
            if (doneFlag)
            {
                ClearDone();

                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return $"[RX01] {FunctionSelect} done={(doneFlag ? 1 : 0)} tr={(TransferRequest ? 1 : 0)} mode={(EightBitMode ? 8 : 12)} unit={UnitSelect} trk={trackAddress} sec={sectorAddress} bp={bufferPointer}";
        }

    }
}