using Core8.Floppy.Interfaces;
using Core8.Floppy.States.Abstract;

namespace Core8.Floppy.States
{
    internal class EmptyBuffer : FillEmptyBufferBase
    {
        public EmptyBuffer(IController controller) : base(controller)
        {

        }

        protected override int TransferData(int acc)
        {
            Controller.IR.SetIR(Controller.Buffer[bufferPointer++]);

            Controller.SetTransferRequest(bufferPointer < Controller.Buffer.Length);

            return Controller.IR.Content;
        }
    }
}
