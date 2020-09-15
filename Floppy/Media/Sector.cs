using Core8.Floppy.Declarations;
using Core8.Floppy.Media.Abstract;

namespace Core8.Floppy.Media
{
    internal class Sector : MediaBase
    {
        public Sector(int number) : base(number)
        {

        }

        public byte[] Data { get; set; }

        public override void Format()
        {
            Data = new byte[DiskLayout.BlockSize];
        }
    }
}
