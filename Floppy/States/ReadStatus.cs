using Core8.Peripherals.Floppy.Interfaces;
using Core8.Peripherals.Floppy.States.Abstract;
using System;

namespace Core8.Peripherals.Floppy.States
{
    internal class ReadStatus : StateBase
    {
        public ReadStatus(IController controller) : base(controller)
        {

        }

        //protected override TimeSpan MinExecutionTime => TimeSpan.FromMilliseconds(250);

        protected override bool FinalizeState()
        {
            Controller.ES.SetInitializationDone(false);

            return true;
        }

        protected override void SetIR()
        {
            Controller.IR.Set(Controller.IR.Content << 8 | Controller.ES.Content);
        }
    }
}
