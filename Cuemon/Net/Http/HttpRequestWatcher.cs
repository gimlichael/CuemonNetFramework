using System;
using System.Globalization;
using System.Net;
using Cuemon.IO;
using Cuemon.Runtime;
using Cuemon.Security.Cryptography;

namespace Cuemon.Net.Http
{
    /// <summary>
    /// A <see cref="RequestWatcher{T}"/> implementation, that can monitor and signal change of a <see cref="UriScheme.Http"/> or <see cref="UriScheme.Https"/> protocol bound URI location by raising the <see cref="Watcher.Changed"/> event.
    /// </summary>
    /// <seealso cref="Watcher{T}"/>.
    /// <seealso cref="Watcher"/>.
    public sealed class HttpRequestWatcher : RequestWatcher<HttpWebRequest>
    {
        private readonly object _locker = new object();

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequestWatcher"/> class.
        /// </summary>
        /// <param name="location">The URI location to monitor for changes.</param>
        /// <remarks>Monitors the provided <paramref name="location"/> for changes, determined by the HTTP response.</remarks>
        public HttpRequestWatcher(Uri location)
            : this(location, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequestWatcher"/> class.
        /// </summary>
        /// <param name="location">The URI location to monitor for changes.</param>
        /// <param name="useFtpDownloadFile">if set to <c>true</c>, a MD5 hash of the FTP RETR response data is used to determine a change state of the resource; <c>false</c> to favor FTP MDTM of the resource.</param>
        /// <remarks>Monitors the provided <paramref name="location"/> for changes, determined by the HTTP response.</remarks>
        public HttpRequestWatcher(Uri location, bool useFtpDownloadFile)
            : this(location, useFtpDownloadFile, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequestWatcher"/> class.
        /// </summary>
        /// <param name="location">The URI location to monitor for changes.</param>
        /// <param name="setup">The <see cref="WatcherOptions" /> which need to be configured.</param>
        /// <remarks>Monitors the provided <paramref name="location"/> for changes as defined by the <paramref name="setup"/> delegate, determined by the HTTP response.</remarks>
        public HttpRequestWatcher(Uri location, Act<WatcherOptions> setup)
            : this(location, false, setup)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequestWatcher"/> class.
        /// </summary>
        /// <param name="location">The URI location to monitor for changes.</param>
        /// <param name="useHttpGet">if set to <c>true</c>, a MD5 hash of the HTTP GET response body is used to determine a change state of the resource; <c>false</c> to favor HTTP HEAD of the resource.</param>
        /// <param name="setup">The <see cref="WatcherOptions"/> which need to be configured.</param>
        /// <remarks>Monitors the provided <paramref name="location"/> for changes as defined by the <paramref name="setup"/> delegate, determined by the HTTP response.</remarks>
        public HttpRequestWatcher(Uri location, bool useHttpGet, Act<WatcherOptions> setup)
            : base(location, useHttpGet, setup)
        {
            UriScheme scheme = UriSchemeConverter.FromString(location.Scheme);
            switch (scheme)
            {
                case UriScheme.Http:
                case UriScheme.Https:
                    break;
                default:
                    throw new ArgumentException("The provided Uri does not have a valid scheme attached. Allowed schemes are HTTP and HTTPS.", nameof(location));
            }
        }
        #endregion

        #region Properties
        #endregion

        #region Methods
        /// <summary>
        /// Handles the signaling of this <see cref="HttpRequestWatcher"/>.
        /// </summary>
        protected override void HandleSignaling()
        {
            lock (_locker)
            {
                DateTime utcLastModified = DateTime.UtcNow;
                string currentChecksum = DefaultChecksum;
                string listenerHeader = string.Format(CultureInfo.InvariantCulture, "Cuemon.Net.Http.HttpRequestWatcher; Interval={0} seconds", Period.TotalSeconds);
                HttpWebRequest request = NetHttpUtility.CreateRequest(Location, UseResponseData ? WebRequestMethods.Http.Get : WebRequestMethods.Http.Head, options =>
                {
                    options.Headers.Add("Listener-Object", listenerHeader);
                });
                WatcherSignalCallback?.Invoke(request);
                using (HttpWebResponse response = NetHttpUtility.Http(request))
                {
                    switch (response.Method)
                    {
                        case WebRequestMethods.Http.Get:
                            currentChecksum = HashUtility.ComputeHash(StreamUtility.CopyStream(response.GetResponseStream())).ToHexadecimal();
                            break;
                        case WebRequestMethods.Http.Head:
                            string etag = response.Headers[HttpResponseHeader.ETag];
                            currentChecksum = string.IsNullOrEmpty(etag) ? "" : etag;
                            utcLastModified = response.LastModified.ToUniversalTime();
                            break;
                    }
                }

                if (Checksum == DefaultChecksum) { Checksum = currentChecksum; }
                if (UseResponseData)
                {
                    if (!Infrastructure.IsOrdinalIgnoreCaseEqual(Checksum, currentChecksum))
                    {
                        SignalChanged(utcLastModified);
                    }
                }
                else
                {
                    if ((currentChecksum != "" && !Infrastructure.IsOrdinalIgnoreCaseEqual(Checksum, currentChecksum)) ||
                        (utcLastModified > UtcTimestamp))
                    {
                        SignalChanged(utcLastModified);
                    }
                }
                Checksum = currentChecksum;
            }
        }
        #endregion
    }
}