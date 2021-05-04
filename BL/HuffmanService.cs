using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public class HuffmanService : IHuffmanService
    {
        public async Task DecodeAsync(string filePath)
        {
            throw new NotImplementedException();
        }

        public async Task EncodeAsync(string filePath, int blockSize)
        {
            ValidateFilePath(filePath);
            ValidateBlockSize(filePath, blockSize);

            var fileInfo = new FileInfo(filePath);
            string outputPath = fileInfo.DirectoryName + "\\" + fileInfo.Name.Remove(fileInfo.Name.IndexOf('.')) + ".huff";

            using var inputStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var outputStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write);

            byte[] inputeBytes = new byte[inputStream.Length];
            await inputStream.ReadAsync(inputeBytes);

            var byteArrays = inputeBytes.Chunk(blockSize).ToByteArray();

            var groupedBytes = byteArrays.GroupBy(cb => cb).Select(g => new GroupedByteArray
            {
                ByteArray = g.Key,
                Count = g.LongCount()
            })
            .OrderByDescending(g => g.Count)
            .ToList();

            var calculatedBytes = groupedBytes.CalculateFreequency().ToList();

            var encodedBytes = calculatedBytes.CalculateCode().ToList();


        }

        private void ValidateFilePath(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentNullException(nameof(filePath));
            if (!File.Exists(filePath)) throw new ArgumentException(nameof(filePath));
        }

        private void ValidateBlockSize(string filePath, int blockSize)
        {
            var fileInfo = new FileInfo(filePath);
            if (blockSize > fileInfo.Length || blockSize < 2) throw new ArgumentException(nameof(blockSize));
        }
    }
}
