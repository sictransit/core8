using Core8.Model.Interfaces;
using Core8.Model.Register;

namespace Core8
{
    public class Registers : IRegisters
    {
        public Registers()
        {
            LINK_AC = new LINK_AC();
            IF_PC = new IF_PC();
            Switch = new Switch();
        }

        public LINK_AC LINK_AC { get; }

        public IF_PC IF_PC { get; }

        public Switch Switch { get; }
    }
}
