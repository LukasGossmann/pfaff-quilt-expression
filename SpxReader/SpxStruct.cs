using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SpxReader
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SpxStruct
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        private byte[] Unknown1; // Always starts with 01 05 01

        public EndianSwap32Bit StitchCountLimit; // 0 = no limit

        public EndianSwap32Bit StitchOffset; // Sewing mode setting

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        private byte[] Unknown2;

        public VsmTaperSide StartTaperSide;

        public VsmTaperSide EndTaperSide;

        public VsmTaperAngle StartTaperAngle;

        public VsmTaperAngle EndTaperAngle;

        public byte StringTension; // Sewing mode setting * 10

        public EndianSwap32Bit StitchHeight; // Sewing mode setting * 1000

        public EndianSwap32Bit StitchWidth; // Sewing mode setting * 1000


        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 15)]
        private byte[] Unknown5;

        public EndianSwap32Bit ResolutionX; // * 1000
        public EndianSwap32Bit ResolutionY; // * 1000

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        private byte[] Unknown7;

        public EndianSwap32Bit StartPositionX; // * 1000

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        private byte[] Unknown8;

        public EndianSwap32Bit StitchArrayLength; // in bytes (divide by 2 for number of stitches)

        public SpxStruct ConvertEndianness()
        {
            StitchCountLimit = StitchCountLimit.Swap();
            StitchOffset = StitchOffset.Swap();
            StitchHeight = StitchHeight.Swap();
            StitchWidth = StitchWidth.Swap();
            ResolutionX = ResolutionX.Swap();
            ResolutionY = ResolutionY.Swap();
            StartPositionX = StartPositionX.Swap();
            StitchArrayLength = StitchArrayLength.Swap();

            return this;
        }
    }

    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    [DebuggerDisplay("{Int} [{LowByte.ToString(\"X2\")}, {LowMiddleByte.ToString(\"X2\")}, {HighMiddleByte.ToString(\"X2\")}, {HighByte.ToString(\"X2\")}]")]
    public struct EndianSwap32Bit
    {
        [FieldOffset(0)]
        public int Int;

        [FieldOffset(0)]
        byte LowByte;

        [FieldOffset(1)]
        byte LowMiddleByte;

        [FieldOffset(2)]
        byte HighMiddleByte;

        [FieldOffset(3)]
        byte HighByte;

        public EndianSwap32Bit Swap()
        {
            var tmp = LowByte;
            LowByte = HighByte;
            HighByte = tmp;

            tmp = LowMiddleByte;
            LowMiddleByte = HighMiddleByte;
            HighMiddleByte = tmp;

            return this;
        }
    }

    public enum VsmTaperSide : byte
    {
        BOTH = 1,
        RIGHT = 2,
        LEFT = 3,
    }

    public enum VsmTaperAngle : byte
    {
        Thirty = 30, // VsmTaperSide.LEFT + VsmTaperSide.RIGHT only
        FourtyFive = 45, // VsmTaperSide.LEFT + VsmTaperSide.RIGHT only
        Sixty = 60,
        Ninety = 90, // VsmTaperSide.BOTH only
        HundredTwenty = 120, // VsmTaperSide.BOTH only
    }
}