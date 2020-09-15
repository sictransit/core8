using Core8.Floppy.Interfaces;

namespace Core8.Floppy.States
{
    internal class ReadSector : ReadWriteSectorBase
    {
        public ReadSector(IController controller) : base(controller)
        {

        }

        protected override bool FinalizeState()
        {
            if (sectorTransferred && trackTransferred)
            {
                Controller.ReadSector();

                return true;
            }

            return base.FinalizeState();
        }
    }
}
