using Core8.Floppy.Declarations;
using Core8.Floppy.Interfaces;
using System;

namespace Core8.Floppy.States.Abstract
{
    internal abstract class StateBase
    {
        protected bool done;
        protected bool transferRequest;
        protected bool error;

        private readonly DateTime stateChangeDue;

        public StateBase(IController controller, IDrive drive)
        {
            Controller = controller ?? throw new ArgumentNullException(nameof(controller));
            Drive = drive ?? throw new ArgumentNullException(nameof(drive));

            error = transferRequest = done = controller.MaintenanceMode;

            stateChangeDue = DateTime.UtcNow + StateLatency;
        }

        protected IController Controller { get; }

        protected IDrive Drive { get; }

        protected virtual TimeSpan StateLatency => Latencies.CommandTime; 
        
        private bool IsStateChangeDue => DateTime.UtcNow > stateChangeDue;

        protected virtual bool EndState() => false;

        public virtual void Tick() 
        {
            if (IsStateChangeDue && EndState())
            {
                Controller.SetState(new Idle(Controller, Drive));
            }
        }        

        public virtual int LCD(int acc) => acc;

        public virtual int XDR(int acc) => acc;

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

        public virtual bool SER()
        {
            if (error)
            {
                error = Controller.MaintenanceMode;

                return true;
            }

            return false;
        }

        public override string ToString()
        {
            return $"{this.GetType().Name} dne={done} tr={transferRequest} err={error}";
        }
    }
}
