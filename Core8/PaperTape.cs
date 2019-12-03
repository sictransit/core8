using Core8.Interfaces;
using System;
using System.Threading;

namespace Core8
{
    public class PaperTape : IReader, IPunch
    {
        private readonly ManualResetEvent readerFlag = new ManualResetEvent(false);

        private volatile uint buffer;

        public uint Buffer => buffer & Masks.READER_BUFFER_MASK;

        public uint ReaderFlag => readerFlag.WaitOne(TimeSpan.Zero) ? 0u : 1u;

        public bool IsReaderFlagSet => ReaderFlag == 1u;

        public void ClearReaderFlag()
        {
            readerFlag.Reset();
        }

        public void Load(byte[] data)
        { 

        }
    }
}
