using Core8.Peripherals.Floppy.Declarations;
using Core8.Peripherals.Floppy.Interfaces;
using System;

namespace Core8.Peripherals.Floppy.States.Abstract
{
    internal abstract class ReadWriteSectorBase : StateBase
    {
        protected bool sectorTransferred;
        protected bool trackTransferred;

        public ReadWriteSectorBase(IController controller) : base(controller)
        {
            Controller.SetTransferRequest(true);
        }

        protected override int TransferData(int acc)
        {
            Controller.IR.SetIR(acc);

            if (!sectorTransferred)
            {
                Controller.SetSectorAddress(Controller.IR.Content);

                sectorTransferred = true;

                Controller.SetTransferRequest(true);
            }
            else if (!trackTransferred)
            {
                Controller.SetTrackAddress(Controller.IR.Content);

                trackTransferred = true;
            }

            return Controller.IR.Content;
        }

        protected override TimeSpan StateLatency => Latencies.AverageAccessTime;
    }
}
