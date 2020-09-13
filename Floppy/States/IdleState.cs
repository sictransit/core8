using Core8.Floppy.States.Abstract;

namespace Core8.Floppy.States
{
    internal class IdleState : StateBase
    {
        public IdleState()
        {
            Done = true;
        }
        public override bool Done { get; }
    }
}
