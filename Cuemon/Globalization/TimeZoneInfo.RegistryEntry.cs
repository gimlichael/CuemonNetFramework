using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using Cuemon.Caching;
using Cuemon.Diagnostics;
using Cuemon.Security.Cryptography;
using Microsoft.Win32;
namespace Cuemon.Globalization
{
    partial class TimeZoneInfo
    {
        private enum DstScenario { Daylight, Standard }

        internal partial class RegistryEntry
        {
            private static readonly object GlobalLock = new object();
            private static bool IsInitialized;
            private readonly static Dictionary<TimeZoneInfoKey, TimeZoneCachedCore> TimeZonesRegistryCore = new Dictionary<TimeZoneInfoKey, TimeZoneCachedCore>();
            private readonly TZI _tziInitialized;
            private readonly string _display;
            private readonly string _dlt;
            private const string CacheGroupName = "Cuemon.Globalization.TimeZoneInfo";

            #region Constructors
            [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
            internal RegistryEntry(string standardName)
            {
                TimeZoneInfoKey resolvedKey = TimeZoneInfo.GetKey(standardName);
                if (resolvedKey == TimeZoneInfoKey.Undefined) { return; }

                TimeZoneCachedCore core = GetTimeZones()[resolvedKey];

                _display = core.Display;
                _dlt = core.Dlt;
                _tziInitialized = this.InitializeTZI(core.RawTZI);
            }
            #endregion

            #region Methods
            internal static IDictionary<TimeZoneInfoKey, TimeZoneCachedCore> GetTimeZones()
            {
                if (!IsInitialized)
                {
                    lock (GlobalLock)
                    {
                        if (!IsInitialized) // have to add this again in case of queued thread callers
                        {
                            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Time Zones"))
                            {
                                string[] timeZoneNames = key.GetSubKeyNames();
                                foreach (string timeZoneName in timeZoneNames)
                                {
                                    using (RegistryKey subKey = key.OpenSubKey(timeZoneName))
                                    {
                                        string standardName = (string)subKey.GetValue("Std");
                                        TimeZoneInfoKey resolvedKey = TimeZoneInfo.GetKey(standardName);
                                        if (resolvedKey != TimeZoneInfoKey.Undefined)
                                        {
                                            TimeZonesRegistryCore.Add(resolvedKey, new TimeZoneCachedCore((string)subKey.GetValue("Display"), (string)subKey.GetValue("Dlt"), (byte[])subKey.GetValue("TZI")));
                                        }
                                    }
                                }
                            }
                        }
                        IsInitialized = true;
                    }
                }
                return TimeZonesRegistryCore;
            }
            #endregion

            #region Properties
            public DateTime GetStartDaylightSavingTime(int year)
            {
                return GetDateFromTZI(DstScenario.Daylight, year);
            }

            public DateTime GetEndDaylightSavingTime(int year)
            {
                return GetDateFromTZI(DstScenario.Standard, year);
            }

            public bool CalculateDaylightSavingTime(DateTime time)
            {
                if (time >= GetStartDaylightSavingTime(time.Year) && time <= GetEndDaylightSavingTime(time.Year))
                {
                    return true;
                }
                return false;
            }

            public string Display
            {
                get { return _display; }
            }

            public string Dlt
            {
                get { return _dlt; }
            }

            public TZI TziInitialized
            {
                get { return _tziInitialized; }
            }
            #endregion

            #region Methods
            [EnvironmentPermissionAttribute(SecurityAction.LinkDemand, Unrestricted = true)]
            private TZI InitializeTZI(byte[] tziValue)
            {
                string hashKey = HashUtility.ComputeHash(tziValue);
                if (CachingManager.Cache.ContainsKey(hashKey, CacheGroupName)) { return CachingManager.Cache.Get<TZI>(hashKey, CacheGroupName); }
                
                if (tziValue.Length != Marshal.SizeOf(this.TziInitialized)) { throw new ArgumentException("Information size is incorrect", "tziValue"); }
                TZI tzi;
                GCHandle h = GCHandle.Alloc(tziValue, GCHandleType.Pinned);

                try
                {
                    tzi = (TZI)Marshal.PtrToStructure(h.AddrOfPinnedObject(), typeof(TZI));
                    if (!CachingManager.Cache.ContainsKey(hashKey, CacheGroupName))
                    {
                        lock (GlobalLock)
                        {
                            if (!CachingManager.Cache.ContainsKey(hashKey, CacheGroupName))
                            {
                                CachingManager.Cache.Add(hashKey, tzi, CacheGroupName);
                            }
                        }
                    }
                }
                finally
                {
                    h.Free();
                }

                return tzi;
            }

            private DateTime GetDateFromTZI(DstScenario scenario, int year)
            {
                int day = 0;
                int month = 0;
                int hour = 0;
                int minute = 0;
                int second = 0;

                try
                {
                    switch (scenario)
                    {
                        case DstScenario.Daylight:
                            day = this.TziInitialized.daylightDate.wDay;
                            month = this.TziInitialized.daylightDate.wMonth;
                            hour = this.TziInitialized.daylightDate.wHour;
                            minute = this.TziInitialized.daylightDate.wMinute;
                            second = this.TziInitialized.daylightDate.wSecond;
                            break;
                        case DstScenario.Standard:
                            day = this.TziInitialized.standardDate.wDay;
                            month = this.TziInitialized.standardDate.wMonth;
                            hour = this.TziInitialized.standardDate.wHour;
                            minute = this.TziInitialized.standardDate.wMinute;
                            second = this.TziInitialized.standardDate.wSecond;
                            break;
                    }
                }
                catch (NullReferenceException)
                {
                    return DateTime.MinValue;
                }

                if (day == 0 &&
                    month == 0 &&
                    hour == 0 &&
                    minute == 0 &&
                    second == 0)
                {
                    return DateTime.MinValue;
                }

                DateTime date = new DateTime(year, month, 1, hour, minute, second);
                DateTime dateEnd= new DateTime();

                dateEnd = date.AddMonths(1).AddDays(-1);

                if (date.DayOfWeek != DayOfWeek.Sunday) { date = date.AddDays(7 - Convert.ToInt32(date.DayOfWeek, CultureInfo.InvariantCulture)); }

                if (dateEnd.DayOfWeek != DayOfWeek.Sunday) { dateEnd = dateEnd.AddDays(Convert.ToInt32(dateEnd.DayOfWeek, CultureInfo.InvariantCulture) * -1); }

                switch (day)
                {
                    case 1:
                        return date;
                    case 2:
                        return date.AddDays(7);
                    case 3:
                        return date.AddDays(14);
                    case 4:
                        return date.AddDays(21);
                    default:
                        return dateEnd;
                }
            }
            #endregion
        }
    }
}