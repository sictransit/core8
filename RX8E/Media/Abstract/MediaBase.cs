namespace Core8.Peripherals.RX8E.Media.Abstract;

internal abstract class MediaBase
{
    protected MediaBase(int number)
    {
        Number = number;
    }

    public int Number { get; }

    public abstract void Format();

    public override string ToString() => $"{GetType().Name} {Number}";
}
