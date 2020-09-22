using Core8.Model.Registers;

namespace Core8.Model.Interfaces
{
    public interface ICPU
    {
        void Run();

        void Halt();

        void Clear();

        void SetBreakpoint(int address);

        void RemoveBreakpoint(int address);

        void RemoveAllBreakpoints();

        IInstruction Debug8(int address);

        IInstruction Debug10(int address);

        void SingleStep(bool state);

        void Debug(bool state);

        IInterrupts Interrupts { get; }

        IMemory Memory { get; }

        ITeletype Teletype { get; }

        IFloppyDrive FloppyDrive { get; }

        LinkAccumulator AC { get; }

        InstructionFieldProgramCounter PC { get; }

        SwitchRegister SR { get; }

        MultiplierQuotient MQ { get; }

        DataField DF { get; }

        InstructionBuffer IB { get; }

        UserBuffer UB { get; }

        UserFlag UF { get; }

        SaveField SF { get; }

    }
}
