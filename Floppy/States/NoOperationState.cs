using Core8.Floppy.Interfaces;
using Core8.Floppy.States.Abstract;

namespace Core8.Floppy.States
{
    internal class NoOperationState : StateBase
    {
        public NoOperationState(IController controller) : base(controller)
        {

        }

        public override void Tick()
        {
            Controller.SetState(new IdleState(this.Controller));
        }
    }
}
