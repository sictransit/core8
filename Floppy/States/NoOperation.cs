using Core8.Floppy.Interfaces;
using Core8.Floppy.States.Abstract;
using System;

namespace Core8.Floppy.States
{
    internal class NoOperation : StateBase
    {
        public NoOperation(IController controller) : base(controller)
        {

        }

        protected override bool FinalizeState() => true;

        protected override TimeSpan StateLatency => TimeSpan.Zero;
    }
}
