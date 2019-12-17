using System;
using System.Collections.Generic;
using System.Text;

namespace Core8.Model.Interfaces
{
    public interface IOutputDevice
    {
        void Output(byte[] data);
    }
}
