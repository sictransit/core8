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
            SR = new SR();
            MQ = new MQ();
            DF = new DF();
            IB = new IB();
            SF = new SF();
        }

        public LINK_AC LINK_AC { get; }

        public IF_PC IF_PC { get; }

        public SR SR { get; }

        public MQ MQ { get; }

        public DF DF { get; }

        public IB IB { get; }

        public SF SF { get; }
    }
}
