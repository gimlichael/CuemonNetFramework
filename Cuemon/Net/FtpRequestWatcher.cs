using System;
using System.Globalization;
using System.Net;
using Cuemon.IO;
using Cuemon.Runtime;
using Cuemon.Security.Cryptography;

namespace Cuemon.Net
{
    /// <summary>
    /// A <see cref="RequestWatcher{T}"/> implementation, that can monitor and signal change of a <see cref="UriScheme.Ftp"/> protocol bound URI location by raising the <see cref="Watcher.Changed"/> event.
    /// </summary>
    /// <seealso cref="Watcher{T}"/>.
    /// <seealso cref="Watcher"/>.
    public sealed class FtpRequestWatcher : RequestWatcher<FtpWebRequest>
    {
        private readonly object _locker = new object();

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FtpRequestWatcher"/> class.
        /// </summary>
        /// <param name="location">The URI location to monitor for changes.</param>
        /// <remarks>Monitors the provided <paramref name="location"/> for changes, determined by the FTP response.</remarks>
        public FtpRequestWatcher(Uri location)
            : this(location, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FtpRequestWatcher"/> class.
        /// </summary>
        /// <param name="location">The URI location to monitor for changes.</param>
        /// <param name="useFtpDownloadFile">if set to <c>true</c>, a MD5 hash of the FTP RETR response data is used to determine a change state of the resource; <c>false</c> to favor FTP MDTM of the resource.</param>
        /// <remarks>Monitors the provided <paramref name="location"/> for changes, determined by the FTP response.</remarks>
        public FtpRequestWatcher(Uri location, bool useFtpDownloadFile)
            : this(location, useFtpDownloadFile, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FtpRequestWatcher"/> class.
        /// </summary>
        /// <param name="location">The URI location to monitor for changes.</param>
        /// <param name="setup">The <see cref="WatcherOptions" /> which need to be configured.</param>
        /// <remarks>Monitors the provided <paramref name="location"/> for changes as defined by the <paramref name="setup"/> delegate, determined by the FTP response.</remarks>
        public FtpRequestWatcher(Uri location, Act<WatcherOptions> setup)
            : this(location, false, setup)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FtpRequestWatcher"/> class.
        /// </summary>
        /// <param name="location">The URI location to monitor for changes.</param>
        /// <param name="useFtpDownloadFile">if set to <c>true</c>, a MD5 hash of the FTP RETR response data is used to determine a change state of the resource; <c>false</c> to favor FTP MDTM of the resource.</param>
        /// <param name="setup">The <see cref="WatcherOptions"/> which need to be configured.</param>
        /// <remarks>Monitors the provided <paramref name="location"/> for changes as defined by the <paramref name="setup"/> delegate, determined by the FTP response.</remarks>
        public FtpRequestWatcher(Uri location, bool useFtpDownloadFile, Act<WatcherOptions> setup)
            : base(location, useFtpDownloadFile, setup)
        {
            UriScheme scheme = UriSchemeConverter.FromString(location.Scheme);
            switch (scheme)
            {
                case UriScheme.Ftp:
                    break;
                default:
                    throw new ArgumentException("The provided Uri does not have a valid scheme attached. Allowed scheme is FTP.", nameof(location));
            }
        }
        #endregion

        #region Properties
        #endregion

        #region Methods
        /// <summary>
        /// Handles the signaling of this <see cref="FtpRequestWatcher"/>.
        /// </summary>
        protected override void HandleSignaling()
        {
            lock (_locker)
            {
                DateTime utcLastModified = DateTime.UtcNow;
                string currentChecksum = DefaultChecksum;
                string listenerHeader = string.Format(CultureInfo.InvariantCulture, "Cuemon.Net.FtpRequestWatcher; Interval={0} seconds", Period.TotalSeconds);
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(Location);
                WatcherSignalCallback?.Invoke(request);
                request.Headers.Add("Listener-Object", listenerHeader);
                request.Method = UseResponseData ? WebRequestMethods.Ftp.DownloadFile : WebRequestMethods.Ftp.GetDateTimestamp;
                using (FtpWebResponse response = NetUtility.GetFtpWebResponse(request))
                {
                    switch (request.Method)
                    {
                        case WebRequestMethods.Ftp.DownloadFile:
                            currentChecksum = HashUtility.ComputeHash(StreamUtility.CopyStream(response.GetResponseStream())).ToHexadecimal();
                            break;
                        case WebRequestMethods.Ftp.GetDateTimestamp:
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
                    if (utcLastModified > UtcTimestamp)
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