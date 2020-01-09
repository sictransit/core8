namespace Core8.Model.Register
{
    public abstract class RegisterBase
    {
        public uint Data { get; protected set; }

        public void Clear()
        {
            Data = 0;
        }

    }
}
