using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using System;

namespace Core8.Model.Instructions;

public class LinePrinterInstructions : PrivilegedInstructionsBase
{
    public LinePrinterInstructions(ICPU cpu) : base(cpu)
    {
    }

    protected override string OpCodeText => throw new NotImplementedException();

    protected override void PrivilegedExecute()
    {
        throw new NotImplementedException();
    }
}
