﻿using System;
using System.Globalization;

namespace Cuemon.Security
{
	/// <summary>
	/// Represents a simple security token schematic.
	/// </summary>
	public sealed class SecurityToken
	{
		#region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SecurityToken"/> class.
        /// </summary>
        /// <param name="settings">The settings to apply to this instance.</param>
	    SecurityToken(SecurityTokenSettings settings)
	    {
	        Validator.ThrowIfNull(settings, "settings");
	        this.Settings = settings;
	        this.UtcCreated = DateTime.UtcNow;
            this.Token = StringUtility.CreateRandomString(settings.LengthOfToken);
	    }

		/// <summary>
		/// Initializes a new instance of the <see cref="SecurityToken"/> class.
		/// </summary>
		/// <param name="securityToken">The security token as its string equivalent.</param>
		SecurityToken(string securityToken)
		{
            Validator.ThrowIfNullOrEmpty(securityToken, "securityToken");
            Validator.ThrowIfLowerThan(securityToken.Length, 34, "securityToken");

		    string[] tokenSegments = StringUtility.Split(securityToken, ";");
            Validator.ThrowIfLowerThan(tokenSegments.Length, 4, "securityToken");
            Validator.ThrowIfGreaterThan(tokenSegments.Length, 5, "securityToken");

            TimeSpan timeToLive = new TimeSpan(long.Parse(tokenSegments[0].Trim('"'), CultureInfo.InvariantCulture));
            DateTime utcCreated = DateTime.Parse(tokenSegments[1].Trim('"'), CultureInfo.InvariantCulture).ToUniversalTime();
			string token = tokenSegments[2].Trim('"');
            string reference = tokenSegments[3].Trim('"');

            SecurityTokenSettings settings = new SecurityTokenSettings(timeToLive, token.Length, reference);
		    this.Settings = settings;
		    this.Token = token;
		    this.UtcCreated = utcCreated;
		}
		#endregion

		#region Properties
        /// <summary>
        /// Gets the settings applied to this <see cref="SecurityToken"/>.
        /// </summary>
        /// <value>The settings applied to this <see cref="SecurityToken"/>.</value>
        public SecurityTokenSettings Settings { get; private set; }

		/// <summary>
		/// Gets the UTC date time value from when this instance was created.
		/// </summary>
		public DateTime UtcCreated { get; private set; }

		/// <summary>
		/// Gets the token of this <see cref="SecurityToken"/>.
		/// </summary>
		public string Token { get; private set; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="SecurityToken"/> has expired.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this <see cref="SecurityToken"/> has expired; otherwise, <c>false</c>.
		/// </value>
		public bool HasExpired
		{
			get { return (DateTime.UtcNow >= this.UtcCreated.Add(this.Settings.TimeToLive)); }
		}
		#endregion

		#region Methods
        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return this.Settings.TimeToLive.GetHashCode() ^ this.UtcCreated.ToString("u", CultureInfo.InvariantCulture).GetHashCode() ^ this.Token.GetHashCode() ^ this.Settings.Reference.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj == null) { return false; }

            SecurityToken token = obj as SecurityToken;
            if (token == null) { return false; }
            return this.Equals(token);
        } 

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><c>true</c> if the current object is equal to the other parameter; otherwise, <c>false</c>. </returns>
        public bool Equals(SecurityToken other)
        {
            if (other == null) { return false; }
            return (this.GetHashCode() == other.GetHashCode());
        }

        /// <summary>
        /// Indicates whether two <see cref="SecurityToken"/> instances are equal.
        /// </summary>
        /// <param name="token1">The first token to compare.</param>
        /// <param name="token2">The second token to compare.</param>
        /// <returns><c>true</c> if the values of <paramref name="token1"/> and <paramref name="token2"/> are equal; otherwise, false. </returns>
        public static bool operator ==(SecurityToken token1, SecurityToken token2)
        {
            if (ReferenceEquals(token1, token2)) { return true; }
            if (((object)token1 == null) || ((object)token2 == null)) { return false; }
            return token1.GetHashCode() == token2.GetHashCode();
        }

        /// <summary>
        /// Indicates whether two <see cref="SecurityToken"/> instances are not equal.
        /// </summary>
        /// <param name="token1">The first token to compare.</param>
        /// <param name="token2">The second token to compare.</param>
        /// <returns><c>true</c> if the values of <paramref name="token1"/> and <paramref name="token2"/> are not equal; otherwise, false.</returns>
        public static bool operator !=(SecurityToken token1, SecurityToken token2)
        {
            return !(token1 == token2);
        }

        /// <summary>
        /// Specifies a set of features to apply on the <see cref="SecurityToken"/> object.
        /// </summary>
        /// <returns>A <see cref="SecurityTokenSettings"/> instance that specifies a set of features to apply the <see cref="SecurityToken"/> object.</returns>
        /// <remarks>
        /// The following table shows the initial property values for an instance of <see cref="SecurityTokenSettings"/>.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Property</term>
        ///         <description>Initial Value</description>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="SecurityTokenSettings.LengthOfToken"/></term>
        ///         <description><c>24</c></description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="SecurityTokenSettings.Reference"/></term>
        ///         <description>Empty (<c>""</c>).</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="SecurityTokenSettings.TimeToLive"/></term>
        ///         <description>15 seconds</description>
        ///     </item>
        /// </list>
        /// </remarks>
	    public static SecurityTokenSettings CreateSettings()
	    {
	        return new SecurityTokenSettings();
	    }

        /// <summary>
        /// Specifies a set of features to apply on the <see cref="SecurityToken"/> object.
        /// </summary>
        /// <param name="timeToLive">The amount of time this token remains usable.</param>
        /// <returns>A <see cref="SecurityTokenSettings"/> instance that specifies a set of features to apply on the <see cref="SecurityToken"/> object.</returns>
	    public static SecurityTokenSettings CreateSettings(TimeSpan timeToLive)
	    {
	        return new SecurityTokenSettings(timeToLive);
	    }

        /// <summary>
        /// Specifies a set of features to apply on the <see cref="SecurityToken"/> object.
        /// </summary>
        /// <param name="timeToLive">The amount of time this token remains usable.</param>
        /// <param name="reference">The reference of this token.</param>
        /// <returns>A <see cref="SecurityTokenSettings"/> instance that specifies a set of features to apply on the <see cref="SecurityToken"/> object.</returns>
        public static SecurityTokenSettings CreateSettings(TimeSpan timeToLive, string reference)
        {
            return new SecurityTokenSettings(timeToLive, reference);
        }

        /// <summary>
        /// Specifies a set of features to apply on the <see cref="SecurityToken"/> object.
        /// </summary>
        /// <param name="timeToLive">The amount of time this token remains usable.</param>
        /// <param name="lengthOfToken">The length of the random generated token.</param>
        /// <returns>A <see cref="SecurityTokenSettings"/> instance that specifies a set of features to apply on the <see cref="SecurityToken"/> object.</returns>
        public static SecurityTokenSettings CreateSettings(TimeSpan timeToLive, int lengthOfToken)
        {
            return new SecurityTokenSettings(timeToLive, lengthOfToken);
        }

        /// <summary>
        /// Specifies a set of features to apply on the <see cref="SecurityToken"/> object.
        /// </summary>
        /// <param name="timeToLive">The amount of time this token remains usable.</param>
        /// <param name="lengthOfToken">The length of the random generated token.</param>
        /// <param name="reference">The reference of this token.</param>
        /// <returns>A <see cref="SecurityTokenSettings"/> instance that specifies a set of features to apply on the <see cref="SecurityToken"/> object.</returns>
        public static SecurityTokenSettings CreateSettings(TimeSpan timeToLive, int lengthOfToken, string reference)
        {
            return new SecurityTokenSettings(timeToLive, lengthOfToken, reference);
        }

		/// <summary>
        /// Creates and returns a new <see cref="SecurityToken"/> from the specified <paramref name="settings"/>.
		/// </summary>
        /// <param name="settings">The settings to apply to the <see cref="SecurityToken"/> instance.</param>
		/// <returns>A new <see cref="SecurityToken"/> instance.</returns>
		public static SecurityToken Create(SecurityTokenSettings settings)
		{
			return new SecurityToken(settings);
		}

		/// <summary>
		/// Converts the <see cref="String"/> representation to its <see cref="SecurityToken"/> equivalent.
		/// </summary>
		/// <param name="securityToken">The <see cref="SecurityToken"/> equivalent created by <see cref="ToString"/>.</param>
		/// <returns>A <see cref="SecurityToken"/> instance.</returns>
		public static SecurityToken Parse(string securityToken)
		{
            return new SecurityToken(securityToken);
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance in the following format: ttl;created;token;.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance in the following format: ttl;created;token;.
		/// </returns>
		public override string ToString()
		{
		    bool enableBackwardCompatibility = string.IsNullOrEmpty(this.Settings.Reference);
            return string.Format(CultureInfo.InvariantCulture, enableBackwardCompatibility ? "\"{0}\";\"{1}\";\"{2}\";" : "\"{0}\";\"{1}\";\"{2}\";\"{3}\";", this.Settings.TimeToLive.Ticks, this.UtcCreated.ToString("u", CultureInfo.InvariantCulture), this.Token, this.Settings.Reference);
		}
		#endregion
	}
}