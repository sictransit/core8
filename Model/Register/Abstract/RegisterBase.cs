using Serilog;

namespace Core8.Model.Register.Abstract
{
    public abstract class RegisterBase
    {
        public uint Content { get; private set; }

        protected abstract string ShortName { get; }

        protected void Set(uint value)
        {
            Content = value;

            if (Log.IsEnabled(Serilog.Events.LogEventLevel.Verbose))
            {
                Log.Verbose(ToString());
            }
        }

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
