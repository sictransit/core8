using Core8.Register;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core8.Interfaces
{
    public interface IRegisters
    {
        LINK_AC LINK_AC { get; }

        IF_PC IF_PC { get; }
    }
}
