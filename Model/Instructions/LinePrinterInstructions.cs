using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;

namespace Core8.Model.Instructions;

public class LinePrinterInstructions : PrivilegedInstructionsBase
{
    private const int PSKF_MASK = 1 << 0;

    public LinePrinterInstructions(ICPU cpu) : base(cpu)
    {
    }

    protected override string OpCodeText => ((PrinterPunchOpCode)(Data & 0b_111)).ToString();

    protected override void PrivilegedExecute()
    {
        switch (Data & 0b_111)
        {
            case PSKF_MASK:
                PSKF();
                break;
            default:
                throw new NotImplementedException();
        }
    }

    private void PSKF()
    {
        // TODO: implement!
    }

    private enum PrinterPunchOpCode
    {
        PSKF = PSKF_MASK,
    }
}
