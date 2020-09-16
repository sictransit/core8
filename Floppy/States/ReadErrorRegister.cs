using Core8.Peripherals.Floppy.Interfaces;
using Core8.Peripherals.Floppy.States.Abstract;

namespace Core8.Peripherals.Floppy.States
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
