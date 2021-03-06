﻿using Core8.Peripherals.Floppy.Interfaces;
using Core8.Peripherals.Floppy.States.Abstract;

namespace Core8.Peripherals.Floppy.States
{
    internal class ReadErrorRegister : StateBase
    {
        public ReadErrorRegister(IController controller) : base(controller)
        {

        }

        protected override bool FinalizeState() => true;

        protected override void SetIR()
        {
            Controller.IR.Set(Controller.IR.Content << 8 | Controller.ER.Content);
        }
    }
}
