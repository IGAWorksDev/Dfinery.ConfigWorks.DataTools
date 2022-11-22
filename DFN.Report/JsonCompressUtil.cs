using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace DFN.Report
{
    public static class JsonCompressUtil
    {
        public static byte[] SerializeObject(Object o)
        {
            string ser = JsonConvert.SerializeObject(o);
            return Compress(ser);
        }

        public static T DeserializeObject<T>(byte[] data)
        {
            string decom = Decompress(data);
            return JsonConvert.DeserializeObject<T>(decom);
        }

        public static Byte[] Compress(string data)
        {
            Byte[] buffer = Encoding.Unicode.GetBytes(data);
            using (MemoryStream ms = new MemoryStream())
            {
                using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Compress))
                {
                    ds.Write(buffer, 0, buffer.Length);
                }
                Byte[] compressedByte = ms.ToArray();
                return compressedByte;
            }
        }
        public static string Decompress(byte[] buffer)
        {
            MemoryStream resultStream = new MemoryStream();
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress))
                {
                    ds.CopyTo(resultStream);
                    ds.Close();
                }
            }
            Byte[] decompressedByte = resultStream.ToArray();
            resultStream.Dispose();
            return Encoding.Unicode.GetString(decompressedByte);
        }
    }
}
