using Core8.Extensions;
using Core8.Interfaces;
using System;
using System.Diagnostics;
using System.Threading;

namespace Core8
{
    public class Processor : IProcessor
    {
        private readonly IEnvironment environment;
        private volatile bool halted;

        public Processor(IMemory memeory, IRegisters registers, IReader reader, IPunch punch)
        {
            environment = new Environment(this, memeory, registers, reader, punch);
        }

        public uint CurrentAddress { get; private set; }

        public void Halt()
        {
            halted = true;
        }

        public void Run()
        {
            halted = false;

            Trace.WriteLine("RUN");

            while (!halted)
            {
                CurrentAddress = environment.Registers.IF_PC.Address;

                var data = environment.Memory.Read(CurrentAddress);

                if (Decoder.TryDecode(data, out var instruction))
                {
                    Trace.WriteLine($"{CurrentAddress.ToOctalString()}: {instruction}");

                    environment.Registers.IF_PC.Increment();

                    instruction.Execute(environment);                    
                }
                else
                {
                    throw new NotImplementedException($"{CurrentAddress.ToOctalString()}: {data.ToOctalString()}");
                }

                environment.Reader.Tick();

                Thread.Sleep(0);
            }

            Trace.WriteLine("HLT");
        }
    }
}
