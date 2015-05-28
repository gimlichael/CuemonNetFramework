using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
namespace Cuemon.Globalization
{
	/// <summary>
	/// Represents a currency information.
	/// </summary>
	public sealed class Currency
	{
		private readonly int _isoCode;
		private readonly string _englishName;
		private readonly string _nativeName;
		private readonly string _symbol;
		private readonly string _threeLetterIsoCode;

		#region Constructors
		internal Currency(int isoCode, string englishName, string nativeName, string symbol, string threeLetterIsoCode)
		{
			_isoCode = isoCode;
			_englishName = englishName;
			_nativeName = nativeName;
			_symbol = symbol;
			_threeLetterIsoCode = threeLetterIsoCode;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the numeric ISO 4217 code of this <see cref="Currency"/>.
		/// </summary>
		public int IsoCode { get { return _isoCode; } }

		/// <summary>
		/// Gets the currency name, in English, of this <see cref="Currency"/>.
		/// </summary>
		public string EnglishName { get { return _englishName; } }

		/// <summary>
		/// Gets the localized name of this <see cref="Currency"/>. 
		/// </summary>
		public string NativeName { get { return _nativeName; } }

		/// <summary>
		/// Gets the localized symbol of this <see cref="Currency"/>.
		/// </summary>
		/// <value>The localized symbol of this <see cref="Currency"/>.</value>
		public string Symbol { get { return _symbol; } }

		/// <summary>
		/// Gets the three-letter ISO 4217 code of this <see cref="Currency"/>.
		/// </summary>
		public string ThreeLetterIsoCode { get { return _threeLetterIsoCode; } }
		#endregion

		#region Methods
		#endregion
	}
}