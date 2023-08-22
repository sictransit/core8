using Core8.Peripherals.RX8E.States.Abstract;

namespace Core8.Peripherals.RX8E.States;

internal class FillBuffer : FillEmptyBufferBase
{
    public FillBuffer(FloppyDrive controller) : base(controller)
    {

    }

    protected override int TransferData(int acc)
    {
        Controller.IR.Set(acc);

        Controller.Buffer[BufferPointer++] = Controller.IR.Content;

        Controller.SetTransferRequest(BufferPointer < Controller.Buffer.Length);

        return Controller.IR.Content;
    }
}
