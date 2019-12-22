﻿using System;

namespace Core8.Model.Extensions
{
    public static class IntExtensions
    {
        public static uint ToDecimal(this uint oct)
        {
            return Convert.ToUInt32(oct.ToString(), 8);
        }

        public static uint ToOctal(this uint dec)
        {
            return dec == 0 ? 0 : dec % 8 + 10 * (dec / 8).ToOctal();
        }

        public static string ToOctalString(this uint dec)
        {
            return dec.ToOctal().ToString("d4");
        }
    }


}