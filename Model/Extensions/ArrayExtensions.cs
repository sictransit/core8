using System;
using System.Collections.Generic;
using System.Text;

namespace Core8.Model.Extensions
{
    public static class ArrayExtensions
    {
        public static IEnumerable<int> Pack(this byte[] data, int length = 64)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            for (int i = 0; i < length / 2 * 3; i += 3)
            {
                yield return data[i] << 4 | data[i + 1] >> 4;

                yield return (data[i + 1] & 0b_001_111) << 8 | data[i + 2];
            }
        }

        public static IEnumerable<byte> Unpack(this int[] data, int length = 96, int pad = 128)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            for (int i = 0; i < length / 3 * 2; i++)
            {
                if (i % 2 == 0)
                {
                    yield return (byte)(data[i] >> 4);
                    yield return (byte)((data[i] & 0b_001_111) << 4 | data[i + 1] >> 8 & 0b_001_111);
                }
                else
                {
                    yield return (byte)(data[i] & 0b_011_111_111);
                }
            }

            for (int i = length; i < pad; i++)
            {
                yield return 0;
            }
        }
    }
}
