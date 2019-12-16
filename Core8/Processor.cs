using Core8.Extensions;
using Core8.Interfaces;
using Serilog;
using System.Threading;

namespace Core8
{
    public class Processor : IProcessor
    {
        private readonly ManualResetEvent running = new ManualResetEvent(false);

        public Processor(IMemory memory, IRegisters registers, IKeyboard keyboard, ITeleprinter teleprinter)
        {
            Hardware = new Hardware(this, memory, registers, keyboard, teleprinter);
        }

        public IHardware Hardware { get; }

        public void Halt()
        {
            running.Reset();
        }

        public void Run()
        {
            running.Set();

            Log.Information("RUN");

            while (running.WaitOne(0))
            {
                var address = Hardware.Registers.IF_PC.Address;

                var data = Hardware.Memory.Read(address);

                Hardware.Registers.IF_PC.Increment();

                if (Decoder.TryDecode(address, data, out var instruction))
                {
                    instruction.Execute(Hardware);

                    Log.Debug(instruction.ToString());
                }
                else
                {
                    Log.Debug($"[{address.ToOctalString()}] NOP {data.ToOctalString()}");
                }

                Hardware.Tick();
            }

            Log.Information("HLT");
        }


    }
}
