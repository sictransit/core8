using Core8.Floppy.Declarations;
using Core8.Floppy.Interfaces;
using Serilog;
using System;

namespace Core8.Floppy.States.Abstract
{
    internal abstract class StateBase
    {
        protected bool done;
        protected bool transferRequest;
        protected bool error;

        private readonly DateTime stateChangeDue;

        public StateBase(IController controller)
        {
            Controller = controller ?? throw new ArgumentNullException(nameof(controller));

            error = transferRequest = done = controller.MaintenanceMode;

            stateChangeDue = DateTime.UtcNow + StateLatency;
        }

        public bool IRQ => done;

        protected IController Controller { get; }

        protected virtual TimeSpan StateLatency => Latencies.CommandTime;

        private bool IsStateChangeDue => DateTime.UtcNow > stateChangeDue;

        protected virtual bool EndState() => false;

        public virtual void Tick()
        {
            if (IsStateChangeDue && EndState())
            {
                Controller.SetState(new Idle(Controller));
            }
        }

        protected virtual int LoadCommand(int acc) => acc;

        public int LCD(int acc)
        {
            if (done)
            {
                Log.Warning("LCD with DF set!");
            }

            return LoadCommand(acc);
        }

        protected virtual int TransferData(int acc) => acc;

        public int XDR(int acc)
        {
            if (transferRequest)
            {
                Log.Warning("XDR with TR high");
            }

            return TransferData(acc);
        }

        public virtual bool SND()
        {
            if (done)
            {
                done = Controller.MaintenanceMode;

                return true;
            }

            return false;
        }


        public virtual bool STR()
        {
            if (transferRequest)
            {
                transferRequest = Controller.MaintenanceMode;

                return true;
            }

            return false;
        }

        public override string ToString() => $"{GetType().Name} dne={done} tr={transferRequest} err={error}";
    }
}
