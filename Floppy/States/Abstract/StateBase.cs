using Core8.Peripherals.Floppy.Declarations;
using Core8.Peripherals.Floppy.Interfaces;
using Serilog;
using System;

namespace Core8.Peripherals.Floppy.States.Abstract
{
    internal abstract class StateBase
    {
        private readonly DateTime stateChangeDue;

        public StateBase(IController controller)
        {
            Controller = controller ?? throw new ArgumentNullException(nameof(controller));

            stateChangeDue = DateTime.UtcNow + StateLatency;
        }

        protected IController Controller { get; }

        protected virtual TimeSpan StateLatency => Latencies.CommandTime;

        private bool IsStateChangeDue => DateTime.UtcNow > stateChangeDue;

        protected virtual bool FinalizeState() => false;

        protected virtual void SetIR() => Controller.IR.SetIR(Controller.ES.Content);

        public virtual void Tick()
        {
            if (IsStateChangeDue && FinalizeState())
            {
                Controller.SetState(new Idle(Controller));

                SetIR();
            }
        }

        protected virtual void LoadCommand(int acc)
        {
            Log.Warning("LCD in state when not expected");
        }

        public void LCD(int acc)
        {
            if (Controller.Done)
            {
                Log.Warning("LCD with DF set!");
            }

            LoadCommand(acc);
        }

        protected virtual int TransferData(int acc)
        {
            return acc;
        }

        public int XDR(int acc)
        {
            if (Controller.TransferRequest)
            {
                Log.Warning("XDR with TR high");
            }

            return TransferData(acc);
        }

        public override string ToString() => $"{GetType().Name}";
    }
}
