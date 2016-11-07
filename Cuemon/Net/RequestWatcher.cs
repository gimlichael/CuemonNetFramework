using System;
using System.Net;
using Cuemon.Runtime;

namespace Cuemon.Net
{
    /// <summary>
    /// An abstract class for establishing a <see cref="WebRequest"/> based watcher.
    /// </summary>
    /// <seealso cref="Watcher{T}" />
    public abstract class RequestWatcher<T> : Watcher<T> where T : WebRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestWatcher{T}"/> class.
        /// </summary>
        protected RequestWatcher()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestWatcher{T}"/> class.
        /// </summary>
        /// <param name="location">The URI location to monitor for changes.</param>
        protected RequestWatcher(Uri location)
            : this(location, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestWatcher{T}"/> class.
        /// </summary>
        /// <param name="location">The URI location to monitor for changes.</param>
        /// <param name="useResponseData">if set to <c>true</c>, the request will invoke and retrieve the response data which can be fairly expensive; <c>false</c> to retrieve the response headers only.</param>
        protected RequestWatcher(Uri location, bool useResponseData)
            : this(location, useResponseData, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestWatcher{T}"/> class.
        /// </summary>
        /// <param name="location">The URI location to monitor for changes.</param>
        /// <param name="setup">The <see cref="WatcherOptions"/> which need to be configured.</param>
        protected RequestWatcher(Uri location, Act<WatcherOptions> setup)
            : this(location, false, setup)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestWatcher{T}"/> class.
        /// </summary>
        /// <param name="location">The URI location to monitor for changes.</param>
        /// <param name="useResponseData">if set to <c>true</c>, the request will invoke and retrieve the response data which can be fairly expensive; <c>false</c> to retrieve the response headers only.</param>
        /// <param name="setup">The <see cref="WatcherOptions"/> which need to be configured.</param>
        protected RequestWatcher(Uri location, bool useResponseData, Act<WatcherOptions> setup) : base(setup)
        {
            Validator.ThrowIfNull(location, nameof(location));
            Location = location;
            Checksum = DefaultChecksum;
            UseResponseData = useResponseData;
            UtcTimestamp = DateTime.UtcNow;
        }

        /// <summary>
        /// Gets a value indicating whether to invoke <see cref="WebResponse.GetResponseStream"/>.
        /// </summary>
        /// <value><c>true</c> to invoke and retrieve the response data of the request; otherwise, <c>false</c>.</value>
        public bool UseResponseData { get; protected set; }

        /// <summary>
        /// Gets the URI location of the request.
        /// </summary>
        /// <value>The URI location of the request.</value>
        public Uri Location { get; protected set; }

        /// <summary>
        /// Gets the timestamp in Coordinated Universal Time (UTC) (also known as Greenwich Mean Time) of this <see cref="RequestWatcher{T}"/>.
        /// </summary>
        /// <value>The timestamp in Coordinated Universal Time (UTC) (also known as Greenwich Mean Time) of this <see cref="RequestWatcher{T}"/>.</value>
        protected DateTime UtcTimestamp { get; private set; }

        /// <summary>
        /// Signals the <see cref="Watcher.Changed"/> event.
        /// </summary>
        /// <param name="utcLastModified">The Coordinated Universal Time (UTC) (also known as Greenwich Mean Time) of the last modified timestamp.</param>
        protected void SignalChanged(DateTime utcLastModified)
        {
            SetUtcLastModified(utcLastModified);
            OnChangedRaised();
        }
    }
}