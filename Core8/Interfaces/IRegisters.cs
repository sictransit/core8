using Core8.Register;

namespace Core8.Interfaces
{
    public interface IRegisters
    {
        LINK_AC LINK_AC { get; }

        IF_PC IF_PC { get; }

        DF DF { get; }

        MQ MQ { get; }
    }
}
