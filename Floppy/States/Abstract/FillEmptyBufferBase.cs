using Core8.Peripherals.Floppy.Interfaces;

namespace Core8.Peripherals.Floppy.States.Abstract
{
    internal abstract class FillEmptyBufferBase : StateBase
    {
        protected int BufferPointer;

        protected FillEmptyBufferBase(IController controller) : base(controller)
        {
            Controller.SetTransferRequest(true);
        }

        protected override bool FinalizeState() => BufferPointer == Controller.Buffer.Length;

        public override string ToString() => $"{base.ToString()} ptr={BufferPointer}";
    }
}
