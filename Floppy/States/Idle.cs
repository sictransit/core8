using Core8.Floppy.Declarations;
using Core8.Floppy.Interfaces;
using Core8.Floppy.States.Abstract;
using System;

namespace Core8.Floppy.States
{
    internal class Idle : StateBase
    {
        public Idle(IController controller) : base(controller)
        {
            done = true;
        }

        protected override int LoadCommand(int acc)
        {
            Controller.CR.SetCR(acc);

            StateBase newState;

            switch (Controller.CR.CurrentFunction)
            {
                case ControllerFunction.FillBuffer:
                    newState = new FillBuffer(this.Controller);
                    break;
                case ControllerFunction.NoOperation:
                    newState = new NoOperation(this.Controller);
                    break;
                case ControllerFunction.EmptyBuffer:
                    newState = new EmptyBuffer(this.Controller);
                    break;
                case ControllerFunction.WriteSector:
                    newState = new ReadWriteSector(this.Controller, false);
                    break;
                case ControllerFunction.ReadSector:
                    newState = new ReadWriteSector(this.Controller);
                    break;
                case ControllerFunction.ReadStatus:
                case ControllerFunction.WriteDeletedDataSector:
                case ControllerFunction.ReadErrorRegister:
                    throw new NotImplementedException(Controller.CR.CurrentFunction.ToString());
                default:
                    throw new NotImplementedException(Controller.CR.CurrentFunction.ToString());
            }

            Controller.SetState(newState);

            return 0;
        }
    }
}
