using Core8.Model.Registers;

namespace Core8.Model.Interfaces
{
    public interface IFixedDisk : IIODevice
    {
        void LoadCurrentAddress(LinkAccumulator lac);
    }

}
