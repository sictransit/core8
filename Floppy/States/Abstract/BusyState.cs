using Core8.Floppy.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core8.Floppy.States.Abstract
{
    internal abstract class BusyState : StateBase
    {
        public BusyState(IController controller) : base(controller)
        {

        }

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
