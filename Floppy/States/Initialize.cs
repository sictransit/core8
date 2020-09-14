using Core8.Floppy.Interfaces;
using Core8.Floppy.States.Abstract;

namespace Core8.Floppy.States
{
    internal class Initialize : StateBase
    {
        public Initialize(IController controller, IDrive drive) : base(controller, drive)
        {

        }
    }
}
