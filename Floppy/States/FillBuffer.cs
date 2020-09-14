using Core8.Floppy.Interfaces;
using Core8.Floppy.States.Abstract;
using Core8.Model;
using Serilog;

namespace Core8.Floppy.States
{
    internal class FillBuffer : StateBase
    {
        private int bufferPointer;

        public FillBuffer(IController controller, IDrive drive) : base(controller, drive)
        {
            transferRequest = true;
        }

        public override int XDR(int acc)
        {
            if (transferRequest)
            {
                Log.Error("XDR with TR high");
            }
            else
            {
                Controller.Buffer[bufferPointer++] = acc & Masks.AC;

                if (bufferPointer < Controller.Buffer.Length)
                {
                    transferRequest = true;
                }
            }

            return acc;
        }

        protected override bool EndState() => bufferPointer == Controller.Buffer.Length;
    }
}
