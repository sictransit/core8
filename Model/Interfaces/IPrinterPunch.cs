using System.Collections.Generic;

namespace Core8.Model.Interfaces;

public interface IPrinterPunch : IIODevice
{
    void Print(byte c);

    void Clear();

    bool OutputFlag { get; }

    void ClearOutputFlag();

    void SetOutputFlag();

    string Printout { get; }

    IReadOnlyCollection<byte> Output { get; }

    void SetDeviceControl(int data);
}