using Core8.Peripherals.Floppy.Interfaces;
using Core8.Peripherals.Floppy.States.Abstract;

namespace Core8.Peripherals.Floppy.States
{
    internal class NoOperation : StateBase
    {
        public NoOperation(IController controller) : base(controller)
        {

        }

        protected override bool FinalizeState() => true;
    }
}
