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

        internal static IEnumerable<GroupedByteArray> CalculateCodeRecursive(this IEnumerable<GroupedByteArray> groupedBytes, string code = "")
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

            firstPart.CalculateCodeRecursive(code + "1");
            secondPart.CalculateCodeRecursive(code + "0");

            return groupedBytes;
        }

        internal static IEnumerable<GroupedByteArray> CalculateCodeLoop(this IEnumerable<GroupedByteArray> groupedBytes)
        {
            var codeStack = new Stack<string>();
            var groupedBytesStack = new Stack<IEnumerable<GroupedByteArray>>();

            codeStack.Push("");
            groupedBytesStack.Push(groupedBytes);

            while (groupedBytesStack.Any())
            {
                var localGroupedBytes = groupedBytesStack.Pop();
                var localCode = codeStack.Pop();

                if (localGroupedBytes.Count() < 2) continue;

                decimal allFreequency = localGroupedBytes.Sum(gb => gb.Freequency);
                decimal accumulator = default;

                var firstPart = new List<GroupedByteArray>();
                var secondPart = new List<GroupedByteArray>();

                foreach (var groupedByte in localGroupedBytes)
                {
                    if (accumulator + groupedByte.Freequency <= allFreequency / 2 || localGroupedBytes.Count() == 2 && localGroupedBytes.First() == groupedByte)
                    {
                        groupedByte.Code = localCode + "1";
                        firstPart.Add(groupedByte);
                    }
                    else
                    {
                        groupedByte.Code = localCode + "0";
                        secondPart.Add(groupedByte);
                    }
                    accumulator += groupedByte.Freequency;
                }

                if(secondPart.Count() > 1)
                {
                    codeStack.Push(localCode + "0");
                    groupedBytesStack.Push(secondPart);
                }
                if(firstPart.Count() > 1)
                {
                    codeStack.Push(localCode + "1");
                    groupedBytesStack.Push(firstPart);
                }
            }

            return groupedBytes;
        }
    }
}
