namespace Core8.Model.Interfaces;

public interface ICPU
{
    IInterrupts Interrupts { get; }

    IMemory Memory { get; }

    IKeyboardReader KeyboardReader { get; }

    IPrinterPunch PrinterPunch { get; }

    ILinePrinter LinePrinter { get; }

    IRX8E RX8E { get; }

    IRK8E RK8E { get; }

    IRegistry Registry { get; }

    IInstruction Fetch(int address);

    IInstruction Instruction { get; }

    void Run();

    void Halt();

    void Clear();

    void SetBreakpoint(Breakpoint breakpoint);

    ulong InstructionCounter { get; }

    void Attach(IRK8E peripheral);

    void Attach(IRX8E peripheral);

    void Attach(IPrinterPunch peripheral);

    void Attach(IKeyboardReader peripheral);

    void Attach(ILinePrinter peripheral);
}
