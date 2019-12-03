using Core8.Extensions;
using Core8.Interfaces;
using System;
using System.Diagnostics;

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
                var data = environment.Memory.Read(environment.Registers.IF_PC.Address);

                if (Decoder.TryDecode(data, out var instruction))
                {
                    Trace.WriteLine($"{environment.Registers.IF_PC.Address.ToOctalString()}: {instruction}");

                    environment.Registers.IF_PC.Increment();

                    instruction.Execute(environment);
                }
                else
                {
                    throw new NotImplementedException($"{environment.Registers.IF_PC.Address.ToOctalString()}: {data.ToOctalString()}");
                }

                environment.Reader.Tick();
            }

            Trace.WriteLine("HLT");
        }
    }
}
