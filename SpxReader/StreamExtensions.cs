using System.Text;

namespace SpxReader
{
    public static class FileStreamExtensions
    {
        public static void Skip(this FileStream fileStream, long count)
        {
            fileStream.Seek(count, SeekOrigin.Current);
        }

        public static byte[] ReadBytesAtCurrentOffset(this FileStream fileStream, int count)
        {
            var buffer = new byte[count];
            var result = fileStream.Read(buffer, 0, buffer.Length);
            if (result != count)
                throw new Exception($"Reading given amount of data failed. Got {result} bytes instead of {count}");
            return buffer;
        }

        public static string ReadStringAtCurrentOffset(this FileStream fileStream, int count, Encoding encoding)
        {
            var buffer = new byte[count];
            var result = fileStream.Read(buffer, 0, buffer.Length);
            if (result != count)
                throw new Exception($"Reading given amount of data failed. Got {result} bytes instead of {count}");
            return encoding.GetString(buffer);
        }

        public static short ReadShortAtCurrentOffset(this FileStream fileStream)
        {
            var buffer = new byte[2];
            var result = fileStream.Read(buffer, 0, 2);
            if (result != 2)
                throw new Exception($"Reading short failed.");
            return (short)(buffer[0] << 8 | buffer[1]);
        }

        public static short ReadIntAtCurrentOffset(this FileStream fileStream)
        {
            var buffer = new byte[4];
            var result = fileStream.Read(buffer, 0, 4);
            if (result != 4)
                throw new Exception($"Reading int failed.");
            return (short)(buffer[0] << 24 | buffer[1] << 16 | buffer[2] << 8 | buffer[3]);
        }

        public static byte ReadByteAtCurrentOffset(this FileStream fileStream)
        {
            var result = fileStream.ReadByte();
            if (result < 0)
                throw new Exception($"Reading byte failed.");
            return (byte)result;
        }

        public static short ReadShortAtOffset(this FileStream fileStream, long offset)
        {
            fileStream.Seek(offset, SeekOrigin.Begin);
            var buffer = new byte[2];
            var result = fileStream.Read(buffer, 0, 2);
            if (result != 2)
                throw new Exception($"Reading short at offset {offset:X16} failed.");
            return (short)(buffer[0] << 8 | buffer[1]);
        }

        public static short ReadIntAtOffset(this FileStream fileStream, long offset)
        {
            fileStream.Seek(offset, SeekOrigin.Begin);
            var buffer = new byte[4];
            var result = fileStream.Read(buffer, 0, 4);
            if (result != 4)
                throw new Exception($"Reading int at offset {offset:X16} failed.");
            return (short)(buffer[0] << 24 | buffer[1] << 16 | buffer[2] << 8 | buffer[3]);

        }

        public static byte ReadByteAtOffset(this FileStream fileStream, long offset)
        {
            fileStream.Seek(offset, SeekOrigin.Begin);
            var result = fileStream.ReadByte();
            if (result < 0)
                throw new Exception($"Reading byte at offset {offset:X16} failed.");
            return (byte)result;
        }
    }
}