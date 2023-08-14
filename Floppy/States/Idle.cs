using Core8.Peripherals.RX8E.Declarations;
using Core8.Peripherals.RX8E.Interfaces;
using Core8.Peripherals.RX8E.States.Abstract;
using System;

namespace Core8.Peripherals.RX8E.States
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
            //Controller.SetError(false);

            Controller.CR.Set(acc);

            StateBase newState = Controller.CR.CurrentFunction switch
            {
                ControllerFunction.FILL_BUFFER => new FillBuffer(Controller),
                ControllerFunction.NO_OPERATION => new NoOperation(Controller),
                ControllerFunction.EMPTY_BUFFER => new EmptyBuffer(Controller),
                ControllerFunction.WRITE_SECTOR => new WriteSector(Controller),
                ControllerFunction.READ_SECTOR => new ReadSector(Controller),
                ControllerFunction.READ_ERROR_REGISTER => new ReadErrorRegister(Controller),
                ControllerFunction.READ_STATUS => new ReadStatus(Controller),
                _ => throw new NotImplementedException(Controller.CR.CurrentFunction.ToString()),
            };

            Controller.SetState(newState);
        }
    }
}
