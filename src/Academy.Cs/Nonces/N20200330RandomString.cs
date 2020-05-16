using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Academy.Cs.Nonces
{
    /// <summary>
    /// Writes random data to a file.
    /// </summary>
    [TestClass]
    public class N20200330RandomString
    {
        [TestMethod]
        public void Main()
        {
            const string OutputPath = @"output.txt";

            const int WordCount = 1000000;
            const int WordByteLength = 4;
            const int MaxLineLength = 100;
            const int WordsPerLine = MaxLineLength / (WordByteLength * 2);

            Assert.IsFalse(File.Exists(OutputPath), "Output file already exists; cannot overwrite.");

            Random random = new Random();
            byte[] bytes = new byte[WordByteLength];
            using (StreamWriter writer = File.CreateText(OutputPath))
            {
                for (int i = 0; i < WordCount; i++)
                {
                    random.NextBytes(bytes);
                    string word = ByteConverter.ToHex(bytes);
                    if (i % WordsPerLine == WordsPerLine - 1)
                    {
                        writer.WriteLine(word);
                    }
                    else
                    {
                        writer.Write(word + " ");
                    }
                }
            }
        }

        /// <summary>
        /// Converts raw bytes to a string representation.
        /// </summary>
        /// <seealso cref="https://stackoverflow.com/questions/311165/how-do-you-convert-a-byte-array-to-a-hexadecimal-string-and-vice-versa/24343727#24343727"/>
        private static class ByteConverter
        {
            private static readonly uint[] _lookup32 = CreateLookup32();

            public static string ToHex(byte[] bytes)
            {
                uint[] lookup32 = _lookup32;
                char[] result = new char[bytes.Length * 2];
                for (int i = 0; i < bytes.Length; i++)
                {
                    uint val = lookup32[bytes[i]];
                    result[2 * i] = (char)val;
                    result[2 * i + 1] = (char)(val >> 16);
                }

                return new string(result);
            }

            private static uint[] CreateLookup32()
            {
                uint[] result = new uint[256]; // 32 bits / 4 bytes = 8 bits/byte; 2^8 is 256
                for (int i = 0; i < 256; i++)   // so for ever bit combo in a byte
                {
                    string s = i.ToString("x2");
                    result[i] = ((uint)s[0]) + ((uint)s[1] << 16);
                }

                return result;
            }
        }
    }
}