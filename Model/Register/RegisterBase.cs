﻿using Serilog;

namespace Core8.Model.Register
{
    public abstract class RegisterBase
    {
        public uint Data { get; private set; }

        protected void Set(uint value)
        {
            Data = value;

            if (Log.IsEnabled(Serilog.Events.LogEventLevel.Verbose))
            {
                Log.Verbose(this.ToString());
            }
        }

        public void Clear()
        {
            Data = 0;
        }

        public override string ToString()
        {
            return $"[{GetType().Name}]";
        }
    }
}
