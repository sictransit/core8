using System;
using System.Collections.Generic;
using System.Text;

namespace Core8.Floppy.States.Abstract
{
    internal abstract class BusyState : StateBase
    {
        protected bool TransferRequest { get; set; }

        public override bool STR()
        {
            if (TransferRequest)
            {
                TransferRequest = false;

                return true;
            }

            return false;
        }
    }
}
