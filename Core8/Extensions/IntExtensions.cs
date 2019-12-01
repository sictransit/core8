using System;
using System.Collections.Generic;
using System.Text;

namespace Core8.Extensions
{
    public static class IntExtensions
    {
        public static uint ToDecimal(this int oct)
        {
            return Convert.ToUInt32(oct.ToString(), 8);
        }

        public static uint ToOctal(this uint dec)
        {
            if (dec == 0)
            {
                return 0;
            }

            return dec % 8 + 10 * (dec / 8).ToOctal();
        }
    }
}
