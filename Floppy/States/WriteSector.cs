using Core8.Floppy.Interfaces;

namespace Core8.Floppy.States
{
    internal class WriteSector : ReadWriteSectorBase
    {
        public WriteSector(IController controller) : base(controller)
        {

        }

        protected override bool FinalizeState()
        {
            if (sectorTransferred && trackTransferred)
            {
                Controller.WriteSector();

                return true;
            }

            return base.FinalizeState();
        }
    }
}
