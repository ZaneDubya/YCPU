using System;
using System.IO;
using System.IO.Compression;

namespace Ypsilon.Core.IO
{
    public static class Compression
    {
        public static void WriteCompressedData(BinaryFileWriter writer, byte[] data, bool prefixUncompressedSize, bool prefixCompressedSize)
        {
            byte[] compressed = Compress(data);
            if (prefixUncompressedSize)
                writer.WriteEncodedInt(data.Length);
            if (prefixCompressedSize)
                writer.WriteEncodedInt(compressed.Length);
            writer.Write(compressed, 0, compressed.Length);
        }

        public static byte[] Compress(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (DeflateStream zip = new DeflateStream(ms, CompressionMode.Compress, true))
                {
                    zip.Write(buffer, 0, buffer.Length);
                    zip.Close();
                }
                ms.Position = 0;
                byte[] compressed = new byte[ms.Length];
                ms.Read(compressed, 0, compressed.Length);
                byte[] gzBuffer = new byte[compressed.Length];
                Buffer.BlockCopy(compressed, 0, gzBuffer, 0, compressed.Length);
                return gzBuffer;
            }
        }

        public static byte[] Decompress(uint uncompressedSize, byte[] gzBuffer)
        {
            MemoryStream ms = new MemoryStream(gzBuffer);
            byte[] buffer = new byte[uncompressedSize];
            ms.Position = 0;
            using (DeflateStream zip = new DeflateStream(ms, CompressionMode.Decompress))
            {
                zip.Read(buffer, 0, buffer.Length);
            }
            return buffer;
        }

    }
}
