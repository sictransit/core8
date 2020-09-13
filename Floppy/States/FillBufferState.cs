using Core8.Floppy.Interfaces;
using Core8.Floppy.States.Abstract;

namespace Core8.Floppy.States
{
    internal class FillBufferState : BusyState
    {
        public FillBufferState(IController controller) : base(controller)
        {

        }
    }
}
