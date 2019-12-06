using Core8.Extensions;
using Core8.Instructions;
using Core8.Instructions.Abstract;
using Core8.Interfaces;
using Serilog;
using System.Threading;

namespace Core8
{
    public class Processor : IProcessor
    {
        private readonly IEnvironment environment;
        private volatile bool halted;

        public Processor(IMemory memeory, IRegisters registers, IKeyboard keyboard, ITeleprinter teleprinter)
        {
            environment = new Environment(this, memeory, registers, keyboard, teleprinter);
        }

        public uint CurrentAddress { get; private set; }

        public void Halt()
        {
            halted = true;
        }

        public void Run()
        {
            halted = false;

            Log.Information("RUN");

            while (!halted)
            {
                CurrentAddress = environment.Registers.IF_PC.Address;

                var data = environment.Memory.Read(CurrentAddress);

                InstructionBase instruction;

                if (Decoder.TryDecode(data, out instruction))
                {
                    Log.Debug($"{CurrentAddress.ToOctalString()}: {instruction}");
                }
                else
                {
                    Log.Warning($"Not implemented: {CurrentAddress.ToOctalString()}: {data.ToOctalString()}");

                    instruction = new NOP(data);
                }

                environment.Registers.IF_PC.Increment();

                instruction.Execute(environment);

                environment.Tick();

                Thread.Sleep(0);
            }

            Log.Information("HLT");
        }
    }
}
