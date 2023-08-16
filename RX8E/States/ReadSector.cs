using Core8.Peripherals.RX8E.Interfaces;
using Core8.Peripherals.RX8E.States.Abstract;

namespace Core8.Peripherals.RX8E.States
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
