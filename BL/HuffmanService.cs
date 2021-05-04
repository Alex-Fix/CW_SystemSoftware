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
            ValidateFilePath(filePath);

            var fileInfo = new FileInfo(filePath);
            if (!fileInfo.Name.Contains(".huff")) throw new ArgumentException(nameof(filePath));

            using var inputStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var metaData = GetMetaData(inputStream);
            var huffmanCode = metaData.CompressionBytes.ToHuffmanCode();

            string outputPath = fileInfo.DirectoryName + "\\Decoded_" + metaData.FileName;
            using var outputStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write);

            await DecodeAsync(huffmanCode, inputStream, outputStream);
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
                CompressionBytes = huffmanCode.ToDictionary(el => JsonConvert.SerializeObject(el.Key.Bytes), el => el.Value.Bytes.ToArray())
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

        private async Task DecodeAsync(IEnumerable<ByteArrayPair> huffmanCode, FileStream inputStream, FileStream outputStream)
        {
            var buffer = new ByteArray(new List<byte>());
            while (inputStream.Position != inputStream.Length)
            {
                buffer.Bytes.Add((byte)inputStream.ReadByte());
                var code = huffmanCode.FirstOrDefault(hc => hc.Key.Equals(buffer));
                if(code != null)
                {
                    await outputStream.WriteAsync(code.Value.Bytes.ToArray());
                    buffer.Bytes.Clear();
                }
            }
        }

        private MetaData GetMetaData(FileStream inputStream)
        {
            var bytes = new List<byte>();
            while (true)
            {
                var oneByte = inputStream.ReadByte();
                if (oneByte == 0) break;
                bytes.Add((byte)oneByte);
            }
            var metaDataStr = Encoding.UTF8.GetString(bytes.ToArray());
            return JsonConvert.DeserializeObject<MetaData>(metaDataStr);
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
