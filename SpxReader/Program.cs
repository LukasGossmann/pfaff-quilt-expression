using System.Text;

namespace SpxReader
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var filename = "";
            var spxFile = SpxFile.Open(filename);
            var stitchPositions = spxFile.DecodeStitchPositions();

            foreach (var position in stitchPositions)
            {
                Console.WriteLine($"{position.X} {position.Y}");
            }
        }
    }
}