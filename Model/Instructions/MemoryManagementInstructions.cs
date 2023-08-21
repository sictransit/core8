using Core8.Extensions;
using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;

namespace Core8.Model.Instructions;

public class MemoryManagementInstructions : PrivilegedInstructionsBase
{
    private const int MEM_MGMT_MASK = 0b_110_010_000_100;

    private const int CINT_MASK = MEM_MGMT_MASK;
    private const int RDF_MASK = (0b_001 << 3) | MEM_MGMT_MASK;
    private const int RIF_MASK = (0b_010 << 3) | MEM_MGMT_MASK;
    private const int RIB_MASK = (0b_011 << 3) | MEM_MGMT_MASK;
    private const int RMF_MASK = (0b_100 << 3) | MEM_MGMT_MASK;
    private const int SINT_MASK = (0b_101 << 3) | MEM_MGMT_MASK;
    private const int CUF_MASK = (0b_110 << 3) | MEM_MGMT_MASK;
    private const int SUF_MASK = (0b_111 << 3) | MEM_MGMT_MASK;

    private const int CDF_MASK = 1 << 0;
    private const int CIF_MASK = 1 << 1;

    private string dataFieldDebug = string.Empty;

    public MemoryManagementInstructions(ICPU cpu) : base(cpu)
    {

    }

    public override IInstruction LoadData(int data)
    {
        IInstruction instruction = base.LoadData(data);

        // TODO: Not a good idea to populate this all the time.
        dataFieldDebug = !IsReadInstruction ? ((Data >> 3) & 0b_111).ToOctalString(0) : string.Empty;

        return instruction;
    }

    protected override string OpCodeText =>
        IsReadInstruction
        ? SplitOpCodes((MemoryManagementReadOpCode)(Data & 0b_111_111_111_111))
        : $"{SplitOpCodes((MemoryManagementChangeOpCodes)(Data & 0b_000_000_000_011))} {dataFieldDebug}";

    private bool IsReadInstruction => (Data & 0b_000_000_000_100) == 0b_000_000_000_100;

    protected override void PrivilegedExecute()
    {
        if (IsReadInstruction)
        {
            PrivilegedExecuteRead();
        }
        else
        {
            PrivilegedExecuteChange();
        }
    }

    private void PrivilegedExecuteRead()
    {
        switch (Data & 0b_111_111_111_111)
        {
            case CINT_MASK:
                CINT();
                break;
            case RDF_MASK:
                RDF();
                break;
            case RIB_MASK:
                RIB();
                break;
            case RIF_MASK:
                RIF();
                break;
            case RMF_MASK:
                RMF();
                break;
            case SINT_MASK:
                SINT();
                break;
            case CUF_MASK:
                CUF();
                break;
            case SUF_MASK:
                SUF();
                break;
            default:
                throw new NotImplementedException();
        }
    }

    private void PrivilegedExecuteChange()
    {
        if ((Data & CDF_MASK) != 0)
        {
            DF.Set(Data >> 3);
        }

        if ((Data & CIF_MASK) != 0)
        {
            IB.Set(Data >> 3);

            Interrupts.Inhibit();
        }
    }


    private void CINT()
    {
        Interrupts.ClearUser();
    }

    private void RDF()
    {
        AC.ORAccumulator(DF.Content << 3);
    }

    private void RIB()
    {
        AC.ORAccumulator(SF.Content & 0b_000_001_111_111);
    }

    private void RIF()
    {
        AC.ORAccumulator(PC.IF << 3);
    }

    private void RMF()
    {
        IB.Set(SF.IF);
        DF.Set(SF.DF);
        UB.Set(SF.UF);

        Interrupts.Inhibit();
    }

    private void SINT()
    {
        if (Interrupts.UserRequested)
        {
            PC.Increment();
        }
    }

    private void CUF()
    {
        UB.Clear();

        Interrupts.Inhibit();
    }

    private void SUF()
    {
        UB.Set(1);

        Interrupts.Inhibit();
    }

    [Flags]
    private enum MemoryManagementChangeOpCodes
    {
        CDF = CDF_MASK,
        CIF = CIF_MASK
    }

    private enum MemoryManagementReadOpCode
    {
        CINT = CINT_MASK,
        RDF = RDF_MASK,
        RIF = RIF_MASK,
        RIB = RIB_MASK,
        RMF = RMF_MASK,
        SINT = SINT_MASK,
        CUF = CUF_MASK,
        SUF = SUF_MASK,
    }
}
