using Core8.Floppy.Interfaces;
using Core8.Floppy.States.Abstract;

namespace Core8.Floppy.States
{
    internal class ReadErrorRegister : StateBase
    {
        public ReadErrorRegister(IController controller) : base(controller)
        {

        }

        protected override void SetIR()
        {
            Controller.IR.SetIR(Controller.IR.Content << 8 | Controller.EC.Content);
        }
    }
}
