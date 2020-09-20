using System;

namespace Core8.Extensions
{
    public static class IntExtensions
    {
        public static int ToDecimal(this int oct)
        {
            return Convert.ToInt32(oct.ToString(), 8);
        }

        public static int ToOctal(this int dec)
        {
            return dec == 0 ? 0 : dec % 8 + 10 * (dec / 8).ToOctal();
        }

        public static string ToOctalString(this int dec, int digits = 4)
        {
            return dec.ToOctal().ToString($"d{digits}");
        }
    }
}
