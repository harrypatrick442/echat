using System.Collections;
using System;
using System.Runtime.InteropServices;

namespace Core.Strings
{
    public static class StringExtensions
    {
        public static long ToNonCryptographicHash(this string str)
        {
            var inputBytes = MemoryMarshal.AsBytes(str.AsSpan());
            var hash = System.IO.Hashing.XxHash32.Hash(inputBytes);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(hash);
            long result =  BitConverter.ToUInt32(hash, 0);
            return result;
        }
    }
}
