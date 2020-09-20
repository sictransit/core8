using System;
using System.Collections.Generic;
using System.Linq;

namespace Core8.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> ChunkBy<T>(this IEnumerable<T> items, int size)
        {
            if (items is null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            if (size <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            return items.Select((item, index) => new { item, index }).GroupBy(x => x.index / size).Select(x => x.Select(y => y.item));
        }
    }
}
