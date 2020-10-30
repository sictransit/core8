using Core8.Model.Interfaces;
using Core8.Model.Registers;
using Core8.Model.Registers.Abstract;
using System;
using System.Linq;

namespace Core8.Model
{
    public class Registry : IRegistry
    {
        public Registry()
        {
            AC = new LinkAccumulator();
            PC = new InstructionFieldProgramCounter();
            SR = new SwitchRegister();
            MQ = new MultiplierQuotient();
            DF = new DataField();
            IB = new InstructionBuffer();
            UB = new UserBuffer();
            UF = new UserFlag();
            SF = new SaveField();
        }

        public LinkAccumulator AC { get; }

        public InstructionFieldProgramCounter PC { get; }

        public SwitchRegister SR { get; }

        public MultiplierQuotient MQ { get; }

        public DataField DF { get; }

        public InstructionBuffer IB { get; }

        public UserBuffer UB { get; }

        public UserFlag UF { get; }

        public SaveField SF { get; }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, new RegisterBase[] { AC, PC, SR, MQ, DF, IB, UB, UF, SF }.Select(x => x.ToString()));
        }
    }
}
