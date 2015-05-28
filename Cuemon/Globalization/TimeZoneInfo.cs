using System;
using System.Globalization;
using Cuemon.Diagnostics;
namespace Cuemon.Globalization
{
    /// <summary>
    /// Represents time zone information.
    /// </summary>
    public partial class TimeZoneInfo : TimeZone
    {
        private RegistryEntry _registryEntry;
        private string _standardName;
        private int _daylightChangesYearInUse;
        private readonly object _instanceLock = new object();
        private readonly int _currentUtcYear = DateTime.UtcNow.Year;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeZoneInfo"/> class, using the time zone of the current computer system and the current UTC year.
        /// </summary>
        public TimeZoneInfo()
        {
            _standardName = TimeZone.CurrentTimeZone.StandardName;
            _daylightChangesYearInUse = _currentUtcYear;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeZoneInfo"/> class, using the specified <paramref name="timeZone"/> and the current UTC year.
        /// </summary>
        /// <param name="timeZone">The <see cref="TimeZoneInfoKey"/> to resolve the standard name of the timezone.</param>
        public TimeZoneInfo(TimeZoneInfoKey timeZone)
        {
            _standardName = GetStandardName(timeZone);
            _daylightChangesYearInUse = _currentUtcYear;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeZoneInfo"/> class, using the specified <paramref name="standardName"/> and the current UTC year.
        /// </summary>
        /// <param name="standardName">The standard name of the timezone.</param>
        public TimeZoneInfo(string standardName)
        {
            _standardName = standardName;
            _daylightChangesYearInUse = _currentUtcYear;
        }
        #endregion

        #region Properties
        private RegistryEntry GetRegistry()
        {
            if (_registryEntry == null)
            {
                lock (_instanceLock)
                {
                    if (_registryEntry == null)
                    {
                        _registryEntry = new RegistryEntry(this.StandardName);
                    }
                }
            }
            return _registryEntry;
        }

        /// <summary>
        /// Gets or sets the standard time zone name.
        /// </summary>
        /// <value></value>
        /// <returns>The standard time zone name.</returns>
        /// <exception cref="T:System.ArgumentNullException">Attempted to set this property to null. </exception>
        public override string StandardName
        {
            get { return _standardName; }
        }

        /// <summary>
        /// Gets the key for the time zone.
        /// </summary>
        /// <value>The key for the time zone.</value>
        public TimeZoneInfoKey GetKey()
        {
            return GetKey(this.StandardName);
        }

        /// <summary>
        /// Gets the year currently used for resolving daylight changes for this time zone.
        /// </summary>
        /// <value>The year currently used for resolving daylight changes for this time zone.</value>
        protected int DaylightChangesYearInUse
        {
            get { return _daylightChangesYearInUse; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Infrastructure. Specifies a new standard time zone name, bypassing constructor value.
        /// </summary>
        /// <param name="timeZone">The <see cref="TimeZoneInfoKey"/> to specify a standard time zone name from.</param>
        internal void SetStandardName(TimeZoneInfoKey timeZone)
        {
            this.SetStandardName(GetStandardName(timeZone));
        }

        /// <summary>
        /// Infrastructure. Specifies a new standard time zone name, bypassing constructor value.
        /// </summary>
        /// <param name="standardName">The standard time zone name.</param>
        internal void SetStandardName(string standardName)
        {
            _standardName = standardName;
        }

        /// <summary>
        /// Determines whether the specified standard name is valid.
        /// </summary>
        /// <param name="standardName">The name of the time zone to validate.</param>
        /// <returns>
        /// 	<c>true</c> if the specified standard name is valid; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValid(string standardName)
        {
            try
            {
                Enum.Parse(typeof(TimeZoneInfoKey), standardName, true);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the time zone key from the standard name.
        /// </summary>
        /// <param name="standardName">The time zone standard name.</param>
        public static TimeZoneInfoKey GetKey(string standardName)
        {
            if (standardName == null) { throw new ArgumentNullException("standardName"); }
            string[] keys = Enum.GetNames(typeof(TimeZoneInfoKey));
            foreach (string key in keys)
            {
                TimeZoneInfoKey keyEnum = (TimeZoneInfoKey)Enum.Parse(typeof(TimeZoneInfoKey), key);
                if (string.Compare(standardName, GetStandardName(keyEnum), StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return keyEnum;
                }
            }
            return TimeZoneInfoKey.Undefined;
        }

        /// <summary>
        /// Gets the time zone standard name.
        /// </summary>
        /// <param name="timeZone">The time zone key to lookup the standard name from.</param>
        public static string GetStandardName(TimeZoneInfoKey timeZone)
        {
            switch (timeZone)
            {
                case TimeZoneInfoKey.AfghanistanStandardTime:
                    return "Afghanistan Standard Time";
                case TimeZoneInfoKey.AlaskanStandardTime:
                    return "Alaskan Standard Time";
                case TimeZoneInfoKey.ArabianStandardTime:
                    return "Arabian Standard Time"; 
                case TimeZoneInfoKey.ArabStandardTime:
                    return "Arab Standard Time";
                case TimeZoneInfoKey.AtlanticStandardTime:
                    return "Atlantic Standard Time";
                case TimeZoneInfoKey.AusCentralStandardTime:
                    return "AUS Central Standard Time";
                case TimeZoneInfoKey.AusEasternStandardTime:
                    return "AUS Eastern Standard Time";
                case TimeZoneInfoKey.AzerbaijanStandardTime:
                    return "Azerbaijan Standard Time";
                case TimeZoneInfoKey.AzoresStandardTime:
                    return "Azores Standard Time";
                case TimeZoneInfoKey.CanadaCentralStandardTime:
                    return "Canada Central Standard Time";
                case TimeZoneInfoKey.CapeVerdeStandardTime:
                    return "Cape Verde Standard Time";
                case TimeZoneInfoKey.CaucasusStandardTime:
                    return "Caucasus Standard Time";
                case TimeZoneInfoKey.CenAustraliaStandardTime:
                    return "Cen. Australia Standard Time";
                case TimeZoneInfoKey.CentralAmericaStandardTime:
                    return "Central America Standard Time";
                case TimeZoneInfoKey.CentralAsiaStandardTime:
                    return "Central Asia Standard Time";
                case TimeZoneInfoKey.CentralBrazilianStandardTime:
                    return "Central Brazilian Standard Time";
                case TimeZoneInfoKey.CentralEuropeanStandardTime:
                    return "Central European Standard Time";
                case TimeZoneInfoKey.CentralEuropeStandardTime:
                    return "Central Europe Standard Time";
                case TimeZoneInfoKey.CentralPacificStandardTime:
                    return "Central Pacific Standard Time";
                case TimeZoneInfoKey.CentralStandardTime:
                    return "Central Standard Time";
                case TimeZoneInfoKey.CentralStandardTimeMexico:
                    return "Central Standard Time (Mexico)";
                case TimeZoneInfoKey.ChinaStandardTime:
                    return "China Standard Time";
                case TimeZoneInfoKey.DatelineStandardTime:
                    return "Dateline Standard Time";
                case TimeZoneInfoKey.EAfricaStandardTime:
                    return "E. Africa Standard Time";
                case TimeZoneInfoKey.EasternStandardTime:
                    return "Eastern Standard Time";
                case TimeZoneInfoKey.EAustraliaStandardTime:
                    return "E. Australia Standard Time";
                case TimeZoneInfoKey.EEuropeStandardTime:
                    return "E. Europe Standard Time";
                case TimeZoneInfoKey.EgyptStandardTime:
                    return "Egypt Standard Time";
                case TimeZoneInfoKey.EkaterinburgStandardTime:
                    return "Ekaterinburg Standard Time";
                case TimeZoneInfoKey.ESouthAmericaStandardTime:
                    return "E. South America Standard Time";
                case TimeZoneInfoKey.FijiStandardTime:
                    return "Fiji Standard Time";
                case TimeZoneInfoKey.FleStandardTime:
                    return "FLE Standard Time";
                case TimeZoneInfoKey.GeorgianStandardTime:
                    return "Georgian Standard Time";
                case TimeZoneInfoKey.GmtStandardTime:
                    return "GMT Standard Time";
                case TimeZoneInfoKey.GreenlandStandardTime:
                    return "Greenland Standard Time";
                case TimeZoneInfoKey.GreenwichStandardTime:
                    return "Greenwich Standard Time";
                case TimeZoneInfoKey.GtbStandardTime:
                    return "GTB Standard Time";
                case TimeZoneInfoKey.HawaiianStandardTime:
                    return "Hawaiian Standard Time";
                case TimeZoneInfoKey.IndiaStandardTime:
                    return "India Standard Time";
                case TimeZoneInfoKey.IranStandardTime:
                    return "Iran Standard Time";
                case TimeZoneInfoKey.IsraelStandardTime:
                    return "Israel Standard Time";
                case TimeZoneInfoKey.JordanStandardTime:
                    return "Jordan Standard Time";
                case TimeZoneInfoKey.KoreaStandardTime:
                    return "Korea Standard Time";
                case TimeZoneInfoKey.SingaporeStandardTime:
                    return "Singapore Standard Time";
                case TimeZoneInfoKey.MidAtlanticStandardTime:
                    return "Mid-Atlantic Standard Time";
                case TimeZoneInfoKey.MiddleEastStandardTime:
                    return "Middle East Standard Time";
                case TimeZoneInfoKey.MountainStandardTime:
                    return "Mountain Standard Time";
                case TimeZoneInfoKey.MountainStandardTimeMexico:
                    return "Mountain Standard Time (Mexico)";
                case TimeZoneInfoKey.MyanmarStandardTime:
                    return "Myanmar Standard Time";
                case TimeZoneInfoKey.NamibiaStandardTime:
                    return "Namibia Standard Time";
                case TimeZoneInfoKey.NCentralAsiaStandardTime:
                    return "N. Central Asia Standard Time";
                case TimeZoneInfoKey.NepalStandardTime:
                    return "Nepal Standard Time";
                case TimeZoneInfoKey.NewfoundlandStandardTime:
                    return "Newfoundland Standard Time";
                case TimeZoneInfoKey.NewZealandStandardTime:
                    return "New Zealand Standard Time";
                case TimeZoneInfoKey.NorthAsiaEastStandardTime:
                    return "North Asia East Standard Time";
                case TimeZoneInfoKey.NorthAsiaStandardTime:
                    return "North Asia Standard Time";
                case TimeZoneInfoKey.PacificSAStandardTime:
                    return "Pacific SA Standard Time";
                case TimeZoneInfoKey.PacificStandardTime:
                    return "Pacific Standard Time";
                case TimeZoneInfoKey.PacificStandardTimeMexico:
                    return "Pacific Standard Time (Mexico)";
                case TimeZoneInfoKey.RomanceStandardTime:
                    return "Romance Standard Time";
                case TimeZoneInfoKey.RussianStandardTime:
                    return "Russian Standard Time";
                case TimeZoneInfoKey.SAEasternStandardTime:
                    return "SA Eastern Standard Time";
                case TimeZoneInfoKey.SamoaStandardTime:
                    return "Samoa Standard Time";
                case TimeZoneInfoKey.SAPacificStandardTime:
                    return "SA Pacific Standard Time";
                case TimeZoneInfoKey.SAWesternStandardTime:
                    return "SA Western Standard Time";
                case TimeZoneInfoKey.SEAsiaStandardTime:
                    return "SE Asia Standard Time";
                case TimeZoneInfoKey.SouthAfricaStandardTime:
                    return "South Africa Standard Time";
                case TimeZoneInfoKey.SriLankaStandardTime:
                    return "Sri Lanka Standard Time";
                case TimeZoneInfoKey.TaipeiStandardTime:
                    return "Taipei Standard Time";
                case TimeZoneInfoKey.TasmaniaStandardTime:
                    return "Tasmania Standard Time";
                case TimeZoneInfoKey.TokyoStandardTime:
                    return "Tokyo Standard Time";
                case TimeZoneInfoKey.TongaStandardTime:
                    return "Tonga Standard Time";
                case TimeZoneInfoKey.UsEasternStandardTime:
                    return "US Eastern Standard Time";
                case TimeZoneInfoKey.UsMountainStandardTime:
                    return "US Mountain Standard Time";
                case TimeZoneInfoKey.VladivostokStandardTime:
                    return "Vladivostok Standard Time";
                case TimeZoneInfoKey.WAustraliaStandardTime:
                    return "W. Australia Standard Time";
                case TimeZoneInfoKey.WCentralAfricaStandardTime:
                    return "W. Central Africa Standard Time";
                case TimeZoneInfoKey.WestAsiaStandardTime:
                    return "West Asia Standard Time";
                case TimeZoneInfoKey.WestPacificStandardTime:
                    return "West Pacific Standard Time";
                case TimeZoneInfoKey.WEuropeStandardTime:
                    return "W. Europe Standard Time";
                case TimeZoneInfoKey.YakutskStandardTime:
                    return "Yakutsk Standard Time";
                default:
                    return null;
            }
        }

        /// <summary>
        /// Gets the display name of the time zone.
        /// </summary>
        /// <value>The display name of the time zone.</value>
        public string DisplayName
        {
            get { return this.GetRegistry().Display; }
        }

        /// <summary>
        /// Gets the daylight saving time zone name.
        /// </summary>
        /// <returns>The daylight saving time zone name.</returns>
        public override string DaylightName
        {
            get { return this.GetRegistry().Dlt; }
        }

        /// <summary>
        /// Returns a value indicating whether the current date and time is within a daylight saving time period.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if time is in a daylight saving time period; <c>false</c> otherwise, or if time is null.
        /// </returns>
        public bool IsDaylightSavingTime()
        {
            return IsDaylightSavingTime(DateTime.UtcNow);
        }

        /// <summary>
        /// Returns a value indicating whether the specified date and time is within a daylight saving time period.
        /// </summary>
        /// <param name="time">A date and time.</param>
        /// <returns>
        /// true if time is in a daylight saving time period; false otherwise, or if time is null.
        /// </returns>
        public override bool IsDaylightSavingTime(DateTime time)
        {
            return this.GetRegistry().CalculateDaylightSavingTime(time);
        }

        /// <summary>
        /// Returns the daylight saving time period for the current year.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Globalization.DaylightTime"></see> instance containing the start and end date for daylight saving time in year.
        /// </returns>
        public DaylightTime GetDaylightChanges()
        {
            return this.GetDaylightChanges(_currentUtcYear);
        }

        /// <summary>
        /// Returns the daylight saving time period for a particular year.
        /// </summary>
        /// <param name="year">The year to which the daylight saving time period applies.</param>
        /// <returns>
        /// A <see cref="T:System.Globalization.DaylightTime"></see> instance containing the start and end date for daylight saving time in year.
        /// </returns>
        /// <exception cref="T:System.ArgumentOutOfRangeException">year is less than 1 or greater than 9999. </exception>
        public override DaylightTime GetDaylightChanges(int year)
        {
            _daylightChangesYearInUse = year;
            DaylightTime dlt = new DaylightTime(this.GetRegistry().GetStartDaylightSavingTime(year),
                this.GetRegistry().GetEndDaylightSavingTime(year),
                new TimeSpan(0, -(this.GetRegistry().TziInitialized.daylightBias), 0));
            if (dlt.Start == DateTime.MinValue && dlt.End == DateTime.MinValue)
            {
                dlt = null;
            }
            return dlt;
        }

        /// <summary>
        /// Returns the coordinated universal time (UTC) offset for the current local time.
        /// </summary>
        /// <returns>The UTC offset from time, measured in ticks.</returns>
        public TimeSpan GetUtcOffset()
        {
            return this.GetUtcOffset(DateTime.UtcNow);
        }

        /// <summary>
        /// Returns the coordinated universal time (UTC) offset for the specified local time.
        /// </summary>
        /// <param name="time">The local date and time.</param>
        /// <returns>
        /// The UTC offset from time, measured in ticks.
        /// </returns>
        public override TimeSpan GetUtcOffset(DateTime time)
        {
            if (!this.IsDaylightSavingTime(time))
            {
                return new TimeSpan(0, -(this.GetRegistry().TziInitialized.bias + this.GetRegistry().TziInitialized.standardBias), 0);
            }
            return new TimeSpan(0, -(this.GetRegistry().TziInitialized.bias + this.GetRegistry().TziInitialized.daylightBias), 0);
        }
        #endregion
    }
}