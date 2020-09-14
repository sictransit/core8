using Core8.Model.Register.Abstract;

namespace Core8.Floppy.Registers
{
    internal class SectorAddressRegister : RegisterBase
    {
        protected override string ShortName => "SA";

        public void SetSAR(int value) => Content = value & 0b_000_001_111_111;
    }
}
