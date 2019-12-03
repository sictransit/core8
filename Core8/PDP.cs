using Core8.Extensions;
using Core8.Interfaces;
using System.Diagnostics;
using System.Threading;

namespace Core8
{
    public class PDP : IProcessor
    {
        private readonly Memory ram;
        private readonly Registers registers;
        private readonly PaperTape paperTape;

        private Thread cpuThread;

        public PDP(Memory memory)
        {
            ram = memory;
            registers = new Registers();
            paperTape = new PaperTape();
        }

        public uint ProgramCounterWord => registers.IF_PC.Word;

        public uint Accumulator => registers.LINK_AC.Accumulator;

        public uint Link => registers.LINK_AC.Link;

        public bool Halted { get; private set; }

        public void Deposit8(uint data)
        {
            Deposit10(data.ToDecimal());
        }

        public void Deposit10(uint data)
        {
            ram.Write(registers.IF_PC.Address, data & Masks.MEM_WORD);

            Trace.WriteLine($"DEP: {registers.IF_PC.Address.ToOctalString()} {data.ToOctalString()}");

            registers.IF_PC.Increment();
        }

        public void Halt()
        {
            Halted = true;
        }

        public void Load8(uint address = 0)
        {
            Load10(address.ToDecimal());
        }

        public void Load10(uint address)
        {
            registers.IF_PC.Set(address);

            Trace.WriteLine($"LOAD: {address.ToOctalString()}");
        }

        public void Exam()
        {
            registers.LINK_AC.SetAccumulator(ram.Read(registers.IF_PC.Address));

            Trace.WriteLine($"EXAM: {registers.LINK_AC.ToString()}");
        }

        public void Start(bool waitForHalt = true)
        {
            var processor = new Processor(ram, registers, paperTape, paperTape);

            cpuThread = new Thread(processor.Run);

            cpuThread.Start();

            if (waitForHalt)
            {
                cpuThread.Join();
            }
        }
    }
}
