using Core8.Floppy.Declarations;
using Core8.Floppy.Interfaces;
using Core8.Floppy.States.Abstract;
using System;

namespace Core8.Floppy.States
{
    internal class Idle : StateBase
    {
        public Idle(IController controller, IDrive drive) : base(controller, drive)
        {
            done = true;
        }

        public override int LCD(int acc)
        {
            if (done)
            {
                return base.LCD(acc);
            }

            Controller.SetCommandRegister(acc);

            StateBase newState;

            switch (Controller.CurrentFunction)
            {
                case ControllerFunction.FillBuffer:
                    newState = new FillBuffer(this.Controller, this.Drive);
                    break;
                case ControllerFunction.NoOperation:
                    newState = new NoOperation(this.Controller, this.Drive);
                    break;
                case ControllerFunction.EmptyBuffer:
                    newState = new EmptyBuffer(this.Controller, this.Drive);
                    break;
                case ControllerFunction.WriteSector:
                case ControllerFunction.ReadSector:
                case ControllerFunction.ReadStatus:
                case ControllerFunction.WriteDeletedDataSector:
                case ControllerFunction.ReadErrorRegister:
                    throw new NotImplementedException(Controller.CurrentFunction.ToString());
                default:
                    throw new NotImplementedException(Controller.CurrentFunction.ToString());
            }

            Controller.SetState(newState);

            return 0;
        }
    }
}
