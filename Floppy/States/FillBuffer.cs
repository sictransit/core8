using Core8.Peripherals.Floppy.Interfaces;
using Core8.Peripherals.Floppy.States.Abstract;

namespace Core8.Peripherals.Floppy.States
{
    internal class FillBuffer : FillEmptyBufferBase
    {
        public FillBuffer(IController controller) : base(controller)
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
}
