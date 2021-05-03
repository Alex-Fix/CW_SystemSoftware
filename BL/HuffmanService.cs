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

        public async Task EncodeAsync(string filePath)
        {
            ValidateFilePath(filePath);

            var fileInfo = new FileInfo(filePath);
            string outputPath = fileInfo.DirectoryName + "\\" + fileInfo.Name.Remove(fileInfo.Name.IndexOf('.')) + ".huff";

            using var inputStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var outputStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write);


;
        }

        private void ValidateFilePath(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) throw new ArgumentNullException(nameof(filePath));
            if (!File.Exists(filePath)) throw new ArgumentException(nameof(filePath));
        }
    }
}
