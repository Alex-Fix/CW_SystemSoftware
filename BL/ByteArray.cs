﻿using System;
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
            if (this.Bytes == null || byteKey.Bytes == null || this.Bytes.Count() != byteKey.Bytes.Count()) return false;
            for (int i = 0; i < this.Bytes.Count(); ++i)
                if (this.Bytes.ElementAt(i) != byteKey.Bytes.ElementAt(i)) return false;
            return true;
        }

        public override int GetHashCode()
        {
            return ((IStructuralEquatable)this.Bytes).GetHashCode(EqualityComparer<byte>.Default);
        }
    }
}
