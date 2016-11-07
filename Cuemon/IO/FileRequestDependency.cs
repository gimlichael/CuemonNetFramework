using System;
using System.Collections.Generic;
using System.Net;
using Cuemon.Collections.Generic;
using Cuemon.Runtime;

namespace Cuemon.IO
{
    /// <summary>
    /// This <see cref="FileRequestDependency"/> class will monitor any changes occurred to one or more <see cref="UriScheme.File"/> specific <see cref="Uri"/> values while notifying subscribing objects.
    /// </summary>
    public sealed class FileRequestDependency : WatcherDependency
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FileRequestDependency"/> class.
        /// </summary>
        /// <param name="value">The URI to monitor for changes.</param>
        public FileRequestDependency(Uri value)
            : this(EnumerableUtility.Yield(value))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileRequestDependency"/> class.
        /// </summary>
        /// <param name="locations">An <see cref="IEnumerable{T}"/> of URI locations that this <see cref="FileRequestDependency"/> will monitor. When any of these resources changes, this <see cref="FileRequestDependency"/> will notify any subscribing objects of the change.</param>
        /// <remarks>The signaling is default delayed 15 seconds before first invoke. Signaling occurs every 2 minutes.</remarks>
        public FileRequestDependency(IEnumerable<Uri> locations)
            : this(locations, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileRequestDependency"/> class.
        /// </summary>
        /// <param name="locations">An <see cref="IEnumerable{T}"/> of URI locations that this <see cref="FileRequestDependency"/> will monitor. When any of these resources changes, this <see cref="FileRequestDependency"/> will notify any subscribing objects of the change.</param>
        /// <param name="setup">The <see cref="WatcherOptions"/> which need to be configured.</param>
        public FileRequestDependency(IEnumerable<Uri> locations, Act<WatcherOptions> setup)
            : this(locations, setup, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileRequestDependency"/> class.
        /// </summary>
        /// <param name="locations">An <see cref="IEnumerable{T}"/> of URI locations that this <see cref="FileRequestDependency"/> will monitor. When any of these resources changes, this <see cref="FileRequestDependency"/> will notify any subscribing objects of the change.</param>
        /// <param name="setup">The <see cref="WatcherOptions"/> which need to be configured.</param>
        /// <param name="requestCallback">The <see cref="FileWebRequest"/> which can be configured.</param>
        public FileRequestDependency(IEnumerable<Uri> locations, Act<WatcherOptions> setup, Act<FileWebRequest> requestCallback)
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
                        default:
                            throw new InvalidOperationException("The provided Uri does not have a valid scheme attached. Allowed scheme is File.");
                    }
                }
                return watchers;
            })
        {
        }
        #endregion
    }
}