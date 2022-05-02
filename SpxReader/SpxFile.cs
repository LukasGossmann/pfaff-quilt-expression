namespace SpxReader
{
    public class SpxFile
    {
        // Offset 0x00 - 0x04
        public const string MAGIC_STRING = "%spx%";

        // Offset 0x0C + 0x0D
        public short ProducedWithLength { get; set; }

        // Starts at offset 0x0E
        public string ProducedWith { get; set; }

        // End of ProducedBy in used example files: 0x36

        // End of ProducedBy + 0x31 or 0xC2
        public short Height { get; set; }

        // End of ProducedBy + 0x6A or 0xC6
        public short Width { get; set; }

        // End of ProducedBy + 0xD9
        public short ResolutionX { get; set; }

        // End of ProducedBy + 0xDD
        public short ResolutionY { get; set; }

        // End of ProducedBy + 0xED
        public short NumberOfBytes { get; set; }

        // End of ProducedBy + 0xEE
        public LinkedList<(int x, int y)> StitchPositions { get; set; }
    }
}