using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;

namespace Core8.Model.Instructions;

public class RK8EInstructions : PrivilegedInstructionsBase
{
    private const int DSKP_MASK = 0b_001;
    private const int DCLR_MASK = 0b_010;
    private const int DLAG_MASK = 0b_011;
    private const int DLCA_MASK = 0b_100;
    private const int DRST_MASK = 0b_101;
    private const int DLDC_MASK = 0b_110;
    private const int DIOT_MASK = 0b_111;

    public RK8EInstructions(ICPU cpu) : base(cpu)
    {
    }

    protected override string OpCodeText => ((RK8EOpCode)(Data & 0b_111)).ToString();

    protected override void PrivilegedExecute()
    {
        switch (Data & 0b_111)
        {
            case DSKP_MASK:
                DSKP();
                break;
            case DCLR_MASK:
                DCLR();
                break;
            case DLAG_MASK:
                DLAG();
                break;
            case DLCA_MASK:
                DLCA();
                break;
            case DRST_MASK:
                DRST();
                break;
            case DLDC_MASK:
                DLDC();
                break;
            case DIOT_MASK:
                DIOT();
                break;
            default:
                throw new NotImplementedException();
        }
    }

    private void DSKP()
    {
        if (CPU.RK8E.SkipOnTransferDoneOrError())
        {
            PC.Increment();
        }
    }

    private void DCLR()
    {
        CPU.RK8E.ClearAll(CPU.Registry.AC);
    }

    private void DLAG()
    {
        CPU.RK8E.LoadAddressAndGo(CPU.Registry.AC);
    }

    private void DLCA()
    {
        CPU.RK8E.LoadCurrentAddress(CPU.Registry.AC);
    }

    private void DRST()
    {
        CPU.RK8E.ReadStatusRegister(CPU.Registry.AC);
    }

    private void DLDC()
    {
        CPU.RK8E.LoadCommandRegister(CPU.Registry.AC);
    }

    private void DIOT()
    {
        throw new NotImplementedException();
    }

    private enum RK8EOpCode
    {
        DSKP = DSKP_MASK,
        DCLR = DCLR_MASK,
        DLAG = DLAG_MASK,
        DLCA = DLCA_MASK,
        DRST = DRST_MASK,
        DLDC = DLDC_MASK,
        DIOT = DIOT_MASK,
    }
}
