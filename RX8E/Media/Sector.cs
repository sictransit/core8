using Core8.Peripherals.RX8E.Declarations;
using Core8.Peripherals.RX8E.Media.Abstract;

namespace Core8.Peripherals.RX8E.Media;

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
