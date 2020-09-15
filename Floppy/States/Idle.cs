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
            Controller.SetDone(true);
        }

        protected override int TransferData(int acc)
        {
            return Controller.IR.Content;
        }

        protected override void LoadCommand(int acc)
        {
            Controller.CR.SetCR(acc);
            
            StateBase newState = Controller.CR.CurrentFunction switch
            {
                ControllerFunction.FillBuffer => new FillBuffer(this.Controller),
                ControllerFunction.NoOperation => new NoOperation(this.Controller),
                ControllerFunction.EmptyBuffer => new EmptyBuffer(this.Controller),
                ControllerFunction.WriteSector => new ReadWriteSector(this.Controller, false),
                ControllerFunction.ReadSector => new ReadWriteSector(this.Controller),
                ControllerFunction.ReadErrorRegister => new ReadErrorRegister(this.Controller),
                _ => throw new NotImplementedException(Controller.CR.CurrentFunction.ToString()),
            };

            Controller.SetState(newState);            
        }
    }
}
