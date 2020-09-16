namespace Core8.Peripherals.Floppy.Media.Abstract
{
    internal abstract class MediaBase
    {
        public MediaBase(int number)
        {
            Number = number;
        }

        public int Number { get; }

        public abstract void Format();

        public override string ToString() => $"{GetType().Name} {Number}";
    }
}
