using Core8.Model.Register;

namespace Core8.Model.Interfaces
{
    public interface IRegisters
    {
        LinkAccumulator LINK_AC { get; }

        InstructionFieldProgramCounter IF_PC { get; }

        SwitchRegister SR { get; }

        MultiplierQuotient MQ { get; }

        DataField DF { get; }

        InstructionBuffer IB { get; }
        
        UserBuffer UB { get; }

        UserFlag UF { get; }

        SaveField SF { get; }
    }
}
