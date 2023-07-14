using System;

namespace Core8.Model.Interfaces
{
    public interface ICPU
    {
        IInstructionSet InstructionSet { get; }

        IInterrupts Interrupts { get; }

        IMemory Memory { get; }

        ITeletype Teletype { get; }

        IFloppyDrive FloppyDrive { get; }

        IRegistry Registry { get; }

        void Run();

        void Halt();

        void Clear();

        void SetBreakpoint(Func<ICPU, bool> breakpoint);

        IInstruction Debug8(int address);

        IInstruction Debug10(int address);

        void SingleStep(bool state);

        void Debug(bool state);

        int InstructionCounter { get; }
    }
}
