using Core8.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core8.Abstract
{
    public abstract class IODevice : IIODevice
    {
        protected IODevice(uint id)
        {
            Id = id;
        }

        public uint Id { get; }

        public abstract void Tick();
    }
}
