using Core8.Extensions;
using Core8.Interfaces;
using System.Diagnostics;
using System.Threading;

namespace Core8
{
    public class PDP
    {
        private Thread cpuThread;

        private readonly IProcessor processor;

        public PDP()
        {
            Memory = new Memory(4096);
            Registers = new Registers();

            var paperTape = new PaperTape();
            Reader = paperTape;
            Punch = paperTape;

            processor = new Processor(Memory, Registers, Reader, Punch);
        }

        public IReader Reader { get; }

        public IPunch Punch { get; }

        public IRegisters Registers { get; }

        public IMemory Memory { get; }

        public void Deposit8(uint data)
        {
            Deposit10(data.ToDecimal());
        }

        public void Deposit10(uint data)
        {
            Memory.Write(Registers.IF_PC.Address, data & Masks.MEM_WORD);

            Trace.WriteLine($"DEP: {Registers.IF_PC.Address.ToOctalString()} {data.ToOctalString()}");

            Registers.IF_PC.Increment();
        }

        public void Load8(uint address = 0)
        {
            Load10(address.ToDecimal());
        }

        public void Load10(uint address)
        {
            Registers.IF_PC.Set(address);

            Trace.WriteLine($"LOAD: {address.ToOctalString()}");
        }

        public void Exam()
        {
            Registers.LINK_AC.SetAccumulator(Memory.Read(Registers.IF_PC.Address));

            Trace.WriteLine($"EXAM: {Registers.LINK_AC.ToString()}");
        }

        public void Start(bool waitForHalt = true)
        {
            cpuThread = new Thread(processor.Run);

            cpuThread.Start();

            if (waitForHalt)
            {
                cpuThread.Join();
            }
        }

        public void Stop()
        {
            processor.Halt();
        }

        public void LoadTape(byte[] tape)
        {
            if (tape is null)
            {
                throw new System.ArgumentNullException(nameof(tape));
            }

            Reader.Load(tape);

            Trace.WriteLine($"TAPE: loaded {tape.Length} bytes");
        }
    }
}
