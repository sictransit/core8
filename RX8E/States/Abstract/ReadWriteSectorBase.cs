﻿namespace Core8.Peripherals.RX8E.States.Abstract;

internal abstract class ReadWriteSectorBase : StateBase
{
    protected bool SectorTransferred;
    protected bool TrackTransferred;

    protected ReadWriteSectorBase(RX8EController controller) : base(controller)
    {
        Controller.ER.Clear();
        Controller.ES.Clear();

        Controller.SetTransferRequest(true);
    }

    //protected override TimeSpan MinExecutionTime => TimeSpan.FromMilliseconds(262);

    protected override int TransferData(int acc)
    {
        Controller.IR.Set(acc);

        if (!SectorTransferred)
        {
            Controller.SetSectorAddress(Controller.IR.Content);

            SectorTransferred = true;

            Controller.SetTransferRequest(true);
        }
        else if (!TrackTransferred)
        {
            Controller.SetTrackAddress(Controller.IR.Content);

            TrackTransferred = true;
        }

        return Controller.IR.Content;
    }
}
