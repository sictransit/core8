﻿using Core8.Model.Extensions;

namespace Core8.Model.Register
{
    public class DF : RegisterBase
    {
        public void SetDF(uint value)
        {
            Set(value & Masks.DF);
        }

        public override string ToString()
        {
            return string.Format($"{base.ToString()} {Data.ToOctalString()}");
        }
    }
}
