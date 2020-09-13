using System;
using System.Collections.Generic;
using System.Text;

namespace Core8.Floppy.States.Abstract
{
    internal abstract class StateBase
    {
        public StateBase()
        {
            //RX01 = rx01 ?? throw new ArgumentNullException(nameof(rx01));
        }

        protected RX01 RX01 { get; private set; }

        public virtual bool Done => false;

        public virtual bool TransferRequest => false;

        public virtual int LCD(int acc)
        {
            return acc;
        }

        public virtual int XDR(int acc)
        {
            return acc;
        }
    }
}
