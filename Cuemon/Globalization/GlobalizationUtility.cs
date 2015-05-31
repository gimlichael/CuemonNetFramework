using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Cuemon.Caching;
using Cuemon.Collections.Generic;
using Cuemon.Reflection;

namespace Cuemon.Globalization
{
	/// <summary>
	/// This utility class is designed to make Globalization related operations easier to work with.
	/// </summary>
	public static class GlobalizationUtility
	{
        private static readonly object Locker = new object();
        private static readonly IDictionary<string, int> CurrencyCodes = GetCurrencyCodesCore();
        private static readonly IDictionary<int, Currency> Currencies = GetCurrenciesCore();

		/// <summary>
		/// Gets a <see cref="Currency"/> instance.
		/// </summary>
		/// <param name="threeLetterIsoCode">A three-letter ISO 4217 code.</param>
		/// <returns>A <see cref="Currency"/> object.</returns>
		public static Currency GetCurrency(string threeLetterIsoCode)
		{
			if (!HasCurrency(threeLetterIsoCode)) { throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "No currency could be resolved by the provided three-letter ISO currency code: {0}.", threeLetterIsoCode), "threeLetterIsoCode"); }
			return Currencies[CurrencyCodes[threeLetterIsoCode]];
		}

		/// <summary>
		/// Gets a <see cref="Currency"/> instance.
		/// </summary>
		/// <param name="isoCode">A numeric ISO 4217 code.</param>
		/// <returns>A <see cref="Currency"/> object.</returns>
		public static Currency GetCurrency(int isoCode)
		{
			if (!HasCurrency(isoCode)) { throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "No currency could be resolved by the provided ISO currency code: {0}.", isoCode), "isoCode"); }
			return Currencies[isoCode];
		}

		/// <summary>
        /// Determines whether the specified three-letter ISO 4217 code is represented by the current system.
		/// </summary>
		/// <param name="threeLetterIsoCode">The three-letter ISO 4217 code.</param>
		/// <returns>
        ///   <c>true</c> if the specified three-letter ISO 4217 code is represented by the current system; otherwise, <c>false</c>.
		/// </returns>
		public static bool HasCurrency(string threeLetterIsoCode)
		{
			if (threeLetterIsoCode == null) { throw new ArgumentNullException("threeLetterIsoCode"); }
			if (threeLetterIsoCode.Length != 3) { throw new ArgumentException("The length of the ISO code must be exactly 3, hence the name.", "threeLetterIsoCode"); }
			return CurrencyCodes.ContainsKey(threeLetterIsoCode);
		}

		/// <summary>
		/// Determines whether the specified numeric ISO 4217 code is represented by the current system.
		/// </summary>
		/// <param name="isoCode">The numeric ISO 4217 code.</param>
		/// <returns>
		///   <c>true</c> if the specified numeric ISO 4217 code is represented by the current system; otherwise, <c>false</c>.
		/// </returns>
		public static bool HasCurrency(int isoCode)
		{
			return CurrencyCodes.Values.Contains(isoCode);
		}

		/// <summary>
        /// Gets a sequence of region-specific ISO 4217 compatible currencies available on the current system.
		/// </summary>
        /// <returns>A sequence of ISO 4217 compatible <see cref="Currency"/> objects available on the current system.</returns>
		public static IEnumerable<Currency> GetCurrencies()
		{
			foreach (KeyValuePair<int, Currency> currency in Currencies)
			{
				yield return currency.Value;
			}
		}

        /// <summary>
        /// Determines whether the specified two or three letter ISO 639 code has an <see cref="CultureInfo"/> object on the current system.
        /// </summary>
        /// <param name="twoOrThreeLetterIsoCode">The two or three letter ISO 639 code.</param>
        /// <returns><c>true</c> if the specified two or three letter ISO 639 code has an <see cref="CultureInfo"/> object on the current system; otherwise, <c>false</c>.</returns>
	    public static bool HasCulture(string twoOrThreeLetterIsoCode)
	    {
            Validator.ThrowIfNullOrEmpty(twoOrThreeLetterIsoCode, "twoOrThreeLetterIsoCode");
            Validator.ThrowIfLowerThan(twoOrThreeLetterIsoCode.Length, 2, "twoOrThreeLetterIsoCode", "The length of the ISO code must be either 2 or 3.");
            Validator.ThrowIfGreaterThan(twoOrThreeLetterIsoCode.Length, 3, "twoOrThreeLetterIsoCode", "The length of the ISO code must be either 2 or 3.");

            Doer<string, bool> verifyCulture = CachingManager.Cache.Memoize<string, bool>(HasCultureCore);
            return verifyCulture(twoOrThreeLetterIsoCode);
	    }

        private static bool HasCultureCore(string twoOrThreeLetterIsoCode)
        {
            switch (twoOrThreeLetterIsoCode.Length)
            {
                case 2:
                    return EnumerableUtility.Any(EnumerableUtility.FindAll(CulturalRegion.CulturesCache, TwoLetterCultureMatch, twoOrThreeLetterIsoCode));
                case 3:
                    return EnumerableUtility.Any(EnumerableUtility.FindAll(CulturalRegion.CulturesCache, ThreeLetterCultureMatch, twoOrThreeLetterIsoCode));
                default:
                    return false;
            }
        }

        private static bool TwoLetterCultureMatch(CultureInfo culture, string twoLetterIsoCode)
        {
            return culture.TwoLetterISOLanguageName.Equals(twoLetterIsoCode, StringComparison.OrdinalIgnoreCase);
        }

        private static bool ThreeLetterCultureMatch(CultureInfo culture, string threeLetterIsoCode)
        {
            return culture.ThreeLetterISOLanguageName.Equals(threeLetterIsoCode, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether the specified two or three letter ISO 3166 code has an <see cref="RegionInfo"/> object on the current system.
        /// </summary>
        /// <param name="twoOrThreeLetterIsoCode">The two or three letter ISO 3166 code.</param>
        /// <returns><c>true</c> if the specified two or three letter ISO 3166 code has an <see cref="RegionInfo"/> object on the current system; otherwise, <c>false</c>.</returns>
	    public static bool HasRegion(string twoOrThreeLetterIsoCode)
	    {
            Validator.ThrowIfNullOrEmpty(twoOrThreeLetterIsoCode, "twoOrThreeLetterIsoCode");
            Validator.ThrowIfLowerThan(twoOrThreeLetterIsoCode.Length, 2, "twoOrThreeLetterIsoCode", "The length of the ISO code must be either 2 or 3.");
            Validator.ThrowIfGreaterThan(twoOrThreeLetterIsoCode.Length, 3, "twoOrThreeLetterIsoCode", "The length of the ISO code must be either 2 or 3.");

            Doer<string, bool> verifyRegion = CachingManager.Cache.Memoize<string, bool>(HasRegionCore);
	        return verifyRegion(twoOrThreeLetterIsoCode);
	    }

        private static bool HasRegionCore(string twoOrThreeLetterIsoCode)
	    {
            switch (twoOrThreeLetterIsoCode.Length)
            {
                case 2:
                    return EnumerableUtility.Any(EnumerableUtility.FindAll(CulturalRegion.RegionsCache, TwoLetterRegionMatch, twoOrThreeLetterIsoCode));
                case 3:
                    return EnumerableUtility.Any(EnumerableUtility.FindAll(CulturalRegion.RegionsCache, ThreeLetterRegionMatch, twoOrThreeLetterIsoCode));
                default:
                    return false;
            }
	    }

	    private static bool TwoLetterRegionMatch(RegionInfo region, string twoLetterIsoCode)
	    {
	        return region.TwoLetterISORegionName.Equals(twoLetterIsoCode, StringComparison.OrdinalIgnoreCase);
	    }

        private static bool ThreeLetterRegionMatch(RegionInfo region, string threeLetterIsoCode)
        {
            return region.ThreeLetterISORegionName.Equals(threeLetterIsoCode, StringComparison.OrdinalIgnoreCase);
        }

	    /// <summary>
        /// Gets a sequence of ISO 3166 regions available on the current system.
        /// </summary>
        /// <returns>A sequence of ISO 3166 compatible <see cref="RegionInfo"/> objects available on the current system.</returns>
	    public static IEnumerable<RegionInfo> Regions
	    {
	        get { return CulturalRegion.RegionsCache; }
	    }

        /// <summary>
        /// Gets a sequence of ISO 639-2 regions available on the current system.
        /// </summary>
        /// <returns>A sequence of ISO 639-2 compatible <see cref="CultureInfo"/> objects available on the current system.</returns>
	    public static IEnumerable<CultureInfo> Cultures
	    {
            get { return CulturalRegion.CulturesCache; }
	    }

		private static IDictionary<int, Currency> GetCurrenciesCore()
		{
		    lock (Locker)
		    {
		        Dictionary<int, Currency> currencies = new Dictionary<int, Currency>();
		        foreach (RegionInfo region in CulturalRegion.RegionsCache)
		        {
                    if (CurrencyCodes.ContainsKey(region.ISOCurrencySymbol))
                    {
                        if (currencies.ContainsKey(CurrencyCodes[region.ISOCurrencySymbol])) { continue; }
                        currencies.Add(CurrencyCodes[region.ISOCurrencySymbol], new Currency(CurrencyCodes[region.ISOCurrencySymbol], region.CurrencyEnglishName, region.CurrencyNativeName, region.CurrencySymbol, region.ISOCurrencySymbol));
                    }
		        }
		        return currencies;
		    }
		}

		private static IDictionary<string, int> GetCurrencyCodesCore()
		{
		    lock (Locker)
		    {
		        Dictionary<string, int> currencyCodes = new Dictionary<string, int>();
		        Stream tempStream = null;
		        try
		        {
		            tempStream = ReflectionUtility.GetEmbeddedResource(typeof(GlobalizationUtility), "Currency.csv", ResourceMatch.ContainsName);
		            using (StreamReader reader = new StreamReader(tempStream, true))
		            {
		                tempStream = null;
		                string textline;
		                while ((textline = reader.ReadLine()) != null)
		                {
		                    string[] currencyCodesLine = textline.Split(';');
		                    if (currencyCodes.ContainsKey(currencyCodesLine[0])) { continue; }
		                    currencyCodes.Add(currencyCodesLine[0], int.Parse(currencyCodesLine[1], CultureInfo.InvariantCulture));
		                }
		            }
		        }
		        finally
		        {
		            if (tempStream != null) { tempStream.Dispose(); }
		        }
		        return currencyCodes;
		    }
		}
	}
}