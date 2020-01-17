using System;

namespace Core8.Model.Interfaces
{
    public interface ITeleprinter
    {
        void Tick();

        void Clear();

        void ClearInputFlag();

        void ClearOutputFlag();

        void SetInputFlag();

        void SetOutputFlag();

        bool IsInputFlagSet { get; }

        bool IsOutputFlagSet { get; }

        bool IsTapeLoaded { get; }

        uint GetBuffer();

        string Printout { get; }

        void FormFeed();

        void SetIRQHook(Action<bool> irq);

        void SetDeviceControls(uint data);

        void Type(byte c);

        void Read(byte[] chars);

        void Read(byte c);
    }
}
