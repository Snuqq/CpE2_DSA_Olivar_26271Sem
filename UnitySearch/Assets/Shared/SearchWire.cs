using System;
using System.IO;
using System.Text;

namespace CpE2_DSA_Olivar_26271Sem
{
    public static class SearchWire
    {
        public const int Version = 2;
        public static string Read(Stream stream)
        {
            byte[] size = Exact(stream, 4);
            int count = BitConverter.ToInt32(size, 0);
            if (count < 0 || count > 65536) throw new InvalidDataException("Invalid message length.");
            return new UTF8Encoding(false, true).GetString(Exact(stream, count));
        }
        static byte[] Exact(Stream stream, int count)
        {
            byte[] bytes = new byte[count];
            int offset = 0;
            while (offset < count)
            {
                int read = stream.Read(bytes, offset, count - offset);
                if (read == 0) throw new EndOfStreamException();
                offset += read;
            }
            return bytes;
        }
        public static void Write(Stream stream, string message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            if (bytes.Length > 65536) throw new InvalidDataException("Message too large.");
            byte[] size = BitConverter.GetBytes(bytes.Length);
            stream.Write(size, 0, size.Length); stream.Write(bytes, 0, bytes.Length); stream.Flush();
        }
    }
}
