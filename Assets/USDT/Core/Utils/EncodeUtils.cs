﻿using System;
using System.Collections.Generic;
using System.Text;

namespace USDT.Utils {

    /// <summary>
    /// 编解码工具
    /// </summary>
    public static class EncodeUtils
    {
        #region Hex

        public readonly static Dictionary<char, byte> hexmap = new Dictionary<char, byte>()
        {
            { 'a', 0xA },{ 'b', 0xB },{ 'c', 0xC },{ 'd', 0xD },
            { 'e', 0xE },{ 'f', 0xF },{ 'A', 0xA },{ 'B', 0xB },
            { 'C', 0xC },{ 'D', 0xD },{ 'E', 0xE },{ 'F', 0xF },
            { '0', 0x0 },{ '1', 0x1 },{ '2', 0x2 },{ '3', 0x3 },
            { '4', 0x4 },{ '5', 0x5 },{ '6', 0x6 },{ '7', 0x7 },
            { '8', 0x8 },{ '9', 0x9 }
        };

        public static byte[] ToBytes(this string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
                throw new ArgumentException("Hex cannot be null/empty/whitespace");

            if (hex.Length % 2 != 0)
                throw new FormatException("Hex must have an even number of characters");

            bool startsWithHexStart = hex.StartsWith("0x", StringComparison.OrdinalIgnoreCase);

            if (startsWithHexStart && hex.Length == 2)
                throw new ArgumentException("There are no characters in the hex string");


            int startIndex = startsWithHexStart ? 2 : 0;

            byte[] bytesArr = new byte[(hex.Length - startIndex) / 2];

            char left;
            char right;

            try
            {
                // 每2个字符转化为一个字节
                int x = 0;
                for (int i = startIndex; i < hex.Length; i += 2, x++)
                {
                    left = hex[i];
                    right = hex[i + 1];
                    bytesArr[x] = (byte)((hexmap[left] << 4) | hexmap[right]);
                }
                return bytesArr;
            }
            catch (KeyNotFoundException)
            {
                throw new FormatException("Hex string has non-hex character");
            }
        }
        public static string ToHexString(this byte[] bytes)
        {
            return BitConverter.ToString(bytes, 0).Replace("-", string.Empty).ToLower();
        }

        public static string ToHex(this byte b) {
            return b.ToString("X2");
        }

        public static string ToHex(this byte[] bytes) {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in bytes) {
                stringBuilder.Append(b.ToString("X2"));
            }
            return stringBuilder.ToString();
        }

        public static string ToHex(this byte[] bytes, string format) {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in bytes) {
                stringBuilder.Append(b.ToString(format));
            }
            return stringBuilder.ToString();
        }

        public static string ToHex(this byte[] bytes, int offset, int count) {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = offset; i < offset + count; ++i) {
                stringBuilder.Append(bytes[i].ToString("X2"));
            }
            return stringBuilder.ToString();
        }

        #endregion

        public static string BytesToString(byte[] bytes) {
            return Encoding.UTF8.GetString(bytes);
        }

        public static byte[] StringToBytes(string str) {
            return Encoding.UTF8.GetBytes(str.ToCharArray());
        }

        public static byte[] GetBytes(this string s) {
            return Encoding.UTF8.GetBytes(s);
        }

        /// <summary>
        /// 将List<long>转为byte[]
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>

        public static unsafe byte[] GetBytes(List<long> values) {
            byte[] bytes = new byte[values.Count * sizeof(long)];
            fixed (byte* ptr = bytes) {
                long* longPtr = (long*)ptr;
                for (int i = 0; i < values.Count; i++) {
                    *longPtr = values[i];
                    longPtr++;
                }
            }
            return bytes;
        }
    }
}