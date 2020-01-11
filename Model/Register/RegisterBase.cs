﻿using Serilog;

namespace Core8.Model.Register
{
    public abstract class RegisterBase
    {
        public virtual uint Data { get; private set; }

        protected void SetRegister(uint data)
        {
            Data = data;            
        }

        public void Clear()
        {
            SetRegister(0);
        }

    }
}
