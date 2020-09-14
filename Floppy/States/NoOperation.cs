using Core8.Floppy.Interfaces;
using Core8.Floppy.States.Abstract;

namespace Core8.Floppy.States
{
    internal class NoOperation : StateBase
    {
        public NoOperation(IController controller, IDrive drive) : base(controller, drive)
        {

        }

        protected override bool EndState() => true;
    }
}
