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
            transferRequest = true;
        }

        protected override int TransferData(int acc)
        {
            Controller.Buffer[bufferPointer++] = acc & Masks.AC;

            transferRequest = bufferPointer < Controller.Buffer.Length;

            return acc;
        }

        protected override bool EndState() => bufferPointer == Controller.Buffer.Length;
    }
}
