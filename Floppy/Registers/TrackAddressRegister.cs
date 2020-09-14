using Core8.Model.Register.Abstract;

namespace Core8.Floppy.Registers
{
    internal class TrackAddressRegister : RegisterBase
    {
        protected override string ShortName => "TA";

        public void SetTAR(int value) => Content = value & 0b_000_011_111_111;
    }
}
