using Core8.Model;
using Core8.Model.Extensions;
using Core8.Model.Interfaces;
using Serilog;
using System;
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
            Keyboard = new Keyboard(3);
            Teleprinter = new Teleprinter(4);

            processor = new Processor(Memory, Registers, Keyboard, Teleprinter);
        }

        public IKeyboard Keyboard { get; }

        public ITeleprinter Teleprinter { get; }

        public IRegisters Registers { get; }

        public IMemory Memory { get; }

        public void Deposit8(uint data)
        {
            Deposit10(data.ToDecimal());
        }

        public void Deposit10(uint data)
        {
            Registers.Switch.Set(data);

            Deposit();
        }

        private void Deposit()
        {
            var data = Registers.Switch.Get;

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
            Registers.Switch.Set(address);

            Load();
        }

        private void Load()
        {
            var address = Registers.Switch.Get;

            Registers.IF_PC.Set(address);

            Log.Information($"LOAD: {address.ToOctalString()}");
        }

        public void Toggle8(uint word)
        {
            Toggle10(word.ToDecimal());
        }

        public void Toggle10(uint word)
        {
            Registers.Switch.Set(word);
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
                throw new ArgumentNullException(nameof(tape));
            }

            foreach(var b in tape)
            { 
                Keyboard.Input(b);
            }

            Log.Information($"TAPE: loaded {tape.Length} bytes");
        }
    }
}
