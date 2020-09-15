using Core8.Floppy.Interfaces;
using Core8.Floppy.States.Abstract;

namespace Core8.Floppy.States
{
    internal class EmptyBuffer : StateBase
    {
        private int bufferPointer;

        public EmptyBuffer(IController controller) : base(controller)
        {
            Controller.SetTransferRequest(true);
        }

        protected override int TransferData(int acc)
        {
            Controller.IR.SetIR(Controller.Buffer[bufferPointer++]);

            Controller.SetTransferRequest(bufferPointer < Controller.Buffer.Length);

            return Controller.IR.Content;
        }

        protected override bool FinalizeState() => bufferPointer == Controller.Buffer.Length;
    }
}
