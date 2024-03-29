﻿using Core8.Peripherals.RX8E.States.Abstract;

namespace Core8.Peripherals.RX8E.States;

internal class WriteSector : ReadWriteSectorBase
{
    public WriteSector(RX8EController controller) : base(controller)
    {

    }

    protected override bool FinalizeState()
    {
        if (SectorTransferred && TrackTransferred)
        {
            Controller.WriteSector();

            return true;
        }

        return base.FinalizeState();
    }
}
