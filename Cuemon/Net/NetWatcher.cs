﻿using System;
using System.Globalization;
using System.IO;
using System.Net;
using Cuemon.IO;
using Cuemon.Net.Http;
using Cuemon.Runtime;
using Cuemon.Security.Cryptography;

namespace Cuemon.Net
{
    /// <summary>
    /// A <see cref="Watcher"/> implementation, that can monitor and signal changes of one or more URI locations by raising the <see cref="Watcher.Changed"/> event.
    /// </summary>
    public sealed class NetWatcher : Watcher
    {
        private readonly object _locker = new object();
        private static readonly string SignatureDefault = StringUtility.CreateRandomString(32);

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
            if (requestUri == null) throw new ArgumentNullException(nameof(requestUri));
            UriScheme scheme = UriSchemeConverter.FromString(requestUri.Scheme);
            switch (scheme)
            {
                case UriScheme.File:
                case UriScheme.Ftp:
                case UriScheme.Http:
                case UriScheme.Https:
                    break;
                default:
                    throw new ArgumentException("The provided Uri does not have a valid scheme attached. Allowed schemes for now is File, FTP, HTTP or HTTPS.", nameof(requestUri));
            }
            RequestUri = requestUri;
            Scheme = scheme;
            UtcCreated = DateTime.UtcNow;
            CheckResponseData = checkResponseData;
            Signature = SignatureDefault;
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
                string currentSignature = SignatureDefault;
                string listenerHeader = string.Format(CultureInfo.InvariantCulture, "Cuemon.Net.NetWatcher; Interval={0} seconds", Period.TotalSeconds);
                DateTime utcLastModified = DateTime.UtcNow;
                switch (Scheme)
                {
                    case UriScheme.File:
                        utcLastModified = File.GetLastWriteTimeUtc(RequestUri.LocalPath);
                        if (CheckResponseData)
                        {
                            using (FileStream stream = new FileStream(RequestUri.LocalPath, FileMode.Open, FileAccess.Read))
                            {
                                stream.Position = 0;
                                currentSignature = HashUtility.ComputeHash(stream).ToHexadecimal();
                            }
                        }
                        break;
                    case UriScheme.Ftp:
                        WebRequest request = WebRequest.Create(RequestUri);
                        request.Headers.Add("Listener-Object", listenerHeader);
                        request.Method = CheckResponseData ? WebRequestMethods.Ftp.DownloadFile : WebRequestMethods.Ftp.GetDateTimestamp;
                        using (FtpWebResponse response = NetUtility.GetFtpWebResponse((FtpWebRequest)request))
                        {
                            switch (request.Method)
                            {
                                case WebRequestMethods.Ftp.DownloadFile:
                                    currentSignature = HashUtility.ComputeHash(StreamUtility.CopyStream(response.GetResponseStream())).ToHexadecimal();
                                    break;
                                case WebRequestMethods.Ftp.GetDateTimestamp:
                                    utcLastModified = response.LastModified.ToUniversalTime();
                                    break;
                            }
                        }
                        break;
                    case UriScheme.Http:
                    case UriScheme.Https:
                        HttpWebRequestSettings settings = new HttpWebRequestSettings();
                        settings.Headers.Add("Listener-Object", listenerHeader);
                        using (HttpWebResponse response = CheckResponseData ? NetHttpUtility.HttpGet(RequestUri, settings) : NetHttpUtility.HttpHead(RequestUri, settings))
                        {
                            switch (response.Method)
                            {
                                case WebRequestMethods.Http.Get:
                                    string etag = response.Headers[HttpResponseHeader.ETag];
                                    currentSignature = string.IsNullOrEmpty(etag) ? HashUtility.ComputeHash(StreamUtility.CopyStream(response.GetResponseStream())).ToHexadecimal() : etag;
                                    break;
                                case WebRequestMethods.Http.Head:
                                    utcLastModified = response.LastModified.ToUniversalTime();
                                    break;
                            }
                        }
                        break;
                    default:
                        throw new InvalidOperationException("Only allowed schemes for now is File, FTP, HTTP or HTTPS.");
                }

                if (CheckResponseData)
                {
                    if (Signature == SignatureDefault) { Signature = currentSignature; }
                    if (!Signature.Equals(currentSignature, StringComparison.OrdinalIgnoreCase))
                    {
                        SetUtcLastModified(utcLastModified);
                        OnChangedRaised();
                    }
                    Signature = currentSignature;
                    return;
                }

                if (utcLastModified > UtcCreated)
                {
                    SetUtcLastModified(utcLastModified);
                    OnChangedRaised();
                }
            }
        }
        #endregion
    }
}