namespace Core8.Peripherals.RX8E.States.Abstract;

internal abstract class FillEmptyBufferBase : StateBase
{
    protected int BufferPointer;

    protected FillEmptyBufferBase(RX8EController controller) : base(controller)
    {
        Controller.SetTransferRequest(true);
    }

    protected override bool FinalizeState() => BufferPointer == Controller.Buffer.Length;

    public override string ToString() => $"{base.ToString()} ptr={BufferPointer}";
}
