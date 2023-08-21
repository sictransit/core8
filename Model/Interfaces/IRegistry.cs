using Core8.Model.Registers;

namespace Core8.Model.Interfaces;

public interface IRegistry
{
    LinkAccumulator AC { get; }

    InstructionFieldProgramCounter PC { get; }

    SwitchRegister SR { get; }

    MultiplierQuotient MQ { get; }

    DataField DF { get; }

    InstructionBuffer IB { get; }

    UserBuffer UB { get; }

    UserFlag UF { get; }

    SaveField SF { get; }
}