﻿namespace Core8.Model.Interfaces
{
    public interface ITeletype
    {
        void Tick();

        void Type(byte c);

        void Clear();

        bool InputFlag { get; }

        void ClearInputFlag();

        bool OutputFlag { get; }

        void ClearOutputFlag();

        void SetOutputFlag();

        bool InputIRQ { get; }

        bool OutputIRQ { get; }

        byte InputBuffer { get; }

        string Printout { get; }

        void SetDeviceControl(int data);

        void MountPaperTape(byte[] chars);
    }
}
