using Core8.Floppy.Declarations;
using Core8.Floppy.Interfaces;
using Core8.Floppy.States.Abstract;
using System;

namespace Core8.Floppy.States
{
    internal class Initialize : StateBase
    {
        public Initialize(IController controller, IDrive drive) : base(controller, drive)
        {

        }

        protected override TimeSpan StateLatency => Latencies.InitializeTime;

        public override void Tick()
        {
            if (IsStateChangeDue)
            {
                // TODO: init magic!

                Controller.SetState(new Idle(this.Controller, this.Drive));
            }
        }
    }
}
