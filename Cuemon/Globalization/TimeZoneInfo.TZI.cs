using System.Runtime.InteropServices;

namespace Cuemon.Globalization
{
    partial class TimeZoneInfo
    {
        /// <summary>
        /// The layout of the Tzi value in the registry.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct TZI
        {
            public int bias;
            public int standardBias;
            public int daylightBias;
            public SYSTEMTIME standardDate;
            public SYSTEMTIME daylightDate;
        }
    }
}