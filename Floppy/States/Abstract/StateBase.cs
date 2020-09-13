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

        public virtual void Tick() { }

        public virtual int LCD(int acc) => acc;
        public virtual int XDR(int acc) => acc;

        public virtual bool SND() => false;

        public virtual bool STR() => false;
    }
}
