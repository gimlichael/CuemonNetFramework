using System;

namespace Cuemon.Globalization
{
	partial class TimeZoneInfo
	{
		/// <summary>
		/// The standard Windows SYSTEMTIME structure.
		/// </summary>
		internal struct SYSTEMTIME
		{
			#pragma warning disable 0649,0169
			public UInt16 wYear;
			public UInt16 wMonth;
			public UInt16 wDayOfWeek;
			public UInt16 wDay;
			public UInt16 wHour;
			public UInt16 wMinute;
			public UInt16 wSecond;
			public UInt16 wMilliseconds;
			#pragma warning restore 0649,0169
		}
	}
}