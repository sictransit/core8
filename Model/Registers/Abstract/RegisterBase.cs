using Core8.Extensions;

namespace Core8.Model.Registers.Abstract
{
    public abstract class RegisterBase
    {
        public int Content { get; protected set; }

        protected virtual int Digits => 4;

        protected abstract string ShortName { get; }

        public void Clear()
        {
            Content = 0;
        }

        public override string ToString()
        {
            return $"[{ShortName}] {Content.ToOctalString(Digits)}";
        }
    }
}
