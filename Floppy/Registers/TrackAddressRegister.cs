using Core8.Model.Registers.Abstract;

namespace Core8.Peripherals.Floppy.Registers
{
    internal class TrackAddressRegister : RegisterBase
    {
        protected override string ShortName => "TA";

        public override void Set(int value) => Content = value & 0b_000_011_111_111;
    }
}
