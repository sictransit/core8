using Core8.Model.Interfaces;
using Core8.Model.Register;
using System;

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

        private const int MIN_TRACK = 0;
        private const int MAX_TRACK = 76;
        private const int MIN_SECTOR = 1; 
        private const int MAX_SECTOR = 26;
        private const int BLOCK_SIZE = 128;

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
            FillBuffer,
            EmptyBuffer,
            WriteSector,
            WriteTrack,
            ReadSector,
            ReadTrack,
            WriteDeletedDataSector,
        }

        private bool sdnWait;

        private int commandRegister;

        private int interfaceRegister;

        private readonly int[] buffer = new int[64];

        //private readonly LinkAccumulator accumulator;

        private int bufferPointer;

        private readonly byte[][] disk = new byte[2][];

        public FloppyDrive(LinkAccumulator accumulator)
        {
            //this.accumulator = accumulator;            
        }

        private ControllerState state;

        private int Function => (commandRegister & 0b_001_110) >> 1;

        private ControllerFunction FunctionSelect => (ControllerFunction)Function;

        private bool EightBitMode => (commandRegister & 0b_001_000_000) != 0;

        private int UnitSelect => (commandRegister >> 4) & 0b_001;

        public bool Done { get; private set; }

        public bool TransferRequest { get; private set; }

        public bool InterruptRequested { get; private set; }

        private int sectorAddress;

        private int trackAddress;

        private int BlockAddress => trackAddress * MAX_SECTOR * BLOCK_SIZE + (sectorAddress - 1) * BLOCK_SIZE;

        public bool Error { get; private set; }

        private void SetTrackAddress(int address)
        {
            if (address < MIN_TRACK || address > MAX_TRACK)
            {
                throw new ArgumentOutOfRangeException(nameof(address), address.ToString());
            }

            trackAddress = address;
        }

        private void SetSectorAddress(int address)
        {
            if (address < MIN_SECTOR || address > MAX_SECTOR)
            {
                throw new ArgumentOutOfRangeException(nameof(address), address.ToString());
            }

            sectorAddress = address;
        }

        public void ClearError()
        {
            Error = false;
        }

        public void ClearDone()
        {            
            Done = false;
        }

        private void SetDone()
        {            
            Done = true;
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

            Initialize();            
        }

        public void LoadCommandRegister(int data)
        {
            if (state != ControllerState.Idle)
            {
                return;
            }

            ClearDone();

            commandRegister = data & 0b_000_011_111_110;

            switch (Function)
            {
                case FILL_BUFFER:
                    SetState(ControllerState.FillBuffer);
                    bufferPointer = 0;
                    SetTransferRequest();
                    break;
                case EMPTY_BUFFER:
                    SetState(ControllerState.EmptyBuffer);
                    bufferPointer = 0;
                    SetTransferRequest();
                    break;
                case WRITE_SECTOR:
                    SetState(ControllerState.WriteSector);
                    SetTransferRequest();
                    break;
                case READ_SECTOR:
                    SetState(ControllerState.ReadSector);
                    SetTransferRequest();
                    break;
                case NO_OPERATION:
                    SetDone();
                    break;
                case READ_STATUS:
                    throw new NotImplementedException();
                case WRITE_DELETED_DATA_SECTOR:
                    SetState(ControllerState.WriteDeletedDataSector);
                    break;
                case READ_ERROR_REGISTER:
                    throw new NotImplementedException();
                default:
                    throw new NotImplementedException();
            }
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

            for (int i = 0; i < 64; i++)
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

        public void TransferDataRegister(LinkAccumulator linkAc)
        {
            if (sdnWait)
            {
                linkAc.SetAccumulator(interfaceRegister);

                sdnWait = false;
            }

            switch (state)
            {
                case ControllerState.FillBuffer:
                    interfaceRegister = linkAc.Accumulator;
                    FillBuffer();
                    break;
                case ControllerState.EmptyBuffer:
                    EmptyBuffer();
                    linkAc.SetAccumulator(interfaceRegister);
                    break;
                case ControllerState.WriteSector:
                    interfaceRegister = linkAc.Accumulator;
                    WriteSector();
                    break;
                case ControllerState.WriteTrack:
                    interfaceRegister = linkAc.Accumulator;
                    WriteTrack();
                    break;
                case ControllerState.ReadSector:
                    interfaceRegister = linkAc.Accumulator;
                    ReadSector();
                    break;
                case ControllerState.ReadTrack:
                    interfaceRegister = linkAc.Accumulator;
                    ReadTrack();
                    break;
                case ControllerState.WriteDeletedDataSector:
                    throw new NotImplementedException();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public void Initialize()
        {
            SetTrackAddress(1);
            SetSectorAddress(1);

            ReadBlock();

            SetTransferRequest();

            SetDone();

            SetState(ControllerState.Idle);
        }

        private void ReadSector()
        {
            SetSectorAddress(interfaceRegister & 0b_000_001_111_111);

            SetState(ControllerState.ReadTrack);

            SetTransferRequest();
        }

        private void ReadTrack()
        {
            SetTrackAddress(interfaceRegister & 0b_000_011_111_111);

            ReadBlock();

            SetState(ControllerState.Idle);

            SetDone();
        }

        private void WriteSector()
        {
            SetSectorAddress(interfaceRegister & 0b_000_001_111_111);

            SetState(ControllerState.WriteTrack);

            SetTransferRequest();
        }

        private void WriteTrack()
        {
            SetTrackAddress(interfaceRegister & 0b_000_011_111_111);

            WriteBlock();

            SetState(ControllerState.Idle);

            SetDone();
        }

        private void FillBuffer()
        {
            if (EightBitMode)
            {
                throw new NotImplementedException();
            }

            buffer[bufferPointer++] = interfaceRegister;

            if (bufferPointer == 64)
            {
                SetState(ControllerState.Idle);

                SetDone();
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

            if (bufferPointer == 64)
            {
                SetDone();

                SetState(ControllerState.Idle);
            }
            else
            {
                SetTransferRequest();
            }
        }

        public bool SkipNotDone()
        {
            if (Done)
            {
                ClearDone();

                return true;
            }

            sdnWait = true;

            return false;
        }

        public override string ToString()
        {
            return $"[RX01] func={FunctionSelect} mode={(EightBitMode?8:12)} unit={UnitSelect} trk={trackAddress} sec={sectorAddress}";
        }

    }
}