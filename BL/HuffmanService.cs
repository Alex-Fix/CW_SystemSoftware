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

            var huffmanCode = byteArrays.GroupBy(cb => cb).Select(g => new GroupedByteArray
            {
                ByteArray = g.Key,
                Count = g.LongCount()
            })
            .OrderByDescending(g => g.Count).ToList()
            .CalculateFreequency().ToList()
            .CalculateCodeLoop().ToList()
            .ToHuffmanCode();

            var metaData = new MetaData 
            { 
                BlockSize = blockSize,
                FileName = fileInfo.Name,
                CompressionBytes = huffmanCode
            };

            await EncodeAsync(metaData, byteArrays, huffmanCode, outputStream);
        }

        private async Task EncodeAsync(MetaData metaData, IEnumerable<ByteArray> byteArrays, IDictionary<ByteArray, ByteArray> huffmanCode, 
            FileStream ouputStream)
        {
            // metaData
            await ouputStream.WriteAsync(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(metaData) + "\0"));
            var bytesToWrite = new List<byte>();

            foreach(var byteArray in byteArrays)
            {
                var code = huffmanCode.First(hc => byteArray.Equals(hc.Key));
                bytesToWrite.AddRange(code.Value.Bytes);
            }

            foreach(var chunk in bytesToWrite.Chunk(100000))
                await ouputStream.WriteAsync(chunk.ToArray());
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
