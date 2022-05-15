using System.Runtime.InteropServices;
using System.Text;

namespace SpxReader
{
    public class SpxFile
    {
        // Offset 0x00 - 0x04
        public const string MAGIC_STRING = "%spx%";

        // Offset 0x05 - 0x09 (5 bytes)
        public byte[] Unknown1 { get; }

        // Offset 0x0A + 0x0B
        public int RemainingFileLength1 { get; private set; }

        // Offset 0x0C + 0x0D
        public short ProducedWithLength { get; private set; }

        // Starts at offset 0x0E
        public string ProducedWithString { get; private set; }

        // Offset (0x0E + ProducedWithLength) - (0x0E + ProducedWithLength + 5) (5 bytes)
        public byte[] Unknown2 { get; }

        // Offset (0x0E + ProducedWithLength + 6) + (0x0E + ProducedWithLength + 7)
        public int RemainingFileLength2 { get; private set; }

        // Offset (0x0E + ProducedWithLength + 8) - (0x0E + ProducedWithLength + 14) (7 bytes)
        public byte[] Unknown3 { get; }

        // Offset (0x0E + ProducedWithLength + 15) + (0x0E + ProducedWithLength + 16)
        public int SizeOfVariableLengthData { get; private set; }

        // Offset (0x0E + ProducedWithLength + 17) to (0x0E + ProducedWithLength + 17 + SizeOfVariableLengthData)
        public byte[] VariableLengthData { get; private set; } // Contains lots of unknown stuff

        private static readonly byte[] SPX_STRUCT_START = new byte[] { 0x01, 0x05, 0x01 };

        public SpxStruct SpxStruct { get; private set; }

        public byte[] RawStitchPositionData { get; private set; }

        private SpxFile()
        {
            Unknown1 = new byte[3];
            Unknown2 = new byte[3];
            Unknown3 = new byte[5];
        }

        public static SpxFile Open(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                var spxFile = new SpxFile();

                var magicString = stream.ReadStringAtCurrentOffset(SpxFile.MAGIC_STRING.Length, Encoding.ASCII);
                if (magicString != SpxFile.MAGIC_STRING)
                    throw new Exception($"Unexpected magic string: {magicString}");

                stream.Read(spxFile.Unknown1, 0, spxFile.Unknown1.Length);
                spxFile.RemainingFileLength1 = stream.ReadIntAtCurrentOffset();
                spxFile.ProducedWithLength = stream.ReadShortAtCurrentOffset();
                spxFile.ProducedWithString = stream.ReadStringAtCurrentOffset(spxFile.ProducedWithLength, Encoding.BigEndianUnicode);
                stream.Read(spxFile.Unknown2, 0, spxFile.Unknown2.Length);
                spxFile.RemainingFileLength2 = stream.ReadIntAtCurrentOffset();
                stream.Read(spxFile.Unknown3, 0, spxFile.Unknown3.Length);
                spxFile.SizeOfVariableLengthData = stream.ReadIntAtCurrentOffset();
                spxFile.VariableLengthData = stream.ReadBytesAtCurrentOffset(spxFile.SizeOfVariableLengthData);

                /*
                // Now we should be at the start of the SpxStruct. Usually starts with 01 05 01.
                {
                    // Backup current stream position
                    var lastStreamPosition = stream.Position;

                    // Check spx struct start sequence
                    var spxStructStart = stream.ReadBytesAtCurrentOffset(3);
                    if (!spxStructStart.SequenceEqual(SPX_STRUCT_START))
                        throw new Exception("Unexpected spx struct start sequence or misalignment");

                    // Restore old position
                    stream.Seek(lastStreamPosition, SeekOrigin.Begin);
                }
                */

                var spxStructBuffer = stream.ReadBytesAtCurrentOffset(Marshal.SizeOf<SpxStruct>());
                spxFile.SpxStruct = spxStructBuffer.ToStructure<SpxStruct>();

                spxFile.SpxStruct = spxFile.SpxStruct.ConvertEndianness();

                spxFile.RawStitchPositionData = stream.ReadBytesAtCurrentOffset(spxFile.SpxStruct.StitchArrayLength.Int);

                if (stream.ReadByte() != -1)
                    throw new Exception("Expected end of file");

                return spxFile;
            }
        }

        public LinkedList<StitchPosition> DecodeStitchPositions()
        {
            var positions = new LinkedList<StitchPosition>();
            positions.AddLast(new StitchPosition(this.SpxStruct.StartPositionX.Int, 0));

            if (this.RawStitchPositionData.Length % 2 != 0)
                throw new Exception("Expected the raw stitch position list to be a multiple of 2");

            for (int i = 0; i < this.RawStitchPositionData.Length / 2; i++)
            {
                var index = i * 2;
                var signedX = (sbyte)this.RawStitchPositionData[index];
                var signedY = (sbyte)this.RawStitchPositionData[index + 1];
                var xOffset = (signedX * SpxStruct.ResolutionX.Int);
                var yOffset = (signedY * SpxStruct.ResolutionY.Int);

                var lastPosition = positions.Last.Value;

                var currentX = lastPosition.X + xOffset;
                var currentY = lastPosition.Y + yOffset;

                positions.AddLast(new StitchPosition(currentX, currentY));
            }

            return positions;
        }
    }
}