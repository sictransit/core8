using Core8.Floppy.Declarations;
using Core8.Floppy.Interfaces;
using Core8.Floppy.States.Abstract;
using System;

namespace Core8.Floppy.States
{
    internal class ReadWriteSector : StateBase
    {
        private bool sectorTransferred;
        private bool trackTransferred;
        private readonly bool readMode;

        public ReadWriteSector(IController controller, bool readMode = true) : base(controller)
        {
            Controller.SetTransferRequest(true);

            this.readMode = readMode;
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

        protected override bool FinalizeState()
        {
            if (sectorTransferred && trackTransferred)
            {
                if (readMode)
                {
                    Controller.ReadSector();
                }
                else
                {
                    Controller.WriteSector();
                }

                return true;
            }

            return base.FinalizeState();
        }

        protected override TimeSpan StateLatency => Latencies.AverageAccessTime;

        public override string ToString() => $"{base.ToString()} read={readMode}";
    }
}
