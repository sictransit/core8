using Core8.Model.Interfaces;

namespace Core8
{
    public class FloppyDrive : IFloppyDrive
    {
        public FloppyDrive(int id)
        {
            this.ID = id;
        }

        //public int Interface { get; private set; }

        //public int TrackAddress { get; private set; }

        //public int SectorAddress { get; private set; }

        //public int ErrorStatus { get; private set; }

        //public int ErrorCode { get; private set; }

        public bool Done { get; private set; }

        public bool InterruptRequested { get; private set; }

        public bool TransferRequest { get; private set; }

        public int ID {get; private set;}

        public void ClearDone()
        {
            Done = false;

            InterruptRequested = true;
        }

        public void ClearTransferRequest()
        {
            TransferRequest = false;
        }
    }
}
