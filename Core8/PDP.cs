using Core8.Extensions;
using Core8.Interfaces;
using Serilog;
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

            var paperTape = new Teletype();
            Reader = paperTape;
            Punch = paperTape;

            processor = new Processor(Memory, Registers, Reader, Punch);
        }

        public IKeyboard Reader { get; }

        public ITeleprinter Punch { get; }

        public IRegisters Registers { get; }

        public IMemory Memory { get; }

        public void Deposit8(uint data)
        {
            Deposit10(data.ToDecimal());
        }

        public void Deposit10(uint data)
        {
            Memory.Write(Registers.IF_PC.Address, data & Masks.MEM_WORD);

            Log.Information($"DEP: {Registers.IF_PC.Address.ToOctalString()} {data.ToOctalString()}");

            Registers.IF_PC.Increment();
        }

        public void Load8(uint address = 0)
        {
            Load10(address.ToDecimal());
        }

        public void Load10(uint address)
        {
            Registers.IF_PC.Set(address);

            Log.Information($"LOAD: {address.ToOctalString()}");
        }

        public void Exam()
        {
            Registers.LINK_AC.SetAccumulator(Memory.Read(Registers.IF_PC.Address));

            Log.Information($"EXAM: {Registers.LINK_AC.ToString()}");
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

            Log.Information($"TAPE: loaded {tape.Length} bytes");
        }
    }
}
