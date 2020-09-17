using Core8.Peripherals.Floppy.Interfaces;
using Core8.Peripherals.Floppy.States.Abstract;

namespace Core8.Peripherals.Floppy.States
{
    internal class WriteSector : ReadWriteSectorBase
    {
        public WriteSector(IController controller) : base(controller)
        {

        }

        protected override bool FinalizeState()
        {
            if (SectorTransferred && TrackTransferred)
            {
                Controller.WriteSector();

                return true;
            }

            return base.FinalizeState();
        }
    }
}
