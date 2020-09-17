using Core8.Peripherals.Floppy.Interfaces;
using Core8.Peripherals.Floppy.States.Abstract;

namespace Core8.Peripherals.Floppy.States
{
    internal class ReadSector : ReadWriteSectorBase
    {
        public ReadSector(IController controller) : base(controller)
        {

        }

        protected override bool FinalizeState()
        {
            if (SectorTransferred && TrackTransferred)
            {
                Controller.ReadSector();

                return true;
            }

            return base.FinalizeState();
        }
    }
}
