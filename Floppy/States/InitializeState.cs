using Core8.Floppy.Interfaces;
using Core8.Floppy.States.Abstract;

namespace Core8.Floppy.States
{
    internal class InitializeState : StateBase
    {
        public InitializeState(IController controller) : base(controller)
        {

        }
    }
}
