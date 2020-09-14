﻿using Core8.Floppy.Declarations;
using Core8.Floppy.Interfaces;
using Core8.Floppy.States.Abstract;

namespace Core8.Floppy
{
    internal class RX8E : IController
    {
        private StateBase state;

        private int commandRegister;

        public ControllerFunction CurrentFunction => (ControllerFunction)(commandRegister & 0b_000_000_001_110);


        public void SetCommandRegister(int acc)
        {
            commandRegister = acc & 0b_000_011_111_110;
        }

        public void SetState(StateBase state)
        {
            this.state = state;
        }

        public bool MaintenanceMode => (commandRegister & 0b_000_010_000_000) != 0;

        public int LCD(int acc) => state.LCD(acc);

        public bool SND() => state.SND();

        public bool STR() => state.STR();

        public void Tick() => state.Tick();
    }
}
