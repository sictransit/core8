using System;

namespace Core8.Model.Interfaces
{
    public interface ICPU
    {
        IInterrupts Interrupts { get; }

        IMemory Memory { get; }

        ITeletype Teletype { get; }

        IFloppyDrive FloppyDrive { get; }

        IRegistry Registry { get; }

        IInstruction Fetch(int address);

        IInstruction Instruction { get; }

        void Run();

        void Halt();

        void Clear();

        void SetBreakpoint(Func<ICPU, bool> breakpoint);

        void SingleStep(bool state);

        void Debug(bool state);

        int InstructionCounter { get; }
    }
}
