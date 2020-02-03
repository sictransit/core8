using System;

namespace Core8.Model.Extensions
{
    public static class IntExtensions
    {
        public static uint ToDecimal(this int oct)
        {
            return ((uint)oct).ToDecimal();
        }

        public static uint ToDecimal(this uint oct)
        {
            return Convert.ToUInt32(oct.ToString(), 8);
        }

        public static uint ToOctal(this uint dec)
        {
            return dec == 0 ? 0 : dec % 8 + 10 * (dec / 8).ToOctal();
        }

        public static string ToOctalString(this uint dec, int digits = 4)
        {
            return dec.ToOctal().ToString($"d{digits}");
        }
    }


}
