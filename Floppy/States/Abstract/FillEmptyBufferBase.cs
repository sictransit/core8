using Core8.Floppy.Interfaces;

namespace Core8.Floppy.States.Abstract
{
    internal abstract class FillEmptyBufferBase : StateBase
    {
        protected int bufferPointer;

        public FillEmptyBufferBase(IController controller) : base(controller)
        {
            Controller.SetTransferRequest(true);
        }

        protected override bool FinalizeState() => bufferPointer == Controller.Buffer.Length;

        public override string ToString() => $"{base.ToString()} ptr={bufferPointer}";
    }
}
