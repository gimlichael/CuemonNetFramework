using System;
using System.Collections.Generic;
using System.Net;
using Cuemon.Collections.Generic;
using Cuemon.IO;
using Cuemon.Net.Http;
using Cuemon.Runtime;

namespace Cuemon.Net
{
    /// <summary>
    /// This <see cref="NetDependency"/> class will monitor any changes occurred to one or more <see cref="Uri"/> values while notifying subscribing objects.
    /// </summary>
    public class NetDependency : WatcherDependency
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="NetDependency"/> class.
        /// </summary>
        /// <param name="location">The URI to monitor for changes.</param>
        /// <remarks>The signaling is default delayed 15 seconds before first invoke.</remarks>
        public NetDependency(Uri location)
            : this(EnumerableUtility.Yield(location), false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetDependency"/> class.
        /// </summary>
        /// <param name="locations">An <see cref="IEnumerable{T}"/> of URI locations that this <see cref="NetDependency"/> will monitor. When any of these resources changes, this <see cref="NetDependency"/> will notify any subscribing objects of the change.</param>
        /// <remarks>The signaling is default delayed 15 seconds before first invoke. Signaling occurs every 2 minutes.</remarks>
        public NetDependency(IEnumerable<Uri> locations)
            : this(locations, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetDependency"/> class.
        /// </summary>
        /// <param name="locations">An <see cref="IEnumerable{T}"/> of URI locations that this <see cref="NetDependency"/> will monitor. When any of these resources changes, this <see cref="NetDependency"/> will notify any subscribing objects of the change.</param>
        /// <param name="useResponseData">if set to <c>true</c>, a MD5 hash check of the response data is used to determine a change state of the resource; <c>false</c> to check only for the last modification of the resource.</param>
        /// <remarks>The signaling is default delayed 15 seconds before first invoke. Signaling occurs every 2 minutes. The <paramref name="useResponseData"/> is useful, when the web server you are probing does not contain the Last-Modified header.</remarks>
        public NetDependency(IEnumerable<Uri> locations, bool useResponseData)
            : this(locations, useResponseData, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetDependency"/> class.
        /// </summary>
        /// <param name="locations">An <see cref="IEnumerable{T}"/> of URI locations that this <see cref="NetDependency"/> will monitor. When any of these resources changes, this <see cref="NetDependency"/> will notify any subscribing objects of the change.</param>
        /// <param name="setup">The <see cref="WatcherOptions"/> which need to be configured.</param>
        /// <remarks>The signaling is default delayed 15 seconds before first invoke.</remarks>
        public NetDependency(IEnumerable<Uri> locations, Act<WatcherOptions> setup)
            : this(locations, false, setup)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetDependency"/> class.
        /// </summary>
        /// <param name="locations">An array of URI locations that this <see cref="NetDependency"/> will monitor. When any of these resources changes, this <see cref="NetDependency"/> will notify any subscribing objects of the change.</param>
        /// <remarks>The signaling is default delayed 15 seconds before first invoke. Signaling occurs every 2 minutes.</remarks>
        public NetDependency(params Uri[] locations)
            : this(locations as IEnumerable<Uri>)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetDependency"/> class.
        /// </summary>
        /// <param name="useResponseData">if set to <c>true</c>, a MD5 hash check of the response data is used to determine a change state of the resource; <c>false</c> to check only for the last modification of the resource.</param>
        /// <param name="locations">An array of URI locations that this <see cref="NetDependency"/> will monitor. When any of these resources changes, this <see cref="NetDependency"/> will notify any subscribing objects of the change.</param>
        /// <remarks>The signaling is default delayed 15 seconds before first invoke. Signaling occurs every 2 minutes. The <paramref name="useResponseData"/> is useful, when the web server you are probing does not contain the Last-Modified header.</remarks>
        public NetDependency(bool useResponseData, params Uri[] locations)
            : this(locations, useResponseData)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetDependency"/> class.
        /// </summary>
        /// <param name="setup">The <see cref="WatcherOptions"/> which need to be configured.</param>
        /// <param name="locations">An array of URI locations that this <see cref="NetDependency"/> will monitor. When any of these resources changes, this <see cref="NetDependency"/> will notify any subscribing objects of the change.</param>
        public NetDependency(Act<WatcherOptions> setup, params Uri[] locations)
            : this(locations, false, setup)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetDependency"/> class.
        /// </summary>
        /// <param name="useResponseData">if set to <c>true</c>, the request will invoke and retrieve the response data which can be fairly expensive; <c>false</c> to retrieve the response headers only.</param>
        /// <param name="setup">The <see cref="WatcherOptions"/> which need to be configured.</param>
        /// <param name="locations">An array of URI locations that this <see cref="NetDependency"/> will monitor. When any of these resources changes, this <see cref="NetDependency"/> will notify any subscribing objects of the change.</param>
        /// <remarks>The <paramref name="useResponseData"/> is useful when the web server you are probing does not contain the Last-Modified header.</remarks>
        public NetDependency(bool useResponseData, Act<WatcherOptions> setup, params Uri[] locations)
            : this(locations, useResponseData, setup)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetDependency"/> class.
        /// </summary>
        /// <param name="locations">An <see cref="IEnumerable{T}"/> of URI locations that this <see cref="NetDependency"/> will monitor. When any of these resources changes, this <see cref="NetDependency"/> will notify any subscribing objects of the change.</param>
        /// <param name="useResponseData">if set to <c>true</c>, the request will invoke and retrieve the response data which can be fairly expensive; <c>false</c> to retrieve the response headers only.</param>
        /// <param name="setup">The <see cref="WatcherOptions"/> which need to be configured.</param>
        /// <remarks>The <paramref name="useResponseData"/> is useful when the web server you are probing does not contain the Last-Modified header.</remarks>
        public NetDependency(IEnumerable<Uri> locations, bool useResponseData, Act<WatcherOptions> setup)
            : this(locations, useResponseData, setup, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NetDependency"/> class.
        /// </summary>
        /// <param name="locations">An <see cref="IEnumerable{T}"/> of URI locations that this <see cref="NetDependency"/> will monitor. When any of these resources changes, this <see cref="NetDependency"/> will notify any subscribing objects of the change.</param>
        /// <param name="useResponseData">if set to <c>true</c>, the request will invoke and retrieve the response data which can be fairly expensive; <c>false</c> to retrieve the response headers only.</param>
        /// <param name="setup">The <see cref="WatcherOptions"/> which need to be configured.</param>
        /// <param name="requestCallback">The <see cref="WebRequest"/> which can be configured.</param>
        /// <remarks>The <paramref name="useResponseData"/> is useful when the web server you are probing does not contain the Last-Modified header.</remarks>
        public NetDependency(IEnumerable<Uri> locations, bool useResponseData, Act<WatcherOptions> setup, Act<WebRequest> requestCallback)
            : base(() =>
            {
                Validator.ThrowIfNull(locations, nameof(locations));
                List<Watcher> watchers = new List<Watcher>();
                foreach (Uri location in locations)
                {
                    switch (UriSchemeConverter.FromString(location.Scheme))
                    {
                        case UriScheme.File:
                            var fileWatcher = DelegateUtility.SafeInvokeDisposable(() =>
                            {
                                var w = new FileRequestWatcher(location, setup);
                                w.WatcherSignalCallback = requestCallback;
                                return w;
                            });
                            watchers.Add(fileWatcher);
                            break;
                        case UriScheme.Ftp:
                            var ftpWatcher = DelegateUtility.SafeInvokeDisposable(() =>
                            {
                                var w = new FtpRequestWatcher(location, useResponseData, setup);
                                w.WatcherSignalCallback = requestCallback;
                                return w;
                            });
                            watchers.Add(ftpWatcher);
                            break;
                        case UriScheme.Http:
                        case UriScheme.Https:
                            var httpWatcher = DelegateUtility.SafeInvokeDisposable(() =>
                            {
                                var w = new HttpRequestWatcher(location, useResponseData, setup);
                                w.WatcherSignalCallback = requestCallback;
                                return w;
                            });
                            watchers.Add(httpWatcher);
                            break;
                        default:
                            throw new InvalidOperationException("The provided Uri does not have a valid scheme attached. Allowed schemes are File, FTP, HTTP or HTTPS.");
                    }
                }
                return watchers;
            })
        {
        }


        #endregion

        #region Properties
        #endregion

        #region Methods
        #endregion
    }
}