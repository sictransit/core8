using Core8.Peripherals.Floppy.Interfaces;
using Core8.Peripherals.Floppy.States.Abstract;

namespace Core8.Peripherals.Floppy.States
{
    internal class EmptyBuffer : FillEmptyBufferBase
    {
        public EmptyBuffer(IController controller) : base(controller)
        {

        }

        protected override int TransferData(int acc)
        {
            Controller.IR.Set(Controller.Buffer[BufferPointer++]);

            Controller.SetTransferRequest(BufferPointer < Controller.Buffer.Length);

            return Controller.IR.Content;
        }
    }
}
