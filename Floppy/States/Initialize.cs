using Core8.Peripherals.Floppy.Declarations;
using Core8.Peripherals.Floppy.Interfaces;
using Core8.Peripherals.Floppy.States.Abstract;
using System;

namespace Core8.Peripherals.Floppy.States
{
    internal class Initialize : StateBase
    {
        public Initialize(IController controller) : base(controller)
        {
            Controller.SetDone(false);
            Controller.SetTransferRequest(false);
            Controller.SetError(false);
            Controller.SetInterrupts(0);

            Controller.CR.Clear();
            Controller.ES.Clear();
            Controller.EC.Clear();
            Controller.IR.Clear();
        }

        protected override TimeSpan StateLatency => Latencies.InitializeTime;

        protected override bool FinalizeState()
        {
            Controller.SetSectorAddress(1);
            Controller.SetTrackAddress(1);

            Controller.ES.SetInitializationDone(true);
            Controller.ES.SetReady(true);
            //Controller.ES.SetWriteProtect(true);

            //Controller.SetInterrupts(1);

            Controller.ReadSector();

            return true;
        }
    }
}
