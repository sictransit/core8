﻿using System;
using System.Threading;

namespace Core8.Model.Interfaces
{
    public interface ITeleprinter
    {
        void Read(byte c);

        void Type(byte c);

        void Clear();

        void ClearInputFlag();

        void ClearOutputFlag();

        void SetInputFlag();

        void SetOutputFlag();

        bool InputFlag { get; }

        bool OutputFlag { get; }

        byte InputBuffer { get; }

        byte OutputBuffer { get; }

        AutoResetEvent OutputAvailable { get; }

        string Printout { get; }

        void FormFeed();

        void SetIRQHook(Action<bool> irq);

        void SetDeviceControls(uint data);

        void MountPaperTape(byte[] chars);
        
        bool IsTapeLoaded { get; }
    }
}
