using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using Cuemon.IO;
using Cuemon.Security.Cryptography;
using Cuemon.Net.Http;

namespace Cuemon.Net
{
	/// <summary>
	/// A <see cref="Watcher"/> implementation, that can monitor and signal changes of one or more URI locations by raising the <see cref="Watcher.Changed"/> event.
	/// </summary>
	public sealed class NetWatcher : Watcher
	{
		private readonly object _locker = new object();

		#region Constructors
		NetWatcher()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NetWatcher"/> class.
		/// </summary>
		/// <param name="requestUri">The request URI to monitor for changes.</param>
		/// <remarks>Monitors the provided <paramref name="requestUri"/> for changes in an interval of two minutes, using the last modified timestamp of the ressource.</remarks>
		public NetWatcher(Uri requestUri) : this(requestUri, false)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NetWatcher"/> class.
		/// </summary>
		/// <param name="requestUri">The request URI to monitor for changes.</param>
		/// <param name="period">The time interval between periodic signaling for changes of provided <paramref name="requestUri"/>.</param>
        /// <remarks>Monitors the provided <paramref name="requestUri"/> for changes in an interval specified by <paramref name="period"/>, using the last modified time stamp of the resource.</remarks>
		public NetWatcher(Uri requestUri, TimeSpan period) : this(requestUri, period, false)
		{   
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NetWatcher"/> class.
		/// </summary>
		/// <param name="requestUri">The request URI to monitor for changes.</param>
        /// <param name="checkResponseData">if set to <c>true</c>, a MD5 hash check of the response data is used to determine a change state of the resource; <c>false</c> to check only for the last modification of the resource.</param>
		/// <remarks>Monitors the provided <paramref name="requestUri"/> for changes in an interval of two minutes, determined by <paramref name="checkResponseData"/>.</remarks>
		public NetWatcher(Uri requestUri, bool checkResponseData) : this(requestUri, TimeSpan.FromMinutes(2), checkResponseData)
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="NetWatcher"/> class.
		/// </summary>
		/// <param name="requestUri">The request URI to monitor for changes.</param>
		/// <param name="period">The time interval between periodic signaling for changes of provided <paramref name="requestUri"/>.</param>
        /// <param name="checkResponseData">if set to <c>true</c>, a MD5 hash check of the response data is used to determine a change state of the resource; <c>false</c> to check only for the last modification of the resource.</param>
		/// <remarks>Monitors the provided <paramref name="requestUri"/> for changes in an interval specified by <paramref name="period"/>, determined by <paramref name="checkResponseData"/>. The signaling is default delayed 15 seconds before first invoke.</remarks>
		public NetWatcher(Uri requestUri, TimeSpan period, bool checkResponseData) : this(requestUri, TimeSpan.FromSeconds(15), period, checkResponseData)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NetWatcher"/> class.
		/// </summary>
		/// <param name="requestUri">The request URI to monitor for changes.</param>
		/// <param name="period">The time interval between periodic signaling for changes of provided <paramref name="requestUri"/>.</param>
		/// <param name="dueTime">The amount of time to delay before the associated <see cref="Watcher"/> starts signaling. Specify negative one (-1) milliseconds to prevent the signaling from starting. Specify zero (0) to start the signaling immediately.</param>
        /// <param name="checkResponseData">if set to <c>true</c>, a MD5 hash check of the response data is used to determine a change state of the resource; <c>false</c> to check only for the last modification of the resource.</param>
		/// <remarks>Monitors the provided <paramref name="requestUri"/> for changes in an interval specified by <paramref name="period"/>, determined by <paramref name="checkResponseData"/>.</remarks>
		public NetWatcher(Uri requestUri, TimeSpan dueTime, TimeSpan period, bool checkResponseData) : base(dueTime, period)
		{
			if (requestUri == null) throw new ArgumentNullException("requestUri");
			UriScheme scheme = UriUtility.ParseScheme(requestUri.Scheme);
			switch (scheme)
			{
				case UriScheme.File:
				case UriScheme.Ftp:
				case UriScheme.Http:
					break;
				default:
					throw new ArgumentException("The provided Uri does not have a valid scheme attached. Allowed schemes for now is File, FTP or HTTP.", "requestUri");
			}
			this.RequestUri = requestUri;
			this.Scheme = scheme;
			this.UtcCreated = DateTime.UtcNow;
			this.CheckResponseData = checkResponseData;
			this.Signature = null;
		}
		#endregion

		#region Properties
		private string Signature { get; set; }

		/// <summary>
		/// Gets a value indicating whether to perform a MD5 hash-check of the response data from the <see cref="RequestUri"/>.
		/// </summary>
		/// <value><c>true</c> to perform a MD5 hash-check of the response data from the <see cref="RequestUri"/>; otherwise, <c>false</c>.</value>
		public bool CheckResponseData { get; private set; }

		private DateTime UtcCreated { get; set; }

		/// <summary>
		/// Gets the associated request URI of this <see cref="NetWatcher"/>.
		/// </summary>
		/// <value>The associated request URI of this <see cref="NetWatcher"/>.</value>
		public Uri RequestUri { get; private set; }

		/// <summary>
		/// Gets the <see cref="UriScheme"/> of this <see cref="NetWatcher"/>.
		/// </summary>
		/// <value>An <see cref="UriScheme"/> of this <see cref="NetWatcher"/>.</value>
		public UriScheme Scheme { get; private set; }
		#endregion

		#region Methods
		/// <summary>
		/// Handles the signaling of this <see cref="NetWatcher"/>.
		/// </summary>
		protected override void HandleSignaling()
		{
			lock (_locker)
			{
				string currentSignature = null;
				string listenerHeader = string.Format(CultureInfo.InvariantCulture, "Cuemon.Net.NetWatcher; Interval={0} seconds", this.Period.TotalSeconds);
				DateTime utcLastModified = DateTime.UtcNow;
				switch (this.Scheme)
				{
					case UriScheme.File:
						utcLastModified = File.GetLastWriteTimeUtc(this.RequestUri.LocalPath);
						if (this.CheckResponseData)
						{
							using (FileStream stream = new FileStream(this.RequestUri.LocalPath, FileMode.Open, FileAccess.Read))
							{
								stream.Position = 0;
								currentSignature = HashUtility.ComputeHash(stream);
							}
						}
						break;
					case UriScheme.Ftp:
                        WebRequest request = WebRequest.Create(this.RequestUri);
						request.Headers.Add("Listener-Object", listenerHeader);
						request.Method = this.CheckResponseData ? WebRequestMethods.Ftp.DownloadFile : WebRequestMethods.Ftp.GetDateTimestamp;
						using (FtpWebResponse response = NetUtility.GetFtpWebResponse((FtpWebRequest)request))
						{
							switch (request.Method)
							{
								case WebRequestMethods.Ftp.DownloadFile:
									currentSignature = HashUtility.ComputeHash(StreamUtility.CopyStream(response.GetResponseStream()));
									break;
								case WebRequestMethods.Ftp.GetDateTimestamp:
									utcLastModified = response.LastModified.ToUniversalTime();
									break;
							}
						}
						break;
					case UriScheme.Http:
                        HttpWebRequestSettings settings = new HttpWebRequestSettings();
                        settings.Headers.Add("Listener-Object", listenerHeader);
                        using (HttpWebResponse response = this.CheckResponseData ? NetHttpUtility.HttpGet(this.RequestUri, settings) : NetHttpUtility.HttpHead(this.RequestUri, settings))
						{
							switch (response.Method)
							{
								case WebRequestMethods.Http.Get:
							        string etag = response.Headers[HttpResponseHeader.ETag];
									currentSignature = string.IsNullOrEmpty(etag) ? HashUtility.ComputeHash(StreamUtility.CopyStream(response.GetResponseStream())) : etag;
									break;
								case WebRequestMethods.Http.Head:
									utcLastModified = response.LastModified.ToUniversalTime();
									break;
							}
						}
						break;
					default:
						throw new InvalidOperationException("Only allowed schemes for now is File, FTP or HTTP.");
				}

				if (this.CheckResponseData)
				{
					if (this.Signature == null) { this.Signature = currentSignature; }
					if (!this.Signature.Equals(currentSignature, StringComparison.OrdinalIgnoreCase))
					{
						this.SetUtcLastModified(utcLastModified);
						this.OnChangedRaised();
					}
					this.Signature = currentSignature;
					return;
				}

				if (utcLastModified > this.UtcCreated)
				{
					this.SetUtcLastModified(utcLastModified);
					this.OnChangedRaised();
				}
			}
		}
		#endregion
	}
}