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
            Controller.IR.SetIR(acc);

            Controller.Buffer[bufferPointer++] = Controller.IR.Content;

            Controller.SetTransferRequest(bufferPointer < Controller.Buffer.Length);

            return Controller.IR.Content;
        }
    }
}
