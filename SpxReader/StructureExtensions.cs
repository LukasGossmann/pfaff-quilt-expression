
using System.Runtime.InteropServices;

namespace SpxReader
{
    public static class StructureExtension
    {
        public static byte[] ToByteArray<T>(this T structure)
            where T : struct
        {
            var size = Marshal.SizeOf<T>();
            var arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(structure, ptr, true);

                Marshal.Copy(ptr, arr, 0, size);

                return arr;
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        public static T ToStructure<T>(this byte[] bytes)
            where T : struct
        {
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                return (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            }
            finally
            {
                handle.Free();
            }
        }
    }
}