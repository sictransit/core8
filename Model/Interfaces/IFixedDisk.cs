﻿using Core8.Model.Registers;

namespace Core8.Model.Interfaces
{
    public interface IFixedDisk : IIODevice
    {
        void Load(int unit, byte[]? data = null);

        void ClearAll(LinkAccumulator lac);

        void LoadCurrentAddress(LinkAccumulator lac);

        void LoadCommandRegister(LinkAccumulator lac);

        void LoadAddressAndGo(LinkAccumulator lac);

        bool SkipOnTransferDoneOrError();
    }

}
