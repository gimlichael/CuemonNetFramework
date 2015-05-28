using System;
using Cuemon.Diagnostics;
namespace Cuemon.Globalization
{
    partial class TimeZoneInfo
    {
        internal partial class RegistryEntry
        {
            internal class TimeZoneCachedCore
            {
                public string Display;
                public string Dlt;
                public byte[] RawTZI;
                //public TZI InitializedTZI;

                //public TimeZoneCachedCore(string display, string dlt, TZI initializedTZI)
                public TimeZoneCachedCore(string display, string dlt, byte[] rawTZI)
                {
                    Display = display;
                    Dlt = dlt;
                    RawTZI = rawTZI;
                }
            }
        }
    }
}