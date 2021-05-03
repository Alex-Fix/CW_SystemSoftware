using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    internal class MetaData
    {
        [JsonProperty("file_name")]
        internal string FileName { get; set; }
        [JsonProperty("block_size")]
        internal int BlockSize { get; set; }
        [JsonProperty("compression_bytes")]
        internal IDictionary<ByteArray, ByteArray> CompressionBytes { get; set; }
    }
}
