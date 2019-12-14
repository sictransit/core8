using Core8.Extensions;
using Core8.Instructions;
using Core8.Instructions.Abstract;
using Core8.Interfaces;
using Serilog;
using System.Threading;

namespace Core8
{
    public partial class Processor : IProcessor
    {
        private readonly IEnvironment environment;
        private volatile bool halted;
        private readonly TeleprinterInstructions teleprinterInstructions;
        private readonly KeyboardInstructions keyboardInstructions;
        private readonly GroupOneMicrocodedInstructions groupOneMicrocodedInstructions;
        private readonly GroupTwoAndMicrocodedInstructions groupTwoAndMicrocodedInstructions;
        private readonly GroupTwoOrMicrocodedInstructions groupTwoOrMicrocodedInstructions;
        private readonly PrivilegedGroupTwoMicrocodedInstructions privilegedGroupTwoMicrocodedInstructions;
        private readonly MemoryReferenceInstructions memoryReferenceInstructions;

        public Processor(IMemory memory, IRegisters registers, IKeyboard keyboard, ITeleprinter teleprinter)
        {
            environment = new Environment(this, memory, registers, keyboard, teleprinter);

            teleprinterInstructions = new TeleprinterInstructions(registers, teleprinter);
            keyboardInstructions = new KeyboardInstructions(registers, keyboard);
            groupOneMicrocodedInstructions = new GroupOneMicrocodedInstructions(registers);
            groupTwoAndMicrocodedInstructions = new GroupTwoAndMicrocodedInstructions(registers);
            groupTwoOrMicrocodedInstructions = new GroupTwoOrMicrocodedInstructions(registers);
            privilegedGroupTwoMicrocodedInstructions = new PrivilegedGroupTwoMicrocodedInstructions(this, registers);
            memoryReferenceInstructions = new MemoryReferenceInstructions(this, registers, memory);
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

                if (!Execute(data))
                {
                    Log.Warning($"Not implemented: {CurrentAddress.ToOctalString()}: {data.ToOctalString()}");
                }


                environment.Registers.IF_PC.Increment();

                environment.Tick();

                Thread.Sleep(0);
            }

            Log.Information("HLT");
        }
    }
}
