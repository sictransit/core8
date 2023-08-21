using Core8.Extensions;
using Core8.Model.Instructions.Abstract;
using Core8.Model.Interfaces;
using Serilog;

namespace Core8.Model.Instructions;

public class PrivilegedNoOperationInstruction : PrivilegedInstructionsBase
{
    public PrivilegedNoOperationInstruction(ICPU cpu) : base(cpu)
    {
    }

    protected override string OpCodeText => "NOP";

    protected override void PrivilegedExecute()
    {
        Log.Warning($"{CPU.Registry.PC.Content.ToOctalString(5)} {Data.ToOctalString()} {this}");
    }
}
