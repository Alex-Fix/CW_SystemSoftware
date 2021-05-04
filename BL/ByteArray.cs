using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    internal class ByteArray
    {
        internal IEnumerable<byte> Bytes { get; set; }

        public ByteArray(IEnumerable<byte> bytes)
        {
            this.Bytes = bytes;
        }

        public override bool Equals(object obj)
        {
            var byteKey = obj as ByteArray;
            if (byteKey is null) return false;

            return string.Equals(JsonConvert.SerializeObject(this.Bytes), JsonConvert.SerializeObject(byteKey.Bytes));
        }

        public override int GetHashCode()
        {
            return JsonConvert.SerializeObject(this.Bytes).GetHashCode();
        }
    }
}
