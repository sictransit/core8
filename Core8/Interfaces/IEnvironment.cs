namespace Core8.Interfaces
{
    public interface IEnvironment
    {
        IMemory Memory { get; }

        IProcessor Processor { get; }

        IRegisters Registers { get; }

        IReader Reader { get; }

        IPunch Punch { get; }
    }
}
