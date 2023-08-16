using Core8.Model.Registers;
using System.Net;

namespace Core8.Model.Interfaces
{
    public interface IFixedDisk : IIODevice
    {
        void ClearAll(LinkAccumulator lac);

        void LoadCurrentAddress(LinkAccumulator lac);

        void LoadCommandRegister(LinkAccumulator lac);

        void LoadAddressAndGo(LinkAccumulator lac);

        bool SkipOnTransferDoneOrError();
    }

}
