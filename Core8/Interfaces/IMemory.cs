using System;
using System.Collections.Generic;
using System.Text;

namespace Core8.Interfaces
{
    public interface IMemory
    {
        uint Read(uint address);
    }
}
