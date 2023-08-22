using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using Serilog;
using System;

namespace Core8.Model.Instructions;

public class RX8EInstructions : PrivilegedInstructionsBase
{
    private const int SEL_MASK = 0b_000;
    private const int LCD_MASK = 0b_001;
    private const int XDR_MASK = 0b_010;
    private const int STR_MASK = 0b_011;
    private const int SER_MASK = 0b_100;
    private const int SDN_MASK = 0b_101;
    private const int INTR_MASK = 0b_110;
    private const int INIT_MASK = 0b_111;

    public RX8EInstructions(ICPU cpu) : base(cpu)
    {

    }

    protected override string OpCodeText => ((RX8EOpCode)(Data & 0b_111)).ToString();

    protected override void PrivilegedExecute()
    {
        switch (Data & 0b_111)
        {
            case SEL_MASK:
                SEL();
                break;
            case LCD_MASK:
                LCD();
                break;
            case XDR_MASK:
                XDR();
                break;
            case STR_MASK:
                STR();
                break;
            case SER_MASK:
                SER();
                break;
            case SDN_MASK:
                SDN();
                break;
            case INTR_MASK:
                INTR();
                break;
            case INIT_MASK:
                INIT();
                break;
            default:
                throw new NotImplementedException();
        }
    }

    private void INIT()
    {
        CPU.RX8E.Initialize();
    }

    private void INTR()
    {
        CPU.RX8E.SetInterrupts(AC.Accumulator);
    }

    private void SDN()
    {
        if (CPU.RX8E.SkipNotDone())
        {
            PC.Increment();
        }
    }

    private void SER()
    {
        if (CPU.RX8E.SkipError())
        {
            PC.Increment();
        }
    }

    private void STR()
    {
        if (CPU.RX8E.SkipTransferRequest())
        {
            PC.Increment();
        }
    }

    private void XDR()
    {
        AC.SetAccumulator(CPU.RX8E.TransferDataRegister(AC.Accumulator));
    }

    private void LCD()
    {
        CPU.RX8E.LoadCommandRegister(AC.Accumulator);

        AC.ClearAccumulator();
    }

    private void SEL()
    {
        Log.Warning("SEL is currently no-op:");
        Log.Warning(AC.ToString());
    }

    private enum RX8EOpCode
    {
        SEL = SEL_MASK,
        LCD = LCD_MASK,
        XDR = XDR_MASK,
        STR = STR_MASK,
        SER = SER_MASK,
        SDN = SDN_MASK,
        INTR = INTR_MASK,
        INIT = INIT_MASK,
    }
}