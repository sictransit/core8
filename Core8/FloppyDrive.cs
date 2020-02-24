using Core8.Model.Interfaces;
using Core8.Model.Register;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Core8
{
    public class FloppyDrive : IFloppyDrive
    {
        private const int FILL_BUFFER = 0;
        private const int EMPTY_BUFFER = 1;
        private const int WRITE_SECTOR = 2;
        private const int READ_SECTOR = 3;
        private const int NO_OPERATION = 4;
        private const int READ_STATUS = 5;
        private const int WRITE_DELETED_DATA_SECTOR = 6;
        private const int READ_ERROR_REGISTER = 7;

        public enum ControllerState
        {
            Idle,
            FillBuffer,
            EmptyBuffer,
            WriteSector,
            ReadSector,
            WriteDeletedDataSector,
        }

        private int commandRegister;

        private volatile bool done;

        private int[] buffer = new int[128];

        private readonly LinkAccumulator accumulator;

        private int bufferPointer;

        private byte[] disk;

        public FloppyDrive(LinkAccumulator accumulator)
        {
            this.accumulator = accumulator;
        }

        public ControllerState State { get; private set; }


        private int Function => (commandRegister & 0b_001_110) >> 1;

        public bool EightBitMode => (commandRegister & 0b_001_000_000) != 0;

        public bool Done => done;

        public bool InterruptRequested { get; private set; }

        public bool TransferRequest { get; private set; }

        public int SectorAddress { get; private set; }

        public int TrackAddress { get; private set; }

        public bool Error { get; private set; }

        public void ClearError()
        {
            Error = false;
        }

        public void ClearDone()
        {
            done = false;

            InterruptRequested = true;
        }

        private void SetDone()
        {
            done = true;
        }

        public void ClearTransferRequest()
        {
            TransferRequest = false;
        }

        public void Load(byte[] disk)
        {
            this.disk = disk;

            TrackAddress = 0;
            SectorAddress = 1;

            ReadBlock();

            SetDone();
        }

        public void LoadCommandRegister(int data)
        {
            if (State != ControllerState.Idle)
            {
                return;
            }

            commandRegister = data & 0b_000_011_111_110;

            Task.Run(ExecuteCommand);
        }

        private void ExecuteCommand()
        {
            switch (Function)
            {
                case FILL_BUFFER:
                    State = ControllerState.FillBuffer;
                    break;
                case EMPTY_BUFFER:
                    State = ControllerState.EmptyBuffer;
                    break;
                case WRITE_SECTOR:
                    State = ControllerState.WriteSector;
                    break;
                case READ_SECTOR:
                    State = ControllerState.ReadSector;
                    break;
                case NO_OPERATION:
                    SetDone();
                    break;
                case READ_STATUS:
                    ReadStatus();
                    break;
                case WRITE_DELETED_DATA_SECTOR:
                    State = ControllerState.WriteDeletedDataSector;
                    break;
                case READ_ERROR_REGISTER:
                    ReadErrorRegister();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void ReadStatus()
        {
            throw new NotImplementedException();
        }

        private void ReadErrorRegister()
        {
            throw new NotImplementedException();
        }

        private void ReadBlock()
        {
            Array.Copy(disk, TrackAddress * 26 + (SectorAddress - 1), buffer, 0, buffer.Length);

            if (!EightBitMode)
            {
                int packed = 0;

                buffer = buffer.Select((x, i) =>
                {
                    if (i < buffer.Length / 3 * 2)
                    {
                        if (i % 2 == 0)
                        {
                            packed = (x << 4) | (buffer[i + 1] >> 4);
                        }
                        else
                        {
                            packed = ((x & 0b_001_111) << 8) | (buffer[i + 1]);
                        }
                    }

                    return packed;
                }).ToArray();
            }

            bufferPointer = 0;

            TransferRequest = true;
        }

        public void TransferDataRegister()
        {
            switch (State)
            {
                case ControllerState.Idle:
                    break;
                case ControllerState.FillBuffer:
                    throw new NotImplementedException();
                    break;
                case ControllerState.EmptyBuffer:
                    EmptyBuffer();
                    break;
                case ControllerState.WriteSector:
                    throw new NotImplementedException();
                    break;
                case ControllerState.ReadSector:
                    throw new NotImplementedException();
                    break;
                case ControllerState.WriteDeletedDataSector:
                    throw new NotImplementedException();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void EmptyBuffer()
        {
            if (EightBitMode)
            {
                throw new NotImplementedException();
            }

            accumulator.SetAccumulator(buffer[++bufferPointer]);

            if (bufferPointer == buffer.Length)
            {
                SetDone();
            }

            TransferRequest = true;
        }

        public bool SkipNotDone()
        {
            State = ControllerState.EmptyBuffer;

            if (Done)
            {
                ClearDone();

                return true;
            }

            return false;
        }
    }
}