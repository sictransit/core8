using System;

namespace Core8.Model.Interfaces
{
    public interface IIODevice
    {
        void SetIRQHook(Action irq);

        void SetDeviceControls(uint data);

        void Tick();

        bool IsFlagSet { get; }

        void ClearFlag();

        void SetFlag();

        void Clear();

        void Type(byte c);

        void Type(byte[] chars);
    }
}
