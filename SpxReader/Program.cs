using System.Text;

namespace SpxReader
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var spxFile = SpxFile.Open(args[0]);

            Console.WriteLine($"RemainingFileLength1: {spxFile.RemainingFileLength1}");
            Console.WriteLine($"ProducedWithLength: {spxFile.ProducedWithLength}");
            Console.WriteLine($"ProducedWithString: {spxFile.ProducedWithString}");
            Console.WriteLine($"RemainingFileLength2: {spxFile.RemainingFileLength2}");
            Console.WriteLine($"SizeOfVariableLengthData: {spxFile.SizeOfVariableLengthData}");
            Console.WriteLine($"SpxStruct.StitchCountLimit: {spxFile.SpxStruct.StitchCountLimit.Int}");
            Console.WriteLine($"SpxStruct.StitchOffset: {spxFile.SpxStruct.StitchOffset.Int}");
            Console.WriteLine($"SpxStruct.StartTaperSide: {spxFile.SpxStruct.StartTaperSide}");
            Console.WriteLine($"SpxStruct.EndTaperSide: {spxFile.SpxStruct.EndTaperSide}");
            Console.WriteLine($"SpxStruct.StartTaperAngle: {spxFile.SpxStruct.StartTaperAngle}");
            Console.WriteLine($"SpxStruct.EndTaperAngle: {spxFile.SpxStruct.EndTaperAngle}");
            Console.WriteLine($"SpxStruct.StringTension: {spxFile.SpxStruct.StringTension}");
            Console.WriteLine($"SpxStruct.StitchHeight: {spxFile.SpxStruct.StitchHeight.Int}");
            Console.WriteLine($"SpxStruct.StitchWidth: {spxFile.SpxStruct.StitchWidth.Int}");
            Console.WriteLine($"SpxStruct.ResolutionX: {spxFile.SpxStruct.ResolutionX.Int}");
            Console.WriteLine($"SpxStruct.ResolutionY: {spxFile.SpxStruct.ResolutionY.Int}");
            Console.WriteLine($"SpxStruct.StartPositionX: {spxFile.SpxStruct.StartPositionX.Int}");
            Console.WriteLine($"SpxStruct.StitchArrayLength: {spxFile.SpxStruct.StitchArrayLength.Int}");

            Console.WriteLine();

            var stitchPositions = spxFile.DecodeStitchPositions();

            Console.WriteLine($"     X     |     Y");
            Console.WriteLine($"-----------|-----------");
            foreach (var position in stitchPositions)
            {
                Console.WriteLine($"{position.X.ToString().PadLeft(10)} | {position.Y.ToString().PadLeft(10)}");
            }
        }
    }
}