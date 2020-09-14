using Core8.Floppy.Interfaces;
using Core8.Floppy.States.Abstract;

namespace Core8.Floppy.States
{
    internal class FillBuffer : StateBase
    {
        public FillBuffer(IController controller, IDrive drive) : base(controller, drive)
        {

        }
    }
}
