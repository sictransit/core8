using Core8.Peripherals.Floppy.Interfaces;
using Core8.Peripherals.Floppy.States.Abstract;
using System;

namespace Core8.Peripherals.Floppy.States
{
    internal class Initialize : StateBase
    {
        public Initialize(IController controller) : base(controller)
        {
            Controller.SetError(false);
            Controller.SetInterrupts(0);

            Controller.CR.Clear();
            Controller.ES.Clear();
            Controller.ER.Clear();
            Controller.IR.Clear();
        }

        protected override TimeSpan MinExecutionTime => TimeSpan.FromMilliseconds(1800);

        protected override bool FinalizeState()
        {
            Controller.SetSectorAddress(1);
            Controller.SetTrackAddress(1);

            Controller.ReadSector();

            Controller.ES.SetInitializationDone(true);
            Controller.ES.SetReady(true);

            return true;
        }
    }
}
