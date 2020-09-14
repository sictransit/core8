using Core8.Floppy.Interfaces;
using Core8.Floppy.States.Abstract;
using Serilog;
using System;

namespace Core8.Floppy.States
{
    internal class EmptyBuffer : StateBase
    {
        private int bufferPointer;

        public EmptyBuffer(IController controller, IDrive drive) : base(controller, drive)
        {
            transferRequest = true;
        }

        public override int XDR(int acc)
        {
            var data = acc;

            if (transferRequest)
            {
                Log.Error("XDR with TR high");
            }
            else
            {
                data = Controller.Buffer[bufferPointer++];

                transferRequest = bufferPointer < Controller.Buffer.Length;
            }

            return data;
        }

        protected override bool EndState() => bufferPointer == Controller.Buffer.Length;
    }
}
