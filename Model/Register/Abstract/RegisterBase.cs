using Serilog;

namespace Core8.Model.Register.Abstract
{
    public abstract class RegisterBase
    {
        public uint Data { get; private set; }

        protected abstract string ShortName { get; }

        protected void Set(uint value)
        {
            Data = value;

            if (Log.IsEnabled(Serilog.Events.LogEventLevel.Verbose))
            {
                Log.Verbose(ToString());
            }
        }

        public void Clear()
        {
            Data = 0;
        }

        public override string ToString()
        {
            return $"[{ShortName}]";
        }
    }
}
