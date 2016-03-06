using System;
using System.Collections.Generic;
using System.Globalization;
using Cuemon.Runtime.Caching;

namespace Cuemon.Globalization
{
    /// <summary>
    /// Represents ISO 639 compatible cultures associated with an ISO 3166 compatible region. This class cannot be inherited.
    /// </summary>
    public sealed class CulturalRegion
    {
        private static readonly object Locker = new object();
        internal static readonly IList<CultureInfo> CulturesCache = GetCulturesCore();
        internal static readonly IList<RegionInfo> RegionsCache = GetRegionsCore();

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CulturalRegion"/> class.
        /// </summary>
        /// <param name="region">The region that this instance represents.</param>
        public CulturalRegion(RegionInfo region)
        {
            this.Region = region;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CulturalRegion"/> class.
        /// </summary>
        /// <param name="culture">The culture used to determine the region of this instance.</param>
        /// <exception cref="System.ArgumentException">The specified <paramref name="culture"/> is either an invariant, custom, or neutral culture.</exception>
        public CulturalRegion(CultureInfo culture)
        {
            Validator.ThrowIfNull(culture, nameof(culture));
            if (culture.IsNeutralCulture ||
                culture.LCID == 127) { throw new ArgumentException("The specified culture is either an invariant, custom, or neutral culture.", nameof(culture)); }
            this.Region = new RegionInfo(culture.LCID);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the region that this instance represents.
        /// </summary>
        /// <value>The region of this instance.</value>
        public RegionInfo Region { get; private set; }

        /// <summary>
        /// Gets the cultures associated with the <see cref="Region"/> of this instance.
        /// </summary>
        /// <value>The cultures associated with the <see cref="Region"/> of this instance.</value>
        public IEnumerable<CultureInfo> Cultures
        {
            get
            {
                Doer<RegionInfo, IEnumerable<CultureInfo>> filteredCultures = CachingManager.Cache.Memoize<RegionInfo, IEnumerable<CultureInfo>>(FilteredCultures);
                return filteredCultures(this.Region);
            }
        }

        private IEnumerable<CultureInfo> FilteredCultures(RegionInfo region)
        {
            foreach (CultureInfo culture in CulturesCache)
            {
                if (culture.Name.EndsWith(region.Name.Substring(region.Name.LastIndexOf('-') + 1), StringComparison.OrdinalIgnoreCase))
                {
                    yield return culture;
                }
            }
        }
        #endregion

        #region Methods
        private static IList<CultureInfo> GetCulturesCore()
        {
            lock (Locker)
            {
                SortedList<string, CultureInfo> cultures = new SortedList<string, CultureInfo>();
                foreach (CultureInfo info in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
                {
                    cultures.Add(info.EnglishName, info);
                }
                return cultures.Values;
            }
        }

        private static IList<RegionInfo> GetRegionsCore()
        {
            lock (Locker)
            {
                SortedList<string, RegionInfo> regions = new SortedList<string, RegionInfo>();
                foreach (CultureInfo culture in CulturesCache)
                {
                    RegionInfo region = new RegionInfo(culture.LCID);
                    if (!regions.ContainsKey(region.EnglishName)) { regions.Add(region.EnglishName, region); }
                }
                return regions.Values;
            }
        }
        #endregion
    }
}
