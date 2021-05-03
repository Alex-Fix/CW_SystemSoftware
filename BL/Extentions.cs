using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    internal static class Extentions
    {
        internal static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int size)
        {
            while (source.Any())
            {
                yield return source.Take(size).ToList();
                source = source.Skip(size);
            }
        }

        internal static IEnumerable<ByteArray> ToByteArray(this IEnumerable<IEnumerable<byte>> source)
        {
            return source.Select(s => new ByteArray(s.ToList())).ToList();
        }
    }
}
