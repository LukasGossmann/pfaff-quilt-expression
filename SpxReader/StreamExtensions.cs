namespace SpxReader
{
    public static class FileStreamExtenstions
    {
        public static short ReadShortAtOffset(this FileStream fileStream, long offset)
        {
            fileStream.Seek(offset, SeekOrigin.Begin);
            var buffer = new byte[2];
            var result = fileStream.Read(buffer, 0, 2);
            if (((byte)result) != 2)
                throw new Exception($"Reading short at offset {offset:X16} failed.");
            return (short)(buffer[0] << 8 | buffer[1]);
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