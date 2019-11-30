using Core8.Interfaces;
using Core8.Register;

namespace Core8
{
    public class Registers : IRegisters
    {
        public Registers()
        {
            LINK_AC = new LINK_AC();
            IF_PC = new IF_PC();
        }

        public LINK_AC LINK_AC { get; }
        public IF_PC IF_PC { get; }

        public uint GetAccumulator()
        {
            return LINK_AC.Accumulator;
        }

        public void SetAccumulator(uint value)
        {
            LINK_AC.SetAccumulator(value);
        }
    }
}
