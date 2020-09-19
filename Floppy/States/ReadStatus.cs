using Core8.Peripherals.Floppy.Interfaces;
using Core8.Peripherals.Floppy.States.Abstract;

namespace Core8.Peripherals.Floppy.States
{
    internal class ReadStatus : StateBase
    {
        public ReadStatus(IController controller) : base(controller)
        {

        }

        protected override bool FinalizeState() => true;

        protected override void SetIR()
        {
            Controller.ES.SetInitializationDone(false);

            Controller.IR.SetIR(Controller.IR.Content << 8 | Controller.ES.Content);
        }

    }
}
