using System.Threading.Tasks;

namespace BL
{
    public interface IHuffmanService
    {
        Task EncodeAsync(string filePath);
        Task DecodeAsync(string filePath);
    }
}