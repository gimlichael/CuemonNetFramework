using System;
using System.Collections.Generic;
using System.Net;
using Cuemon.Collections.Generic;
using Cuemon.Runtime;

namespace Cuemon.Net
{
    /// <summary>
    /// This <see cref="FtpRequestDependency"/> class will monitor any changes occurred to one or more <see cref="UriScheme.Ftp"/> specific <see cref="Uri"/> values while notifying subscribing objects.
    /// </summary>
    public sealed class FtpRequestDependency : WatcherDependency
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FtpRequestDependency"/> class.
        /// </summary>
        /// <param name="value">The URI to monitor for changes.</param>
        public FtpRequestDependency(Uri value)
            : this(EnumerableUtility.Yield(value))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FtpRequestDependency"/> class.
        /// </summary>
        /// <param name="locations">An <see cref="IEnumerable{T}"/> of URI locations that this <see cref="FtpRequestDependency"/> will monitor. When any of these resources changes, this <see cref="FtpRequestDependency"/> will notify any subscribing objects of the change.</param>
        /// <remarks>The signaling is default delayed 15 seconds before first invoke. Signaling occurs every 2 minutes.</remarks>
        public FtpRequestDependency(IEnumerable<Uri> locations)
            : this(locations, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FtpRequestDependency"/> class.
        /// </summary>
        /// <param name="locations">An <see cref="IEnumerable{T}"/> of URI locations that this <see cref="FtpRequestDependency"/> will monitor. When any of these resources changes, this <see cref="FtpRequestDependency"/> will notify any subscribing objects of the change.</param>
        /// <param name="useResponseData">if set to <c>true</c>, a MD5 hash check of the response data is used to determine a change state of the resource; <c>false</c> to check only for the last modification of the resource.</param>
        public FtpRequestDependency(IEnumerable<Uri> locations, bool useResponseData)
            : this(locations, useResponseData, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetDependency"/> class.
        /// </summary>
        /// <param name="locations">An <see cref="IEnumerable{T}"/> of URI locations that this <see cref="FtpRequestDependency"/> will monitor. When any of these resources changes, this <see cref="FtpRequestDependency"/> will notify any subscribing objects of the change.</param>
        /// <param name="useResponseData">if set to <c>true</c>, the request will invoke and retrieve the response data which can be fairly expensive; <c>false</c> to retrieve the response headers only.</param>
        /// <param name="setup">The <see cref="WatcherOptions"/> which need to be configured.</param>
        public FtpRequestDependency(IEnumerable<Uri> locations, bool useResponseData, Act<WatcherOptions> setup)
            : this(locations, useResponseData, setup, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetDependency"/> class.
        /// </summary>
        /// <param name="locations">An <see cref="IEnumerable{T}"/> of URI locations that this <see cref="FtpRequestDependency"/> will monitor. When any of these resources changes, this <see cref="FtpRequestDependency"/> will notify any subscribing objects of the change.</param>
        /// <param name="useResponseData">if set to <c>true</c>, the request will invoke and retrieve the response data which can be fairly expensive; <c>false</c> to retrieve the response headers only.</param>
        /// <param name="setup">The <see cref="WatcherOptions"/> which need to be configured.</param>
        /// <param name="requestCallback">The <see cref="FtpWebRequest"/> which can be configured.</param>
        public FtpRequestDependency(IEnumerable<Uri> locations, bool useResponseData, Act<WatcherOptions> setup, Act<FtpWebRequest> requestCallback)
            : base(() =>
            {
                Validator.ThrowIfNull(locations, nameof(locations));
                List<Watcher> watchers = new List<Watcher>();
                foreach (Uri location in locations)
                {
                    switch (UriSchemeConverter.FromString(location.Scheme))
                    {
                        case UriScheme.Ftp:
                            var ftpWatcher = DelegateUtility.SafeInvokeDisposable(() =>
                            {
                                var w = new FtpRequestWatcher(location, useResponseData, setup);
                                w.WatcherSignalCallback = requestCallback;
                                return w;
                            });
                            watchers.Add(ftpWatcher);
                            break;
                        default:
                            throw new InvalidOperationException("The provided Uri does not have a valid scheme attached. Allowed scheme is FTP.");
                    }
                }
                return watchers;
            })
        {
        }
        #endregion
    }
}