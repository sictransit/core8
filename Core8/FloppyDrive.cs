using Core8.Model.Interfaces;
using Core8.Model.Register;
using Serilog;
using System;
using System.Collections.Generic;
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
            ReadTrack,
            WriteDeletedDataSector,
        }

        private int commandRegister;

        private volatile bool done;

        private volatile bool transferRequest;

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

        public bool TransferRequest => transferRequest;

        public bool InterruptRequested { get; private set; }

        public int SectorAddress { get; private set; }

        public int TrackAddress { get; private set; }

        public bool Error { get; private set; }

        public void ClearError()
        {
            Error = false;
        }

        public void ClearDone()
        {
            InterruptRequested = done = false;
        }

        private void SetDone()
        {
            done = true;
        }

        public void ClearTransferRequest()
        {
            transferRequest = false;
        }

        public void Load(byte[] disk)
        {
            this.disk = disk;

            TrackAddress = 1;
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

            ClearDone();

            ClearTransferRequest();

            commandRegister = data & 0b_000_011_111_110;

            //ExecuteCommand();

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
            Array.Copy(disk, TrackAddress * 26 * 128 + (SectorAddress - 1) * 128, buffer, 0, buffer.Length);

            if (!EightBitMode)
            {
                var packedBuffer = new List<int>();

                for (int i = 0; i < 96; i++)
                {
                    if ((i + 1) % 3 != 0)
                    {
                        int packed;

                        if (packedBuffer.Count % 2 == 0)
                        {
                            packed = (buffer[i] << 4) | ((buffer[i + 1] >> 4) & 0b_001_111);
                        }
                        else
                        {
                            packed = ((buffer[i] & 0b_001_111) << 8) | (buffer[i + 1]);
                        }

                        packedBuffer.Add(packed);
                    }
                }

                buffer = packedBuffer.ToArray();
            }

            bufferPointer = 0;

            transferRequest = true;
        }

        public void TransferDataRegister()
        {
            Log.Information(State.ToString());

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
                    ReadSector();
                    break;
                case ControllerState.ReadTrack:
                    ReadTrack();
                    break;
                case ControllerState.WriteDeletedDataSector:
                    throw new NotImplementedException();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void ReadSector()
        {
            SectorAddress = accumulator.Accumulator & 0b_000_001_111_111;

            State = ControllerState.ReadTrack;
        }

        private void ReadTrack()
        {
            TrackAddress = accumulator.Accumulator & 0b_000_011_111_111;

            State = ControllerState.Idle;

            SetDone();
        }

        private void EmptyBuffer()
        {
            if (EightBitMode)
            {
                throw new NotImplementedException();
            }

            accumulator.SetAccumulator(buffer[bufferPointer++]);

            if (bufferPointer == buffer.Length)
            {
                SetDone();

                State = ControllerState.Idle;
            }
            else
            {
                transferRequest = true;
            }
        }

        public bool SkipNotDone()
        {
            if (Done)
            {
                ClearDone();

                return true;
            }

            return false;
        }
    }
}