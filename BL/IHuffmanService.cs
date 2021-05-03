using System.Threading.Tasks;

namespace BL
{
    public interface IHuffmanService
    {
        Task EncodeAsync(string filePath, int blockSize);
        Task DecodeAsync(string filePath);
    }
}