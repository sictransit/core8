using Core8.Peripherals.Floppy.Declarations;
using Core8.Peripherals.Floppy.Interfaces;
using System;

namespace Core8.Peripherals.Floppy.States.Abstract
{
    internal abstract class ReadWriteSectorBase : StateBase
    {
        protected bool SectorTransferred;
        protected bool TrackTransferred;

        protected ReadWriteSectorBase(IController controller) : base(controller)
        {
            Controller.SetTransferRequest(true);
        }

        protected override int TransferData(int acc)
        {
            Controller.IR.SetIR(acc);

            if (!SectorTransferred)
            {
                Controller.SetSectorAddress(Controller.IR.Content);

                SectorTransferred = true;

                Controller.SetTransferRequest(true);
            }
            else if (!TrackTransferred)
            {
                Controller.SetTrackAddress(Controller.IR.Content);

                TrackTransferred = true;
            }

            return Controller.IR.Content;
        }

        protected override TimeSpan StateLatency => Latencies.AverageAccessTime;
    }
}
