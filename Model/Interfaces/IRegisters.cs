using Core8.Model.Register;

namespace Core8.Model.Interfaces
{
    public interface IRegisters
    {
        LINK_AC LINK_AC { get; }

        IF_PC IF_PC { get; }

        SR Switch { get; }
    }
}
