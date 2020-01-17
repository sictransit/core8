using Core8.Model;
using Core8.Model.Extensions;
using Core8.Model.Instructions;
using Core8.Model.Interfaces;
using Serilog;
using System;
using System.Threading;

namespace Core8
{
    public class PDP
    {
        private Thread cpuThread;



        public PDP()
        {
            Memory = new Memory(4096);
            Registers = new Registers();
            Teleprinter = new Teleprinter();

            Processor = new Processor(Memory, Registers, Teleprinter);
        }

        public IProcessor Processor { get; }

        public ITeleprinter Teleprinter { get; }

        public IRegisters Registers { get; }

        public IMemory Memory { get; }

        public void DumpMemory()
        {
            var instructionFactory = new InstructionFactory(Processor, Memory, Registers, Teleprinter);

            for (uint address = 0; address < Memory.Size; address++)
            {
                var data = Memory.Examine(address);

                if (instructionFactory.TryFetch(address, data, out var instruction))
                {
                    Log.Information($"{instruction}");
                }
                else
                {
                    Log.Information($"{address.ToOctalString()}:{data.ToOctalString()}");
                }
            }
        }

        public void Clear()
        {
            Processor.Clear();
        }

        public void Deposit8(uint data)
        {
            Deposit10(data.ToDecimal());
        }

        public void Deposit10(uint data)
        {
            Registers.SR.Set(data);

            Deposit();
        }

        private void Deposit()
        {
            var data = Registers.SR.Get;

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
            Registers.SR.Set(address);

            Load();
        }

        private void Load()
        {
            var address = Registers.SR.Get;

            Registers.IF_PC.Set(address);

            Log.Information($"LOAD: {address.ToOctalString()}");
        }

        public void Toggle8(uint word)
        {
            Toggle10(word.ToDecimal());
        }

        public void Toggle10(uint word)
        {
            Registers.SR.Set(word);
        }

        public void Exam()
        {
            Registers.LINK_AC.SetAccumulator(Memory.Read(Registers.IF_PC.Address));

            Log.Information($"EXAM: {Registers.LINK_AC.ToString()}");
        }

        public void Start(bool waitForHalt = true)
        {
            cpuThread = new Thread(Processor.Run)
            {
                IsBackground = true,
                Priority = ThreadPriority.AboveNormal
            };

            cpuThread.Start();

            if (waitForHalt)
            {
                cpuThread.Join();
            }
        }

        public void Stop()
        {
            Processor.Halt();
        }

        public void LoadTape(byte[] tape)
        {
            if (tape is null)
            {
                throw new ArgumentNullException(nameof(tape));
            }

            Teleprinter.Read(tape);

            Log.Information($"TAPE: loaded {tape.Length} bytes");
        }
    }
}
