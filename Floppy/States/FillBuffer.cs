using Core8.Floppy.Interfaces;
using Core8.Floppy.States.Abstract;

namespace Core8.Floppy.States
{
    internal class FillBuffer : BusyState
    {
        public FillBuffer(IController controller) : base(controller)
        {

        }
    }
}
