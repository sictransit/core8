﻿using Core8.Floppy.Declarations;
using Core8.Floppy.Interfaces;
using Core8.Floppy.States.Abstract;
using System;

namespace Core8.Floppy.States
{
    internal class Initialize : StateBase
    {
        public Initialize(IController controller) : base(controller)
        {

        }

        protected override TimeSpan StateLatency => Latencies.InitializeTime;

        protected override bool EndState()
        {
            Controller.SetCommandRegister(0);

            Controller.SetTrackAddress(1);
            Controller.SetTrackAddress(1);

            Controller.ReadSector();

            return true;
        }
    }
}
