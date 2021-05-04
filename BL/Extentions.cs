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
            long pageCount = (long)Math.Ceiling(((decimal)source.LongCount()) / size);
            for (int pageIndex = 0; pageIndex < pageCount; ++pageIndex)
                yield return source.Skip(pageIndex * size).Take(size).ToList();
        }

        internal static IEnumerable<ByteArray> ToByteArray(this IEnumerable<IEnumerable<byte>> source)
        {
            return source.Select(s => new ByteArray(s.ToList())).ToList();
        }

        internal static IEnumerable<GroupedByteArray> CalculateFreequency(this IEnumerable<GroupedByteArray> array)
        {
            long count = array.Sum(a => a.Count);
            foreach (var item in array)
                item.Freequency = ((decimal)item.Count) / count;
            return array;
        }

        internal static IEnumerable<GroupedByteArray> CalculateCode(this IEnumerable<GroupedByteArray> groupedBytes, string code = "")
        {
            if (groupedBytes.Count() < 2) return groupedBytes;

            decimal allFreequency = groupedBytes.Sum(gb => gb.Freequency);
            decimal accumulator = default;

            var firstPart = new List<GroupedByteArray>();
            var secondPart = new List<GroupedByteArray>();

            foreach(var groupedByte in groupedBytes)
            {
                if (accumulator + groupedByte.Freequency <= allFreequency / 2)
                {
                    groupedByte.Code = code + "1";
                    firstPart.Add(groupedByte);
                }
                else
                {
                    groupedByte.Code = code + "0";
                    secondPart.Add(groupedByte);
                }
                accumulator += groupedByte.Freequency;
            }

            firstPart.CalculateCode(code + "1");
            secondPart.CalculateCode(code + "0");

            return groupedBytes;
        }
    }
}
