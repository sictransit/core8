using Core8.Model.Interfaces;
using Core8.Model.Registers;

namespace Core8.Peripherals.RK8E
{
    public class FixedDisk : IFixedDisk
    {
        public bool InterruptRequested => throw new NotImplementedException();

        private int addressRegister;

        public void LoadCurrentAddress(LinkAccumulator lac)
        {
            addressRegister = lac.Accumulator;

            lac.Clear();
        }

        public void Tick()
        {
            //throw new NotImplementedException();
        }
    }
}