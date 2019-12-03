using Core8.Enums;
using Core8.Extensions;
using Core8.Instructions.Abstract;
using Core8.Interfaces;
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

                var opCode = (data & Masks.OP_CODE);

                var instructionName = (InstructionName)opCode;

                InstructionBase instruction;

                switch (instructionName)
                {
                    case InstructionName.Microcoded:
                        instruction = Decoder.DecodeMicrocode(data);
                        break;
                    case InstructionName.PaperTape:
                        instruction = Decoder.DecodePaperTape(data);
                        break;
                    default:
                        instruction = Decoder.DecodeMemoryReference(instructionName, data);
                        break;
                }

                Trace.WriteLine($"{environment.Registers.IF_PC.Address.ToOctalString()}: {instruction}");

                environment.Registers.IF_PC.Increment();

                instruction.Execute(environment);

                environment.Reader.Tick();
            }
        }
    }
}
