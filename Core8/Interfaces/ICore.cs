using System;
using System.Collections.Generic;
using System.Text;

namespace Core8.Interfaces
{
    public interface ICore
    {
        IMemory Memory { get; }

        IProcessor Processor { get; }

        IRegisters Registers { get; }
    }
}
