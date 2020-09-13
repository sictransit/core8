using Core8.Floppy.Interfaces;
using Core8.Floppy.States.Abstract;
using System;

namespace Core8.Floppy.States
{
    internal class IdleState : StateBase
    {
        private bool done;

        public IdleState(IController controller) : base(controller)
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
                case Constants.ControllerFunction.FillBuffer:
                    newState = new FillBufferState(this.Controller);
                    break;
                case Constants.ControllerFunction.EmptyBuffer:
                case Constants.ControllerFunction.WriteSector:
                case Constants.ControllerFunction.ReadSector:
                case Constants.ControllerFunction.NoOperation:
                case Constants.ControllerFunction.ReadStatus:
                case Constants.ControllerFunction.WriteDeletedDataSector:
                case Constants.ControllerFunction.ReadErrorRegister:
                    throw new NotImplementedException(Controller.CurrentFunction.ToString());
                default:
                    throw new NotImplementedException(Controller.CurrentFunction.ToString());
            }

            Controller.SetState(newState);

            return 0;
        }

        public override bool SND()
        {
            if (done)
            {
                done = false;

                return true;
            }

            return false;
        }
    }
}
