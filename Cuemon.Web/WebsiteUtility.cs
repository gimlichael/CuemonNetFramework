using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.SessionState;
using Cuemon.Globalization;
using Cuemon.Security.Cryptography;
using Cuemon.Web.SessionState;

namespace Cuemon.Web
{
	/// <summary>
	/// This utility class is designed to make some <see cref="Website"/> dependant operations easier to work with, as well as initializing the core values from the <see cref="WebsiteModule"/> class.
	/// </summary>
	/// <remarks>
	/// On all application/session related operations, we had to add a check for NULL values, because if the IIS has just started up
	/// and we are making calls from an object implementing the IHttpModule interface, the aforementioned might actually be NULL.
	/// This is occurring even if hooking up on some of the later events in the state cycle.
	/// </remarks>
	public static class WebsiteUtility
	{
		internal static IList<string> RequestPipelineProcessingExtension = new List<string>();
		internal static readonly string SessionCacheGroup = HashUtility.ComputeHash("SurrogateSession").ToHexadecimal();
		
		/// <summary>
		/// Get the key for the special debug view in a Cuemon enabled website.
		/// </summary>
		internal static readonly string CuemonDebugViewKey = "Cuemon.NET_DebugView_";

		/// <summary>
		/// Get the name of the special Cuemon Session which was optimized for performance.
		/// </summary>
		public static readonly string CuemonSessionName = "Cuemon.NET_SessionId";

		/// <summary>
		/// Determines whether the current request pipeline is valid for application processing.
		/// </summary>
		/// <returns>
		/// 	<c>true</c> if the current request pipeline is valid for application processing; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsCurrentRequestPipelineValidForProcessing()
		{
			string requestedFileExtension = Path.GetExtension(HttpContext.Current.Request.Path) ?? "";
			if (requestedFileExtension.Length == 0) { return true; }
			requestedFileExtension = requestedFileExtension.ToLowerInvariant().Substring(1);
			return RequestPipelineProcessingExtension.Contains(requestedFileExtension);
		}

		/// <summary>
		/// Converts the user language format (which is "&lt;Language&gt;;&lt;q=&lt;weight&gt;&gt;" (e.g., "en-us;q=0.5") to culture info format (without the weight information).
		/// </summary>
		/// <param name="userLanguages">The user languages to convert.</param>
		/// <returns>A string array with <see cref="System.Globalization.CultureInfo"/> compatible values.</returns>
		public static string[] RemoveUserLanguageWeightInformation(string[] userLanguages)
		{
			if (userLanguages == null) { return null; }
			string[] cultureInfos = new string[userLanguages.Length];
			for (int i = 0; i < userLanguages.Length; i++)
			{
				string language = userLanguages[i];
				if (language.IndexOf(';') >= 0)
				{
					language = language.Substring(0, language.IndexOf(';'));
				}
				cultureInfos[i] = language;
			}
			return cultureInfos;
		}

		/// <summary>
		/// Specifies the Session key for a CultureInfo.
		/// </summary>
		internal static readonly string SessionCultureInfoKey = "Cuemon_CultureInfo";

		/// <summary>
		/// Specifies the Application key for a CultureInfo.
		/// </summary>
		internal static readonly string ApplicationCultureInfoKey = string.Format(CultureInfo.InvariantCulture, "Global_{0}", SessionCultureInfoKey);

		/// <summary>
		/// Specifies the Session key for a TimeZone.
		/// </summary>
		internal static readonly string SessionTimeZoneKey = "Cuemon_TimeZone";

		/// <summary>
		/// Specifies the Application key for TimeZone.
		/// </summary>
		internal static readonly string ApplicationTimeZoneKey = string.Format(CultureInfo.InvariantCulture, "Global_{0}", SessionTimeZoneKey);

		private static readonly Dictionary<string, DateTime> refreshList = new Dictionary<string, DateTime>();

		/// <summary>
		/// This method will help determine if the current client is using the refresh option.
		/// </summary>
		/// <param name="request">An instance of the <see cref="HttpRequest"/> object.</param>
		/// <param name="session">An instance of the <see cref="HttpSessionState"/> object.</param>
		/// <returns><c>true</c> if the current request seems to be a refresh of the webpage; otherwise, <c>false</c>.</returns>
		public static bool IsCurrentRequestRefresh(HttpRequest request, IHttpSessionState session)
		{
			return IsCurrentRequestRefresh(request, session, TimeSpan.FromSeconds(5));
		}

		/// <summary>
		/// This method will help determine if the current client is using the refresh option.
		/// </summary>
		/// <param name="request">An instance of the <see cref="HttpRequest"/> object.</param>
		/// <param name="session">An instance of the <see cref="HttpSessionState"/> object.</param>
		/// <param name="maxElapsedTime">The maximum elapsed time used in the determination of an assumed refresh of an webpage. Default is 5 seconds.</param>
		/// <returns><c>true</c> if the current request seems to be a refresh of the webpage; otherwise, <c>false</c>.</returns>
		public static bool IsCurrentRequestRefresh(HttpRequest request, IHttpSessionState session, TimeSpan maxElapsedTime)
		{
			if (request == null) throw new ArgumentNullException("request");
			if (session == null) throw new ArgumentNullException("session");
			string key = HashUtility.ComputeHash(string.Format(CultureInfo.InvariantCulture, "{0}{1}", request.RawUrl.ToUpperInvariant(), session.SessionID)).ToHexadecimal();
			lock (refreshList)
			{
				if (refreshList.ContainsKey(key))
				{
					DateTime currentTime = DateTime.UtcNow;
					DateTime entryTime = refreshList[key];
					TimeSpan elapsedTime = (currentTime - entryTime);
					if (elapsedTime.TotalSeconds >= maxElapsedTime.TotalSeconds)
					{
						refreshList.Remove(key);
						return false;
					}
					refreshList[key] = currentTime;
					return true;
				}
			}

			lock (refreshList)
			{
				if (!refreshList.ContainsKey(key))
				{
					refreshList.Add(key, DateTime.UtcNow);
				}
			}
			return false;
		}

		/// <summary>
		/// Gets or sets the TimeZone identifier for the Application context.
		/// </summary>
		/// <value>The TimeZone identifier for the Application context.</value>
		internal static TimeZoneInfoKey? TimeZoneApplication
		{
			get
			{
				if (HttpContext.Current.Application[ApplicationTimeZoneKey] == null) { return null; }
				return (TimeZoneInfoKey)Enum.Parse(typeof(TimeZoneInfoKey), HttpContext.Current.Application[ApplicationTimeZoneKey].ToString());
			}
			set { HttpContext.Current.Application[ApplicationTimeZoneKey] = value; }
		}

		/// <summary>
		/// Gets or sets the CultureInfo identifier for the Application context.
		/// </summary>
		/// <value>The CultureInfo identifier for the Application context.</value>
		internal static ushort? CultureInfoApplication
		{
			get
			{
				if (HttpContext.Current.Application[ApplicationCultureInfoKey] == null) { return null; }
				return Convert.ToUInt16(HttpContext.Current.Application[ApplicationCultureInfoKey].ToString(), CultureInfo.InvariantCulture);
			}
			set { HttpContext.Current.Application[ApplicationCultureInfoKey] = value; }
		}

		/// <summary>
		/// Gets or sets the TimeZone identifier for the current Cuemon Session.
		/// </summary>
		/// <value>The TimeZone identifier for the current Session.</value>
		/// <remarks>This is a special implementation that bypasses the normal session handling by Microsoft, as this is very slow in a HttpModule.</remarks>
		internal static TimeZoneInfoKey TimezoneBySurrogateSession
		{
			get
			{
				DoModuleImplementationCheck();
				if (FastSession[SessionTimeZoneKey] != null)
				{
					return (TimeZoneInfoKey)FastSession[SessionTimeZoneKey];
				}
				return TimeZoneApplication.Value;
			}
			set
			{
				FastSession[SessionTimeZoneKey] = value;
			}
		}

		/// <summary>
		/// Gets or sets the CultureInfo identifier for the current Cuemon Session,.
		/// </summary>
		/// <value>The CultureInfo identifier for the current Cuemon Session.</value>
		/// <remarks>This is a special implementation that bypasses the normal session handling by Microsoft, as this is very slow in a HttpModule.</remarks>
		internal static ushort CultureInfoBySurrogateSession
		{
			get
			{
				DoModuleImplementationCheck();
				if (FastSession[SessionCultureInfoKey] != null)
				{
					return (ushort)FastSession[SessionCultureInfoKey];
				}
				return CultureInfoApplication.Value;
			}
			set
			{
				FastSession[SessionCultureInfoKey] = value;
			}
		}

		/// <summary>
		/// Gets the fast lightweight (and experimental) Cuemon Session for the current HTTP request.
		/// </summary>
		/// <value>The fast lightweight (and experimental) Cuemon Session for the current HTTP request.</value>
		/// <remarks>Because of the ligthweight implementation, this property can be null in some cases.</remarks>
		public static HttpFastSessionState FastSession
		{
			get
			{
				return new HttpFastSessionState();
			}
		}

		private static void DoModuleImplementationCheck()
		{
			if (CultureInfoApplication == null || TimeZoneApplication == null)
			{
				throw new InvalidOperationException("You must implement a Cuemon.Web.WebsiteModule compatible object in your Web.config file.");
			}
		}
	}
}
