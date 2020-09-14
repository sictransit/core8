using Core8.Floppy.Interfaces;

namespace Core8.Floppy.States.Abstract
{
    internal abstract class StateBase
    {
        protected bool done;
        protected bool transferRequest;
        protected bool error;

        public StateBase(IController controller, IDrive drive)
        {
            Controller = controller ?? throw new System.ArgumentNullException(nameof(controller));
            Drive = drive ?? throw new System.ArgumentNullException(nameof(drive));

            error = transferRequest = done = controller.MaintenanceMode;
        }

        protected IController Controller { get; }

        protected IDrive Drive { get; }

        public virtual void Tick() { }

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
    }
}
