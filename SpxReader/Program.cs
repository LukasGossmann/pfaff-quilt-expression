using System.Text;

namespace SpxReader
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var spxFile = new SpxFile();
            using (FileStream stream = new FileStream("Stitches/U10/U10.spx", FileMode.Open, FileAccess.Read))
            {

                var buffer = new byte[SpxFile.MAGIC_STRING.Length];
                var result = stream.Read(buffer, 0, SpxFile.MAGIC_STRING.Length);
                if (result != SpxFile.MAGIC_STRING.Length)
                    throw new Exception("Reading of magic string failed.");

                var magicString = Encoding.ASCII.GetString(buffer, 0, SpxFile.MAGIC_STRING.Length);
                if (magicString != SpxFile.MAGIC_STRING)
                    throw new Exception("Unexpected magic string-");

                spxFile.ProducedWithLength = stream.ReadShortAtOffset(0x0C);
                buffer = new byte[spxFile.ProducedWithLength];
                result = stream.Read(buffer, 0, spxFile.ProducedWithLength);
                if (result != spxFile.ProducedWithLength)
                    throw new Exception("Reading of produced with string failed.");

                spxFile.ProducedWith = Encoding.BigEndianUnicode.GetString(buffer, 0, spxFile.ProducedWithLength);

                var offsetAfterProducedWith = stream.Position;

                spxFile.Height = stream.ReadShortAtOffset(offsetAfterProducedWith + 0xC2);
                spxFile.Width = stream.ReadShortAtOffset(offsetAfterProducedWith + 0xC6);
                spxFile.ResolutionX = stream.ReadShortAtOffset(offsetAfterProducedWith + 0xD9);
                spxFile.ResolutionY = stream.ReadShortAtOffset(offsetAfterProducedWith + 0xDD);
                spxFile.NumberOfBytes = stream.ReadShortAtOffset(offsetAfterProducedWith + 0xEC);
                spxFile.StitchPositions = new LinkedList<(int x, int y)>();
                spxFile.StitchPositions.AddLast((0, 0));

                stream.Seek(offsetAfterProducedWith + 0xEE, SeekOrigin.Begin);
                buffer = new byte[2];

                while (true)
                {
                    result = stream.Read(buffer, 0, buffer.Length);
                    if (result <= 0)
                        break;

                    if (result != 2)
                        throw new Exception("Failed to read x and y position of stitch");

                    var signedX = (sbyte)buffer[0];
                    var signedY = (sbyte)buffer[1];
                    var xOffset = (signedX * spxFile.ResolutionX);
                    var yOffset = (signedY * spxFile.ResolutionY);

                    var lastPosition = spxFile.StitchPositions.Last.Value;

                    var currentX = lastPosition.x + xOffset;
                    var currentY = lastPosition.y + yOffset;

                    spxFile.StitchPositions.AddLast((currentX, currentY));
                }
            }

            Console.WriteLine($"ProducedWithLength: {spxFile.ProducedWithLength}");
            Console.WriteLine($"ProducedWith: {spxFile.ProducedWith}");
            Console.WriteLine($"Height: {spxFile.Height}");
            Console.WriteLine($"Width: {spxFile.Width}");
            Console.WriteLine($"ResolutionX: {spxFile.ResolutionX}");
            Console.WriteLine($"ResolutionY: {spxFile.ResolutionY}");
            Console.WriteLine($"NumberOfBytes: {spxFile.NumberOfBytes}");
            Console.WriteLine("----------");

            foreach (var stitch in spxFile.StitchPositions)
            {
                Console.WriteLine($"X: {stitch.x}");
                Console.WriteLine($"Y: {stitch.y}");
                Console.WriteLine("----------");
            }
        }
    }
}