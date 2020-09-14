﻿using Core8.Floppy.Declarations;
using Core8.Floppy.Interfaces;
using Core8.Floppy.States.Abstract;
using Serilog;

namespace Core8.Floppy
{
    internal class Controller : IController
    {
        private StateBase state;

        private int commandRegister;

        public Controller(IDrive drive)
        {
            Buffer = new int[64];

            Drive = drive ?? throw new System.ArgumentNullException(nameof(drive));
        }

        public int[] Buffer { get; }

        public ControllerFunction CurrentFunction => (ControllerFunction)(commandRegister & 0b_000_000_001_110);

        public void SetCommandRegister(int acc)
        {
            commandRegister = acc & 0b_000_011_111_110;
        }

        public void SetState(StateBase state)
        {
            Log.Information($"Controller state transition: {this.state} -> {state}");

            this.state = state;
        }

        public bool MaintenanceMode => (commandRegister & 0b_000_010_000_000) != 0;

        public IDrive Drive { get; }

        public int LCD(int acc) => state.LCD(acc);

        public int XDR(int acc) => state.XDR(acc);

        public bool SND() => state.SND();

        public bool STR() => state.STR();

        public void Tick() => state.Tick();

        public void ReadSector()
        {
            throw new System.NotImplementedException();
        }

        public void WriteSector()
        {
            throw new System.NotImplementedException();
        }
    }
}
