﻿using Core8.Peripherals.RX8E.Interfaces;
using Core8.Peripherals.RX8E.States.Abstract;

namespace Core8.Peripherals.RX8E.States;

internal class EmptyBuffer : FillEmptyBufferBase
{
    public EmptyBuffer(IController controller) : base(controller)
    {

    }

    protected override int TransferData(int acc)
    {
        Controller.IR.Set(Controller.Buffer[BufferPointer++]);

        Controller.SetTransferRequest(BufferPointer < Controller.Buffer.Length);

        return Controller.IR.Content;
    }
}
