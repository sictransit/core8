using Core8.Model.Interfaces;

namespace Core8.Peripherals.RK8E
{
    public class FixedDisk : IFixedDisk
    {
        public bool InterruptRequested => throw new NotImplementedException();

        public void Tick()
        {
            throw new NotImplementedException();
        }
    }
}