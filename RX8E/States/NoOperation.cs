using Core8.Peripherals.RX8E.States.Abstract;

namespace Core8.Peripherals.RX8E.States;

internal class NoOperation : StateBase
{
    public NoOperation(RX8EController controller) : base(controller)
    {

    }

    protected override bool FinalizeState() => true;
}
