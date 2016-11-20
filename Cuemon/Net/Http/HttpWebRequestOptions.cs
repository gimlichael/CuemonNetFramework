using System;
using System.Collections.Generic;
using System.Net;

namespace Cuemon.Net.Http
{

    /// <summary>
    /// Specifies options for the <see cref="HttpWebRequest" /> object. This class cannot be inherited.
    /// </summary>
    public sealed class HttpWebRequestOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpWebRequestOptions"/> class.
        /// </summary>
        public HttpWebRequestOptions() : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpWebRequestOptions"/> class.
        /// </summary>
        /// <param name="headers">Specifies a collection of the name/value pairs that make up the HTTP request headers.</param>
        /// <summary>
        /// Specifies a set of features to support the <see cref="HttpWebRequest"/> object.
        /// </summary>
        /// <returns>A <see cref="HttpWebRequestOptions"/> instance that specifies a set of features to support the <see cref="HttpWebRequest"/> object.</returns>
        /// <remarks>
        /// The following table shows the initial property values for an instance of <see cref="HttpWebRequestOptions"/>.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Property</term>
        ///         <description>Initial Value</description>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="AllowAutoRedirect"/></term>
        ///         <description><c>true</c></description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="AllowWriteStreamBuffering"/></term>
        ///         <description><c>true</c></description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="AutomaticDecompression"/></term>
        ///         <description><see cref="DecompressionMethods.GZip"/> | <see cref="DecompressionMethods.Deflate"/></description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="KeepAlive"/></term>
        ///         <description><c>true</c></description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="MaximumAutomaticRedirections"/></term>
        ///         <description><c>10</c></description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="MaximumResponseHeadersLength"/></term>
        ///         <description><see cref="HttpWebRequest.DefaultMaximumResponseHeadersLength"/></description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Pipelined"/></term>
        ///         <description><c>true</c></description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Proxy"/></term>
        ///         <description><see cref="WebRequest.DefaultWebProxy"/></description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="ReadWriteTimeout"/></term>
        ///         <description><c>5 minutes</c></description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="SendChunked"/></term>
        ///         <description><c>false</c></description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Timeout"/></term>
        ///         <description><see cref="NetHttpUtility.DefaultHttpTimeout"/></description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="CookieJar"/></term>
        ///         <description><c>null</c></description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="RequestCallback"/></term>
        ///         <description><c>null</c></description>
        ///     </item>
        /// </list>
        /// </remarks>
        public HttpWebRequestOptions(WebHeaderCollection headers)
        {
            AllowAutoRedirect = true;
            AllowWriteStreamBuffering = true;
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            Headers = headers ?? new WebHeaderCollection();
            KeepAlive = true;
            MaximumAutomaticRedirections = 10;
            MaximumResponseHeadersLength = HttpWebRequest.DefaultMaximumResponseHeadersLength;
            Pipelined = true;
            Proxy = WebRequest.DefaultWebProxy;
            ReadWriteTimeout = TimeSpan.FromMinutes(5);
            SendChunked = false;
            Timeout = NetHttpUtility.DefaultHttpTimeout;
        }

        /// <summary>
        /// Gets or sets the callback delegate to setup <see cref="HttpWebRequest"/> details otherwise not provided by these <see cref="HttpWebRequestOptions"/>.
        /// </summary>
        /// <value>The callback delegate to setup <see cref="HttpWebRequest"/> details otherwise not provided by these <see cref="HttpWebRequestOptions"/>.</value>
        public Act<HttpWebRequest> RequestCallback { get; set; }

        /// <summary>
        /// Gets or sets the cookies associated with the request.
        /// </summary>
        /// <value>A <see cref="CookieContainer"/> that contains the cookies associated with this request.</value>
        public CookieContainer CookieJar { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether the request should follow redirection responses.
        /// </summary>
        /// <value><c>true</c> if the request should automatically follow redirection responses from the Internet resource; otherwise, <c>false</c>. The default value is true.</value>
        public bool AllowAutoRedirect { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether to buffer the data sent to the Internet resource.
        /// </summary>
        /// <value><c>true</c> to enable buffering of the data sent to the Internet resource; otherwise, <c>false</c>. The default value is true.</value>
        public bool AllowWriteStreamBuffering { get; set; }

        /// <summary>
        /// Gets or sets the type of decompression that is used.
        /// </summary>
        /// <value>The decompression algorithm to use in the server response. Default is <see cref="DecompressionMethods.GZip"/> | <see cref="DecompressionMethods.Deflate"/>.</value>
        public DecompressionMethods AutomaticDecompression { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether to make a persistent connection to the Internet resource.
        /// </summary>
        /// <value><c>true</c> if the request to the Internet resource should contain a Connection HTTP header with the value Keep-Alive; otherwise, <c>false</c>. The default value is <c>true</c>.</value>
        public bool KeepAlive { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of redirects that the request follows.
        /// </summary>
        /// <value>The maximum number of redirection responses that the request follows. The default value is 10.</value>
        public int MaximumAutomaticRedirections { get; set; }

        /// <summary>
        /// Gets or sets the maximum length of the response headers.
        /// </summary>
        /// <value>The length, in kilobytes (1024 bytes), of the response headers.</value>
        public int MaximumResponseHeadersLength { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether to pipeline the request to the Internet resource.
        /// </summary>
        /// <value><c>true</c> if the request should be pipelined; otherwise, <c>false</c>. The default value is <c>true</c>.</value>
        public bool Pipelined { get; set; }

        /// <summary>
        /// Gets or sets proxy information for the request.
        /// </summary>
        /// <value>The IWebProxy object to use to proxy the request. The default value is <see cref="WebRequest.DefaultWebProxy"/>.</value>
        public IWebProxy Proxy { get; set; }

        /// <summary>
        /// Specifies a collection of the name/value pairs that make up the HTTP request headers.
        /// </summary>
        /// <value>A <see cref="WebHeaderCollection"/> that contains the name/value pairs that make up the headers for the HTTP request.</value>
        public WebHeaderCollection Headers { get; set; }

        /// <summary>
        /// Gets or sets a time-out value for when writing to or reading from a stream.
        /// </summary>
        /// <value>The amount of time before the writing or reading times out. The default value is 5 minutes.</value>
        public TimeSpan ReadWriteTimeout { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates whether to send data in segments to the Internet resource.
        /// </summary>
        /// <value><c>true</c> to send data to the Internet resource in segments; otherwise, <c>false</c>. The default value is <c>false</c>.</value>
        public bool SendChunked { get; set; }

        /// <summary>
        /// Gets or sets the time-out value for the <see cref="HttpWebRequest.GetResponse"/> and <see cref="HttpWebRequest.GetRequestStream()"/> methods.
        /// </summary>
        /// <value>The amount of time to wait before the request times out. The default value is <see cref="NetHttpUtility.DefaultHttpTimeout"/>. </value>
        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// Specifies a set of features to support the <see cref="HttpWebRequest"/> object.
        /// </summary>
        /// <param name="headers">A variable number of header/value pairs that make up the HTTP request headers.</param>
        /// <returns>A <see cref="HttpWebRequestOptions"/> instance that specifies a set of features to support the <see cref="HttpWebRequest"/> object.</returns>
        public static HttpWebRequestOptions Create(params KeyValuePair<HttpRequestHeader, string>[] headers)
        {
            return Create(options =>
            {
                options.Headers = ToWebHeaderCollection(headers);
            });
        }

        /// <summary>
        /// Specifies a set of features to support the <see cref="HttpWebRequest"/> object.
        /// </summary>
        /// <param name="setup">The <see cref="HttpWebRequestOptions"/> which need to be configured.</param>
        /// <returns>A <see cref="HttpWebRequestOptions"/> instance that specifies a set of features to support the <see cref="HttpWebRequest"/> object.</returns>
        public static HttpWebRequestOptions Create(Act<HttpWebRequestOptions> setup)
        {
            HttpWebRequestOptions options = DelegateUtility.ConfigureAction(setup);
            if (options.Timeout.TotalMilliseconds > int.MaxValue) { throw new ArgumentOutOfRangeException(nameof(setup), "The Timeout property of the HTTP request options cannot exceed 2,147,483,647 milliseconds."); }
            if (options.ReadWriteTimeout.TotalMilliseconds > int.MaxValue) { throw new ArgumentOutOfRangeException(nameof(setup), "The ReadWriteTimeout property of the HTTP request options cannot exceed 2,147,483,647 milliseconds."); }
            return options;
        }

        private static WebHeaderCollection ToWebHeaderCollection(IEnumerable<KeyValuePair<HttpRequestHeader, string>> headers)
        {
            WebHeaderCollection headerCollection = new WebHeaderCollection();
            if (headers != null)
            {
                foreach (KeyValuePair<HttpRequestHeader, string> header in headers)
                {
                    headerCollection.Add(header.Key, header.Value);
                }
            }
            return headerCollection;
        }
    }
}