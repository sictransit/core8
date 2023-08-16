using Core8.Peripherals.RX8E.Interfaces;
using Core8.Peripherals.RX8E.States.Abstract;

namespace Core8.Peripherals.RX8E.States
{
    internal class NoOperation : StateBase
    {
        public NoOperation(IController controller) : base(controller)
        {

        }

        protected override bool FinalizeState() => true;
    }
}
