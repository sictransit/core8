using Core8.Peripherals.Floppy.Declarations;
using Core8.Peripherals.Floppy.Media.Abstract;

namespace Core8.Peripherals.Floppy.Media
{
    internal class Sector : MediaBase
    {
        public Sector(int number) : base(number)
        {

        }

        public byte[] Data { get; set; }

        public override void Format()
        {
            Data = new byte[DiskLayout.BLOCK_SIZE];
        }
    }
}
