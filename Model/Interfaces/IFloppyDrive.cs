using System;
using System.Collections.Generic;
using System.Text;

namespace Core8.Model.Interfaces
{
    public interface IFloppyDrive
    {
        int ID { get; }

        bool Done { get; }

        bool TransferRequest { get; }

        bool InterruptRequested { get; }

        void ClearDone();

        void ClearTransferRequest();        
    }
}
