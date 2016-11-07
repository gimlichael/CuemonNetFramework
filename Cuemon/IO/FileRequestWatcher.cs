using System;
using System.Globalization;
using System.Net;
using Cuemon.Net;
using Cuemon.Runtime;
using Cuemon.Security.Cryptography;

namespace Cuemon.IO
{
    /// <summary>
    /// A <see cref="RequestWatcher{T}"/> implementation, that can monitor and signal change of a <see cref="UriScheme.File"/> protocol bound URI location by raising the <see cref="Watcher.Changed"/> event.
    /// </summary>
    /// <seealso cref="Watcher{T}"/>.
    /// <seealso cref="Watcher"/>.
    public sealed class FileRequestWatcher : RequestWatcher<FileWebRequest>
    {
        private readonly object _locker = new object();

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FileRequestWatcher"/> class.
        /// </summary>
        /// <param name="location">The URI location to monitor for changes.</param>
        /// <remarks>Monitors the provided <paramref name="location"/> for changes, determined by the HTTP response.</remarks>
        public FileRequestWatcher(Uri location)
            : this(location, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileRequestWatcher"/> class.
        /// </summary>
        /// <param name="location">The URI location to monitor for changes.</param>
        /// <param name="setup">The <see cref="WatcherOptions" /> which need to be configured.</param>
        /// <remarks>Monitors the provided <paramref name="location"/> for changes as defined by the <paramref name="setup"/> delegate, determined by the HTTP response.</remarks>
        public FileRequestWatcher(Uri location, Act<WatcherOptions> setup)
            : base(location, true, setup)
        {
            UriScheme scheme = UriSchemeConverter.FromString(location.Scheme);
            switch (scheme)
            {
                case UriScheme.File:
                    break;
                default:
                    throw new ArgumentException("The provided Uri does not have a valid scheme attached. Allowed scheme is File.", nameof(location));
            }
        }
        #endregion

        #region Properties
        #endregion

        #region Methods
        /// <summary>
        /// Handles the signaling of this <see cref="FileRequestWatcher"/>.
        /// </summary>
        protected override void HandleSignaling()
        {
            lock (_locker)
            {
                DateTime utcLastModified = DateTime.UtcNow;
                string listenerHeader = string.Format(CultureInfo.InvariantCulture, "Cuemon.IO.FileRequestWatcher; Interval={0} seconds", Period.TotalSeconds);
                FileWebRequest request = (FileWebRequest)WebRequest.Create(Location);
                WatcherSignalCallback?.Invoke(request);
                request.Headers.Add("Listener-Object", listenerHeader);
                request.Method = WebRequestMethods.File.DownloadFile;
                using (FileWebResponse response = NetUtility.GetFileWebResponse(request))
                {
                    string currentChecksum = HashUtility.ComputeHash(StreamUtility.CopyStream(response.GetResponseStream())).ToHexadecimal();
                    if (Checksum == DefaultChecksum) { Checksum = currentChecksum; }
                    if (!Infrastructure.IsOrdinalIgnoreCaseEqual(Checksum, currentChecksum))
                    {
                        SignalChanged(utcLastModified);
                    }
                    Checksum = currentChecksum;
                }
            }
        }
        #endregion
    }
}