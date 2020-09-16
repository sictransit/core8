using Core8.Peripherals.Floppy.Declarations;
using Core8.Peripherals.Floppy.Interfaces;
using Core8.Peripherals.Floppy.States.Abstract;
using System;

namespace Core8.Peripherals.Floppy.States
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
                ControllerFunction.FillBuffer => new FillBuffer(Controller),
                ControllerFunction.NoOperation => new NoOperation(Controller),
                ControllerFunction.EmptyBuffer => new EmptyBuffer(Controller),
                ControllerFunction.WriteSector => new WriteSector(Controller),
                ControllerFunction.ReadSector => new ReadSector(Controller),
                ControllerFunction.ReadErrorRegister => new ReadErrorRegister(Controller),
                _ => throw new NotImplementedException(Controller.CR.CurrentFunction.ToString()),
            };

            Controller.SetState(newState);
        }
    }
}
