using Core8.Floppy.Interfaces;
using Core8.Floppy.States.Abstract;

namespace Core8.Floppy.States
{
    internal class EmptyBuffer : StateBase
    {
        private int bufferPointer;

        public EmptyBuffer(IController controller) : base(controller)
        {
            transferRequest = true;
        }

        protected override int TransferData(int acc)
        {
            var data = Controller.Buffer[bufferPointer++];

            transferRequest = bufferPointer < Controller.Buffer.Length;

            return data;
        }

        protected override bool EndState() => bufferPointer == Controller.Buffer.Length;
    }
}
