namespace Core8.Model.Register.Abstract
{
    public abstract class RegisterBase
    {
        public int Content { get; protected set; }

        protected abstract string ShortName { get; }

        public void Clear()
        {
            Content = 0;
        }

        public override string ToString()
        {
            return $"[{ShortName}]";
        }
    }
}
