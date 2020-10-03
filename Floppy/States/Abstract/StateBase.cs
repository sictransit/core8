using Core8.Peripherals.Floppy.Interfaces;
using System;

namespace Core8.Peripherals.Floppy.States.Abstract
{
    internal abstract class StateBase
    {
        private readonly int initialTicks;

        protected StateBase(IController controller)
        {
            Controller = controller ?? throw new ArgumentNullException(nameof(controller));

            Controller.SetDone(false);
            Controller.SetTransferRequest(false);

            initialTicks = controller.Ticks;
        }

        protected IController Controller { get; }

        private const int STATE_TICKS = 30;

        private bool IsStateChangeDue => Controller.Ticks > initialTicks + STATE_TICKS;

        protected virtual bool FinalizeState() => false;

        protected virtual void SetIR() => Controller.IR.SetIR(Controller.ES.Content);

        public void Tick()
        {
            if (IsStateChangeDue && FinalizeState())
            {
                SetIR();

                Controller.SetState(new Idle(Controller));
            }
        }

        protected virtual void LoadCommand(int acc)
        {
            throw new InvalidOperationException($"LCD in state {GetType().Name}!");
        }

        public void LCD(int acc)
        {
            LoadCommand(acc);
        }

        protected virtual int TransferData(int acc)
        {
            throw new InvalidOperationException($"XDR in state {GetType().Name}!");
        }

        public int XDR(int acc)
        {
            return TransferData(acc);
        }

        public override string ToString() => $"{GetType().Name}";
    }
}
