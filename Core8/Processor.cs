using Core8.Extensions;
using Core8.Interfaces;
using Serilog;
using System.Threading;

namespace Core8
{
    public class Processor : IProcessor
    {
        private volatile bool halted;


        public Processor(IMemory memory, IRegisters registers, IKeyboard keyboard, ITeleprinter teleprinter)
        {
            Hardware = new Hardware(this, memory, registers, keyboard, teleprinter);
        }

        public IHardware Hardware { get; }

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
                    Log.Warning($"[{address.ToOctalString()}] NOP {data.ToOctalString()}");
                }

                Hardware.Tick();

                Thread.Sleep(0);
            }

            Log.Information("HLT");
        }


    }
}
