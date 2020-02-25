using Core8.Model.Interfaces;
using Core8.Model.Register;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading;

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
            NoOperation,
            WriteDeletedDataSector,
        }

        private volatile ControllerState state;

        private volatile int commandRegister;

        private volatile bool done;

        private volatile bool transferRequest;

        private int[] buffer;

        private readonly LinkAccumulator accumulator;

        private volatile int bufferPointer;

        private byte[] disk;

        private Thread controllerThread;

        private AutoResetEvent commandIssued = new AutoResetEvent(false);

        private bool controllerRunning;

        public FloppyDrive(LinkAccumulator accumulator)
        {
            this.accumulator = accumulator;

            state  = ControllerState.Idle;

            controllerThread = new Thread(Control)
            {
                IsBackground = true,
                Priority = ThreadPriority.AboveNormal
            };

            controllerThread.Start();
        }

        private void Control()
        {
            controllerRunning = true;

            while (controllerRunning)
            {
                if (commandIssued.WaitOne(TimeSpan.FromMilliseconds(200)))
                {
                    try
                    {
                        ExecuteCommand();
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "Caught Exception while executing command.");

                        throw;
                    }                    
                }
            }
        }

        private void ExecuteCommand()
        {
            Log.Information($"Executing: {State}");

            switch (State)
            {
                case ControllerState.Initialize:
                    Initialize();
                    break;
                case ControllerState.FillBuffer:
                    FillBuffer();
                    break;
                case ControllerState.EmptyBuffer:
                    EmptyBuffer();
                    break;
                case ControllerState.WriteSector:
                    WriteSector();
                    break;
                case ControllerState.WriteTrack:
                    WriteTrack();
                    break;
                case ControllerState.ReadSector:
                    ReadSector();
                    break;
                case ControllerState.ReadTrack:
                    ReadTrack();
                    break;
                case ControllerState.NoOperation:
                    NoOperation();
                    break;
                case ControllerState.WriteDeletedDataSector:
                    throw new NotImplementedException();
                    break;
                default:
                    throw new NotImplementedException();
            }

        }

        public ControllerState State => state;


        private int Function => (commandRegister & 0b_001_110) >> 1;

        private ControllerFunction FunctionSelect => (ControllerFunction)Function;

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
            InterruptRequested = done = true;
        }

        public void ClearTransferRequest()
        {
            transferRequest = false;
        }

        private void SetTransferRequest()
        {
            transferRequest = true;

            Log.Information("TRQ set");
        }

        public void Load(byte[] disk)
        {
            this.disk = disk;

            state = ControllerState.Initialize;

            commandIssued.Set();
        }

        public void LoadCommandRegister(int data)
        {
            if (State != ControllerState.Idle)
            {
                return;
            }

            ClearDone();

            commandRegister = data & 0b_000_011_111_110;

            Log.Information($"Function select: {FunctionSelect}");

            switch (Function)
            {
                case FILL_BUFFER:
                    state = ControllerState.FillBuffer;
                    bufferPointer = 0;
                    SetTransferRequest();
                    break;
                case EMPTY_BUFFER:
                    state = ControllerState.EmptyBuffer;
                    bufferPointer = 0;
                    SetTransferRequest();
                    break;
                case WRITE_SECTOR:
                    state = ControllerState.WriteSector;
                    SetTransferRequest();
                    break;
                case READ_SECTOR:
                    state = ControllerState.ReadSector;
                    SetTransferRequest();
                    break;
                case NO_OPERATION:
                    state = ControllerState.NoOperation;
                    break;
                case READ_STATUS:
                    ReadStatus();
                    break;
                case WRITE_DELETED_DATA_SECTOR:
                    state = ControllerState.WriteDeletedDataSector;
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
            if (EightBitMode)
            {
                throw new NotImplementedException();
            }
               
            buffer = new int[128];

            Array.Copy(disk, TrackAddress * 26 * 128 + (SectorAddress - 1) * 128, buffer, 0, buffer.Length);            
            
            var packed = new List<int>();

            for (int i = 0; i < 96; i++)
            {
                if ((i + 1) % 3 != 0)
                {
                    int word;

                    if (packed.Count % 2 == 0)
                    {
                        word = (buffer[i] << 4) | ((buffer[i + 1] >> 4) & 0b_001_111);
                    }
                    else
                    {
                        word = ((buffer[i] & 0b_001_111) << 8) | (buffer[i + 1]);
                    }

                    packed.Add(word);
                }
            }

            buffer = packed.ToArray();            
        }

        private void WriteBlock()
        {
            if (EightBitMode)
            {
                throw new NotImplementedException();
            }

            var buffer = new byte[128];

            var position = 0;

            for (int i = 0; i < 64; i++)
            {
                if (i % 2 == 0)
                {
                    buffer[position++] = (byte)(buffer[i] >> 4);
                    buffer[position++] = (byte)((buffer[i] << 4) | ((buffer[i + 1] >> 8) & 0b_001_111));                        
                }
                else
                {
                    buffer[position++] = (byte)(buffer[i] & 0b_011_111_111);
                }                
            }

            Array.Copy(buffer,0, disk, TrackAddress * 26 * 128 + (SectorAddress - 1) * 128, buffer.Length);            
        }

        public void TransferDataRegister()
        {
            Log.Information("XDR");

            commandIssued.Set();
        }

        public void Initialize()
        {
            TrackAddress = 1;
            SectorAddress = 1;

            ReadBlock();            

            SetTransferRequest();

            SetDone();

            state = ControllerState.Idle;
        }

        private void ReadSector()
        {
            SectorAddress = accumulator.Accumulator & 0b_000_001_111_111;

            state = ControllerState.ReadTrack;

            SetTransferRequest();
        }

        private void ReadTrack()
        {
            TrackAddress = accumulator.Accumulator & 0b_000_011_111_111;

            ReadBlock();

            state = ControllerState.Idle;            

            SetDone();
        }

        private void NoOperation()
        {
            SetDone();
        }

        private void WriteSector()
        {
            SectorAddress = accumulator.Accumulator & 0b_000_001_111_111;

            state = ControllerState.WriteTrack;

            SetTransferRequest();
        }

        private void WriteTrack()
        {
            TrackAddress = accumulator.Accumulator & 0b_000_011_111_111;

            WriteBlock();

            state = ControllerState.Idle;

            SetDone();
        }

        private void FillBuffer()
        {
            if (EightBitMode)
            {
                throw new NotImplementedException();
            }

            buffer[bufferPointer++] = accumulator.Accumulator;

            if (bufferPointer == 64)
            {
                state = ControllerState.Idle; 
                
                SetDone();                
            }
            else
            {
                transferRequest = true;
            }
        }

        private void EmptyBuffer()
        {
            if (EightBitMode)
            {
                throw new NotImplementedException();
            }

            accumulator.SetAccumulator(buffer[bufferPointer++]);

            if (bufferPointer == 64)
            {
                SetDone();

                state = ControllerState.Idle;
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