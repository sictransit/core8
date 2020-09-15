using Core8.Floppy.Interfaces;
using Core8.Floppy.States.Abstract;
using Core8.Model;

namespace Core8.Floppy.States
{
    internal class FillBuffer : StateBase
    {
        private int bufferPointer;

        public FillBuffer(IController controller) : base(controller)
        {
            Controller.SetTransferRequest(true);
        }

        protected override int TransferData(int acc)
        {
            Controller.IR.SetIR(acc);

            Controller.Buffer[bufferPointer++] = Controller.IR.Content;

            Controller.SetTransferRequest(bufferPointer < Controller.Buffer.Length);

            return Controller.IR.Content;
        }

        protected override bool FinalizeState() => bufferPointer == Controller.Buffer.Length;
    }
}
