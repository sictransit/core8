using Core8.Extensions;
using Core8.Interfaces;
using Serilog;
using System;
using System.Diagnostics;
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

                if (Decoder.TryDecode(data, out var instruction))
                {
                    Log.Debug($"{CurrentAddress.ToOctalString()}: {instruction}");

                    environment.Registers.IF_PC.Increment();

                    instruction.Execute(environment);                    
                }
                else
                {
                    var debugInformation = $"{CurrentAddress.ToOctalString()}: {data.ToOctalString()}";

                    Log.Error($"Not implemented: {debugInformation}");

                    throw new NotImplementedException(debugInformation);
                }

                environment.Keyboard.Tick();

                Thread.Sleep(0);
            }

            Log.Information("HLT");
        }
    }
}
