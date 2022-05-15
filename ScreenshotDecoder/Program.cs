using System.Diagnostics;

namespace ScreenshotDecoder
{
    internal class Program
    {
        private const int IMAGE_WIDTH = 320;
        private const int IMAGE_HEIGHT = 240;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var tmpOutputFilename = Path.Combine(Path.GetDirectoryName(args[0]), Path.GetFileNameWithoutExtension(args[0]) + ".rgb");
            var outputFilename = Path.Combine(Path.GetDirectoryName(args[0]), Path.GetFileNameWithoutExtension(args[0]) + ".png");

            using (FileStream rawScreenshot = new FileStream(args[0], FileMode.Open, FileAccess.Read))
            using (FileStream rgbScreenhot = new FileStream(tmpOutputFilename, FileMode.Create, FileAccess.Write))
            {
                var readBuffer = new byte[4];
                var writeBuffer = new byte[3];
                for (long pixelCounter = 0; pixelCounter < (IMAGE_WIDTH * IMAGE_HEIGHT); pixelCounter++)
                {
                    if (rawScreenshot.Read(readBuffer, 0, readBuffer.Length) != readBuffer.Length)
                        throw new Exception($"Reading raw image data failed at pixel {pixelCounter}");

                    /*
                    readBuffer[0] = g1 g0 r5 r4 r3 r2 r1 r0;
                    readBuffer[1] = b3 b2 b1 b0 g5 g4 g3 g2;
                    readBuffer[2] = 0  0  0  0  0  0  b5 b4;
                    readBuffer[3] = 0  0  0  0  0  0  0  0;
                    */

                    writeBuffer[0] = ConvertSixToEightBitColorValue(readBuffer[0] & 0b00111111);
                    writeBuffer[1] = ConvertSixToEightBitColorValue(((readBuffer[0] & 0b11000000) >> 6) | ((readBuffer[1] & 0b00001111) << 2));
                    writeBuffer[2] = ConvertSixToEightBitColorValue(((readBuffer[1] & 0b11110000) >> 4) | ((readBuffer[2] & 0b00000011) << 4));

                    rgbScreenhot.Write(writeBuffer, 0, writeBuffer.Length);
                }
            }

            var imageMagick = new Process();
            imageMagick.StartInfo.FileName = "convert";
            imageMagick.StartInfo.Arguments = $"-depth 8 -size {IMAGE_WIDTH}x{IMAGE_HEIGHT} {tmpOutputFilename} {outputFilename}";
            imageMagick.Start();
            imageMagick.WaitForExit();

            File.Delete(tmpOutputFilename);
        }

        private static byte ConvertSixToEightBitColorValue(int sixBitColorValue)
        {
            //(sixBitColorValue - 0) * (0xFF - 0) / (0x3F - 0) + 0;
            return (byte)((sixBitColorValue * 0xFF) / 0x3F);
            //return (byte)(sixBitColorValue << 2);
        }
    }
}