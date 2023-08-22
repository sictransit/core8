using Core8.Peripherals.RX8E.States.Abstract;

namespace Core8.Peripherals.RX8E.States;

internal class ReadErrorRegister : StateBase
{
    public ReadErrorRegister(FloppyDrive controller) : base(controller)
    {

    }

    protected override bool FinalizeState() => true;

    protected override void SetIR()
    {
        Controller.IR.Set(Controller.IR.Content << 8 | Controller.ER.Content);
    }
}
