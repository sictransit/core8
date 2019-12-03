using Core8.Interfaces;

namespace Core8
{
    public class Environment : IEnvironment
    {
        public Environment(IProcessor processor, IMemory memory, IRegisters registers, IReader reader, IPunch punch)
        {
            Memory = memory;
            Processor = processor;
            Registers = registers;
            Reader = reader;
            Punch = punch;
        }

        public IMemory Memory { get; }

        public IProcessor Processor { get; }

        public IRegisters Registers { get; }

        public IReader Reader { get; }

        public IPunch Punch { get; }
    }
}
