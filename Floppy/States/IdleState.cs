using Core8.Floppy.States.Abstract;

namespace Core8.Floppy.States
{
    internal class IdleState : StateBase
    {
        private bool done;

        public IdleState()
        {
            done = true;
        }        

        public override int LCD(int acc)
        {
            if (done)
            {
                return base.LCD(acc);
            }

            return base.LCD(acc);
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
