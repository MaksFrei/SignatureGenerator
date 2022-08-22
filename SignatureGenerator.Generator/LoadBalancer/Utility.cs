using System.Runtime.InteropServices;

namespace SignatureGenerator.CLI
{
    /// <summary>
    /// Provides access to info from kernel32.dll
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// Tryes to get RAM info
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static bool TryToGetRamInfo(out RAMInfo info)
        {
            MEMORYSTATUSEX memStatus = new MEMORYSTATUSEX();
            if (GlobalMemoryStatusEx(memStatus))
            {
                info = new RAMInfo(memStatus.ullTotalPhys, memStatus.ullAvailPhys);
                return true;
            }
            info = null;
            return false;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
            public MEMORYSTATUSEX()
            {
                this.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            }
        }


        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);
    }

    /// <summary>
    /// RAM info DTO
    /// </summary>
    public record RAMInfo(ulong TotalRAM, ulong AvailableRAM);
}
