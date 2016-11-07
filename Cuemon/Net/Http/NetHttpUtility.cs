using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using Cuemon.Collections.Generic;
using Cuemon.Collections.Specialized;
using Cuemon.IO;
using Cuemon.Threading;

namespace Cuemon.Net.Http
{
    /// <summary>
    /// This utility class is designed to make various HTTP related operations easier to work with.
    /// </summary>
    public static class NetHttpUtility
    {
        private static TimeSpan _httpDefaultTimeout = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Gets or sets the default HTTP timeout value as a <see cref="TimeSpan"/> for <see cref="HttpWebRequest "/> and <see cref="HttpWebResponse"/> related operations. Default value is 30 seconds.
        /// </summary>
        /// <value>The default HTTP timeout value as a <see cref="TimeSpan"/>.</value>
        public static TimeSpan DefaultHttpTimeout
        {
            get { return _httpDefaultTimeout; }
            set
            {
                if (value.TotalMilliseconds > int.MaxValue) { throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Default HTTP timeout value exceeded the allowed maximum value of {0} milliseconds.", int.MaxValue)); }
                _httpDefaultTimeout = value;
            }
        }

        /// <summary>
        /// Creates a <see cref="AsyncCallResult{HttpWebRequest,WebResponse}"/> that encapsulates a pair of <see cref="HttpWebRequest.BeginGetResponse"/> and <see cref="HttpWebRequest.EndGetResponse"/> served like synchronous programming model but with full advantage of the asynchronous programming model pattern.
        /// </summary>
        /// <param name="beginMethod">The delegate that begins the asynchronous operation.</param>
        /// <param name="endMethod">The delegate that ends the asynchronous operation.</param>
        /// <param name="state">An object containing data to be used by the <paramref name="beginMethod"/> delegate.</param>
        /// <returns>The created <see cref="AsyncCallResult{HttpWebRequest,WebResponse}"/> that represents the asynchronous operation.</returns>
        public static AsyncCallResult<HttpWebRequest, WebResponse> WebResponseFromAsync(Doer<AsyncCallback, object, IAsyncResult> beginMethod, Doer<IAsyncResult, WebResponse> endMethod, HttpWebRequest state)
        {
            AsyncCall<HttpWebRequest, WebResponse> result = new AsyncCall<HttpWebRequest, WebResponse>(beginMethod, endMethod, state);
            AsyncCallResult<HttpWebRequest, WebResponse> vexingResult = null;
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(state.Timeout);
                try
                {
                    result.Invoke();
                    result.Wait(timeout);
                }
                catch (TimeoutException)
                {
                    throw new TimeoutException(string.Format(CultureInfo.InvariantCulture, "The HTTP request {0} took longer than the specified timeout value of {1}.", state.RequestUri.OriginalString, timeout));
                }

                vexingResult = result.ToAsyncCallResult();
                if (vexingResult.Exception != null)
                {
                    var exceptions = ExceptionUtility.Flatten(vexingResult.Exception);
                    var vexingException = ExceptionUtility.Parse<WebException>(exceptions);
                    if (vexingException != null && vexingException.Status == WebExceptionStatus.ProtocolError)
                    {
                        vexingResult.Result = vexingException.Response;
                        vexingResult.Exception = null;
                    }
                }
            }
            catch (WebException ex)
            {
                if (ex.Response == null || ex.Status != WebExceptionStatus.ProtocolError) { throw; }
                if (vexingResult == null) { vexingResult = result.ToAsyncCallResult(); }
                vexingResult.Result = ex.Response as HttpWebResponse;
            }
            catch (TimeoutException ex)
            {
                if (state != null) { state.Abort(); }
                vexingResult = new AsyncCallResult<HttpWebRequest, WebResponse>(state, null);
                vexingResult.Exception = ex;
            }
            return vexingResult;
        }

        /// <summary>
        /// Creates and returns an UTF-8 encoded <see cref="string"/> containing the body from the specified <paramref name="response"/>. Default expected <see cref="HttpStatusCode"/> is <see cref="HttpStatusCode.OK"/>. Any preamble sequence is preserved.
        /// </summary>
        /// <param name="response">The <see cref="HttpWebResponse"/> to retrieve the <see cref="string"/> from.</param>
        /// <returns>A <see cref="string"/> containing the body of the response.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="response"/> is null.
        /// </exception>
        /// <exception cref="System.Net.WebException">
        /// The resulting <see cref="HttpWebResponse.StatusCode"/> is different from <see cref="HttpStatusCode.OK"/>.
        /// </exception>
        public static string ResponseAsString(HttpWebResponse response)
        {
            return ResponseAsString(response, HttpStatusCode.OK);
        }

        /// <summary>
        /// Creates and returns an UTF-8 encoded <see cref="string"/> containing the body from the specified <paramref name="response"/>. Any preamble sequence is preserved.
        /// </summary>
        /// <param name="response">The <see cref="HttpWebResponse"/> to retrieve the <see cref="string"/> from.</param>
        /// <param name="expectedStatusCodes">One or more expected status codes in order to return a <see cref="string"/>.</param>
        /// <returns>A <see cref="string"/> containing the body of the response.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="response"/> is null or<br/>
        /// <paramref name="expectedStatusCodes"/> is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="expectedStatusCodes"/> is empty.
        /// </exception>
        /// <exception cref="System.Net.WebException">
        /// The specified <paramref name="expectedStatusCodes"/> did not contain the resulting <see cref="HttpWebResponse.StatusCode"/>.
        /// </exception>
        public static string ResponseAsString(HttpWebResponse response, params HttpStatusCode[] expectedStatusCodes)
        {
            return ResponseAsString(response, PreambleSequence.Keep, expectedStatusCodes);
        }

        /// <summary>
        /// Creates and returns an UTF-8 encoded <see cref="string"/> containing the body from the specified <paramref name="response"/>.
        /// </summary>
        /// <param name="response">The <see cref="HttpWebResponse"/> to retrieve the <see cref="string"/> from.</param>
        /// <param name="sequence">Determines whether too keep or remove any preamble sequences.</param>
        /// <param name="expectedStatusCodes">One or more expected status codes in order to return a <see cref="string"/>.</param>
        /// <returns>A <see cref="string"/> containing the body of the response.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="response"/> is null or<br/>
        /// <paramref name="expectedStatusCodes"/> is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="expectedStatusCodes"/> is empty.
        /// </exception>
        /// <exception cref="System.Net.WebException">
        /// The specified <paramref name="expectedStatusCodes"/> did not contain the resulting <see cref="HttpWebResponse.StatusCode"/>.
        /// </exception>
        public static string ResponseAsString(HttpWebResponse response, PreambleSequence sequence, params HttpStatusCode[] expectedStatusCodes)
        {
            return ResponseAsString(response, sequence, Encoding.UTF8, expectedStatusCodes);
        }

        /// <summary>
        /// Creates and returns a <see cref="string"/> containing the body from the specified <paramref name="response"/>.
        /// </summary>
        /// <param name="response">The <see cref="HttpWebResponse"/> to retrieve the <see cref="string"/> from.</param>
        /// <param name="sequence">Determines whether too keep or remove any preamble sequences.</param>
        /// <param name="encoding">The preferred encoding to apply to the result.</param>
        /// <param name="expectedStatusCodes">One or more expected status codes in order to return a <see cref="string"/>.</param>
        /// <returns>A <see cref="string"/> containing the body of the response.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="response"/> is null or<br/>
        /// <paramref name="expectedStatusCodes"/> is null.
        /// </exception>
        /// <exception cref="Cuemon.ArgumentEmptyException">
        /// <paramref name="expectedStatusCodes"/> is empty.
        /// </exception>
        /// <exception cref="System.Net.WebException">
        /// The specified <paramref name="expectedStatusCodes"/> did not contain the resulting <see cref="HttpWebResponse.StatusCode"/>.
        /// </exception>
        public static string ResponseAsString(HttpWebResponse response, PreambleSequence sequence, Encoding encoding, params HttpStatusCode[] expectedStatusCodes)
        {
            if (response == null) { throw new ArgumentNullException(nameof(response)); }
            if (encoding == null) { throw new ArgumentNullException(nameof(encoding)); }
            if (expectedStatusCodes == null) { throw new ArgumentNullException(nameof(expectedStatusCodes)); }
            if (expectedStatusCodes.Length == 0) { throw new ArgumentEmptyException(nameof(expectedStatusCodes), "You must specify at least one status code to accept as a valid response."); }
            return StringConverter.FromStream(ResponseAsStream(response, expectedStatusCodes), sequence, encoding);
        }

        /// <summary>
        /// Creates and returns a byte array containing the body from the specified <paramref name="response"/>. Default expected <see cref="HttpStatusCode"/> is <see cref="HttpStatusCode.OK"/>.
        /// </summary>
        /// <param name="response">The <see cref="HttpWebResponse"/> to retrieve the byte array from.</param>
        /// <returns>A byte array containing the body of the response.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="response"/> is null.
        /// </exception>
        /// <exception cref="System.Net.WebException">
        /// The resulting <see cref="HttpWebResponse.StatusCode"/> is different from <see cref="HttpStatusCode.OK"/>.
        /// </exception>
        public static byte[] ResponseAsByteArray(HttpWebResponse response)
        {
            return ResponseAsByteArray(response, HttpStatusCode.OK);
        }

        /// <summary>
        /// Creates and returns a byte array containing the body from the specified <paramref name="response"/>.
        /// </summary>
        /// <param name="response">The <see cref="HttpWebResponse"/> to retrieve the byte array from.</param>
        /// <param name="expectedStatusCodes">One or more expected status codes in order to return a byte array.</param>
        /// <returns>A byte array containing the body of the response.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="response"/> is null or<br/>
        /// <paramref name="expectedStatusCodes"/> is null.
        /// </exception>
        /// <exception cref="Cuemon.ArgumentEmptyException">
        /// <paramref name="expectedStatusCodes"/> is empty.
        /// </exception>
        /// <exception cref="System.Net.WebException">
        /// The specified <paramref name="expectedStatusCodes"/> did not contain the resulting <see cref="HttpWebResponse.StatusCode"/>.
        /// </exception>
        public static byte[] ResponseAsByteArray(HttpWebResponse response, params HttpStatusCode[] expectedStatusCodes)
        {
            if (response == null) { throw new ArgumentNullException(nameof(response)); }
            if (expectedStatusCodes == null) { throw new ArgumentNullException(nameof(expectedStatusCodes)); }
            if (expectedStatusCodes.Length == 0) { throw new ArgumentEmptyException(nameof(expectedStatusCodes), "You must specify at least one status code to accept as a valid response."); }
            return ByteConverter.FromStream(ResponseAsStream(response, expectedStatusCodes));
        }

        /// <summary>
        /// Creates and returns a seekable <see cref="Stream"/> containing the body from the specified <paramref name="response"/>. Default expected <see cref="HttpStatusCode"/> is <see cref="HttpStatusCode.OK"/>.
        /// </summary>
        /// <param name="response">The <see cref="HttpWebResponse"/> to retrieve the <see cref="Stream"/> from.</param>
        /// <returns>A seekable <see cref="Stream"/> containing the body of the response.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="response"/> is null.
        /// </exception>
        /// <exception cref="System.Net.WebException">
        /// The resulting <see cref="HttpWebResponse.StatusCode"/> is different from <see cref="HttpStatusCode.OK"/>.
        /// </exception>
        public static Stream ResponseAsStream(HttpWebResponse response)
        {
            return ResponseAsStream(response, HttpStatusCode.OK);
        }

        /// <summary>
        /// Creates and returns a seekable <see cref="Stream"/> containing the body from the specified <paramref name="response"/>.
        /// </summary>
        /// <param name="response">The <see cref="HttpWebResponse"/> to retrieve the <see cref="Stream"/> from.</param>
        /// <param name="expectedStatusCodes">One or more expected status codes in order to return a seekable response <see cref="Stream"/>.</param>
        /// <returns>A seekable <see cref="Stream"/> containing the body of the response.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="response"/> is null or<br/>
        /// <paramref name="expectedStatusCodes"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="expectedStatusCodes"/> is empty.
        /// </exception>
        /// <exception cref="System.Net.WebException">
        /// The specified <paramref name="expectedStatusCodes"/> did not contain the resulting <see cref="HttpWebResponse.StatusCode"/>.
        /// </exception>
        public static Stream ResponseAsStream(HttpWebResponse response, params HttpStatusCode[] expectedStatusCodes)
        {
            if (response == null) { throw new ArgumentNullException(nameof(response)); }
            if (expectedStatusCodes == null) { throw new ArgumentNullException(nameof(expectedStatusCodes)); }
            if (expectedStatusCodes.Length == 0) { throw new ArgumentOutOfRangeException(nameof(expectedStatusCodes), "You must specify at least one status code to accept as a valid response."); }

            using (response)
            {
                if (EnumerableUtility.Contains(expectedStatusCodes, response.StatusCode))
                {
                    return StreamUtility.CopyStream(response.GetResponseStream());
                }
                throw new WebException(string.Format(CultureInfo.InvariantCulture, "Unexpected status code returned from the response: '{0}'. Expected range of status codes was: {1}.", response.StatusCode, StringConverter.ToDelimitedString(expectedStatusCodes, ",", " '{0}'")), WebExceptionStatus.ProtocolError);
            }
        }

        /// <summary>
        /// Creates and returns a <see cref="HttpWebResponse"/> from the specified Internet resource <paramref name="request"/>.
        /// </summary>
        /// <param name="request">An instance of <see cref="HttpWebRequest"/>.</param>
        /// <returns>Returns a HTTP-specific response from an Internet resource.</returns>
        public static HttpWebResponse Http(HttpWebRequest request)
        {
            if (request == null) { throw new ArgumentNullException(nameof(request)); }
            return HttpCore(request);
        }

        /// <summary>
        /// Creates and returns a <see cref="HttpWebResponse"/> from the specified HTTP <paramref name="method"/> and URI <paramref name="location"/>.
        /// </summary>
        /// <param name="method">The request method to use to contact the Internet resource.</param>
        /// <param name="location">The URI to retrieve a <see cref="HttpWebResponse"/> from.</param>
        /// <returns>Returns a HTTP-specific response from an Internet resource.</returns>
        public static HttpWebResponse Http(string method, Uri location)
        {
            if (method == null) { throw new ArgumentNullException(nameof(method)); }
            if (method.Length == 0) { throw new ArgumentEmptyException(nameof(method)); }
            if (location == null) { throw new ArgumentNullException(nameof(location)); }
            return HttpCore(location, method, null, HttpWebRequestSettings.CreateDefault());
        }

        /// <summary>
        /// Creates and returns a <see cref="HttpWebResponse"/> from the specified HTTP <paramref name="method"/> and URI <paramref name="location"/>.
        /// </summary>
        /// <param name="method">The request method to use to contact the Internet resource.</param>
        /// <param name="location">The URI to retrieve a <see cref="HttpWebResponse"/> from.</param>
        /// <param name="settings">The settings that will be applied doing the HTTP request.</param>
        /// <returns>Returns a HTTP-specific response from an Internet resource.</returns>
        public static HttpWebResponse Http(string method, Uri location, HttpWebRequestSettings settings)
        {
            if (method == null) { throw new ArgumentNullException(nameof(method)); }
            if (method.Length == 0) { throw new ArgumentEmptyException(nameof(method)); }
            if (location == null) { throw new ArgumentNullException(nameof(location)); }
            if (settings == null) { throw new ArgumentNullException(nameof(settings)); }
            return HttpCore(location, method, null, settings);
        }

        /// <summary>
        /// Creates and returns a <see cref="HttpWebResponse"/> from the specified HTTP <paramref name="method"/> and URI <paramref name="location"/>.
        /// </summary>
        /// <param name="method">The request method to use to contact the Internet resource.</param>
        /// <param name="location">The URI to retrieve a <see cref="HttpWebResponse"/> from.</param>
        /// <param name="settings">The settings that will be applied doing the HTTP request.</param>
        /// <param name="content">The content value of the HTTP request body.</param>
        /// <returns>Returns a HTTP-specific response from an Internet resource.</returns>
        public static HttpWebResponse Http(string method, Uri location, HttpWebRequestSettings settings, Stream content)
        {
            if (method == null) { throw new ArgumentNullException(nameof(method)); }
            if (method.Length == 0) { throw new ArgumentEmptyException(nameof(method)); }
            if (location == null) { throw new ArgumentNullException(nameof(location)); }
            if (settings == null) { throw new ArgumentNullException(nameof(settings)); }
            if (content == null) { throw new ArgumentNullException(nameof(content)); }
            return HttpCore(location, method, content, settings);
        }

        /// <summary>
        /// Creates and returns a <see cref="HttpWebResponse"/> from the specified HTTP <paramref name="method"/> and URI <paramref name="location"/>.
        /// </summary>
        /// <param name="method">The request method to use to contact the Internet resource.</param>
        /// <param name="location">The URI to retrieve a <see cref="HttpWebResponse"/> from.</param>
        /// <param name="contentType">The value of the Content-Type HTTP header.</param>
        /// <param name="content">The content value of the HTTP request body.</param>
        /// <returns>Returns a HTTP-specific response from an Internet resource.</returns>
        public static HttpWebResponse Http(string method, Uri location, string contentType, Stream content)
        {
            if (method == null) { throw new ArgumentNullException(nameof(method)); }
            if (method.Length == 0) { throw new ArgumentEmptyException(nameof(method)); }
            if (location == null) { throw new ArgumentNullException(nameof(location)); }
            if (content == null) { throw new ArgumentNullException(nameof(content)); }
            if (contentType == null) { throw new ArgumentNullException(nameof(contentType)); }
            if (contentType.Length == 0) { throw new ArgumentEmptyException(nameof(contentType)); }
            HttpWebRequestSettings settings = HttpWebRequestSettings.CreateDefault();
            settings.Headers.Add(HttpRequestHeader.ContentType, contentType);
            return HttpCore(location, method, content, settings);
        }


        private static void ParseRequestHeaders(HttpWebRequest request, WebHeaderCollection headers)
        {
            foreach (string header in headers.AllKeys)
            {
                try
                {
                    if (string.IsNullOrEmpty(header)) { continue; }
                    if (header.Trim().Length == 0) { continue; }
                    HttpRequestHeader requestHeader = (HttpRequestHeader)Enum.Parse(typeof(HttpRequestHeader), header.Replace("-", ""), true);
                    switch (requestHeader)
                    {
                        case HttpRequestHeader.Accept:
                            request.Accept = headers[requestHeader];
                            break;
                        case HttpRequestHeader.Connection:
                            request.Connection = headers[requestHeader];
                            break;
                        case HttpRequestHeader.ContentLength:
                            request.ContentLength = long.Parse(headers[requestHeader], CultureInfo.InvariantCulture);
                            break;
                        case HttpRequestHeader.ContentType:
                            request.ContentType = headers[requestHeader];
                            break;
                        case HttpRequestHeader.Expect:
                            request.Expect = headers[requestHeader];
                            break;
                        case HttpRequestHeader.Date:
                        case HttpRequestHeader.Host:
                        case HttpRequestHeader.Range:
                            break;
                        case HttpRequestHeader.IfModifiedSince:
                            request.IfModifiedSince = DateTime.Parse(headers[requestHeader], CultureInfo.InvariantCulture);
                            break;
                        case HttpRequestHeader.Referer:
                            request.Referer = headers[requestHeader];
                            break;
                        case HttpRequestHeader.TransferEncoding:
                            request.TransferEncoding = headers[requestHeader];
                            break;
                        case HttpRequestHeader.UserAgent:
                            request.UserAgent = headers[requestHeader];
                            break;
                        default:
                            request.Headers.Add(requestHeader, headers[requestHeader]);
                            break;
                    }
                }
                catch (ArgumentException)
                {
                    request.Headers.Add(header, headers[header]);
                }
            }
        }

        internal static HttpWebRequest CreateRequest(Uri location, string method, HttpWebRequestSettings settings)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(location);
            request.Method = method.ToUpperInvariant();
            request.AllowAutoRedirect = settings.AllowAutoRedirect;
            request.AllowWriteStreamBuffering = settings.AllowWriteStreamBuffering;
            request.AutomaticDecompression = settings.AutomaticDecompression;
            request.KeepAlive = settings.KeepAlive;
            request.MaximumAutomaticRedirections = settings.MaximumAutomaticRedirections;
            request.MaximumResponseHeadersLength = settings.MaximumResponseHeadersLength;
            request.Pipelined = settings.Pipelined;
            request.Proxy = settings.Proxy;
            request.ReadWriteTimeout = (int)settings.ReadWriteTimeout.TotalMilliseconds;
            request.SendChunked = settings.SendChunked;
            request.Timeout = (int)settings.Timeout.TotalMilliseconds;
            ParseRequestHeaders(request, settings.Headers);
            return request;
        }

        private static HttpWebResponse HttpCore(Uri location, string method, Stream content, HttpWebRequestSettings settings)
        {
            if (method == null) { throw new ArgumentNullException(nameof(method)); }
            if (method.Length == 0) { throw new ArgumentEmptyException(nameof(method)); }
            if (location == null) { throw new ArgumentNullException(nameof(location)); }
            if (settings == null) { throw new ArgumentNullException(nameof(settings)); }

            var request = CreateRequest(location, method, settings);
            if (content != null)
            {
                request.ContentLength = content.Length;
                Stream requestBody = request.GetRequestStream();
                StreamUtility.CopyStream(content, requestBody);
            }

            return HttpCore(request);
        }

        private static HttpWebResponse HttpCore(HttpWebRequest request)
        {
            AsyncCallResult<HttpWebRequest, WebResponse> asyncResponse = WebResponseFromAsync(request.BeginGetResponse, request.EndGetResponse, request);
            if (asyncResponse.Exception != null) { throw new ArgumentException("An error occurred while invoking the asynchronous operation.", nameof(request), asyncResponse.Exception); }
            return asyncResponse.ResultAs<HttpWebResponse>();
        }

        /// <summary>
        /// Creates and returns a <see cref="HttpWebResponse"/> from the specified URI <paramref name="location"/>.
        /// </summary>
        /// <param name="location">The URI to retrieve a <see cref="HttpWebResponse"/> from.</param>
        /// <returns>Returns a <see cref="HttpMethods.Options"/> response from an Internet resource.</returns>
        public static HttpWebResponse HttpOptions(Uri location)
        {
            return HttpOptions(location, HttpWebRequestSettings.CreateDefault());
        }

        /// <summary>
        /// Creates and returns a <see cref="HttpWebResponse"/> from the specified URI <paramref name="location"/>.
        /// </summary>
        /// <param name="location">The URI to retrieve a <see cref="HttpWebResponse"/> from.</param>
        /// <param name="settings">The settings that will be applied doing the HTTP request.</param>
        /// <returns>Returns a <see cref="HttpMethods.Options"/> response from an Internet resource.</returns>
        public static HttpWebResponse HttpOptions(Uri location, HttpWebRequestSettings settings)
        {
            return Http(HttpMethods.Options.ToString(), location, settings);
        }

        /// <summary>
        /// Creates and returns a <see cref="HttpWebResponse"/> from the specified URI <paramref name="location"/>.
        /// </summary>
        /// <param name="location">The URI to retrieve a <see cref="HttpWebResponse"/> from.</param>
        /// <returns>Returns a <see cref="HttpMethods.Delete"/> response from an Internet resource.</returns>
        public static HttpWebResponse HttpDelete(Uri location)
        {
            return HttpDelete(location, HttpWebRequestSettings.CreateDefault());
        }

        /// <summary>
        /// Creates and returns a <see cref="HttpWebResponse"/> from the specified URI <paramref name="location"/>.
        /// </summary>
        /// <param name="location">The URI to retrieve a <see cref="HttpWebResponse"/> from.</param>
        /// <param name="settings">The settings that will be applied doing the HTTP request.</param>
        /// <returns>Returns a <see cref="HttpMethods.Delete"/> response from an Internet resource.</returns>
        public static HttpWebResponse HttpDelete(Uri location, HttpWebRequestSettings settings)
        {
            return Http(HttpMethods.Delete.ToString(), location, settings);
        }

        /// <summary>
        /// Creates and returns a <see cref="HttpWebResponse"/> from the specified URI <paramref name="location"/>.
        /// </summary>
        /// <param name="location">The URI to retrieve a <see cref="HttpWebResponse"/> from.</param>
        /// <returns>Returns a <see cref="HttpMethods.Trace"/> response from an Internet resource.</returns>
        public static HttpWebResponse HttpTrace(Uri location)
        {
            return HttpTrace(location, HttpWebRequestSettings.CreateDefault());
        }

        /// <summary>
        /// Creates and returns a <see cref="HttpWebResponse"/> from the specified URI <paramref name="location"/>.
        /// </summary>
        /// <param name="location">The URI to retrieve a <see cref="HttpWebResponse"/> from.</param>
        /// <param name="settings">The settings that will be applied doing the HTTP request.</param>
        /// <returns>Returns a <see cref="HttpMethods.Trace"/> response from an Internet resource.</returns>
        public static HttpWebResponse HttpTrace(Uri location, HttpWebRequestSettings settings)
        {
            return Http(HttpMethods.Trace.ToString(), location, settings);
        }

        /// <summary>
        /// Creates and returns a <see cref="HttpWebResponse"/> from the specified URI <paramref name="location"/>.
        /// </summary>
        /// <param name="location">The URI to retrieve a <see cref="HttpWebResponse"/> from.</param>
        /// <returns>Returns a <see cref="HttpMethods.Head"/> response from an Internet resource.</returns>
        public static HttpWebResponse HttpHead(Uri location)
        {
            return HttpHead(location, HttpWebRequestSettings.CreateDefault());
        }

        /// <summary>
        /// Creates and returns a <see cref="HttpWebResponse"/> from the specified URI <paramref name="location"/>.
        /// </summary>
        /// <param name="location">The URI to retrieve a <see cref="HttpWebResponse"/> from.</param>
        /// <param name="settings">The settings that will be applied doing the HTTP request.</param>
        /// <returns>Returns a <see cref="HttpMethods.Head"/> response from an Internet resource.</returns>
        public static HttpWebResponse HttpHead(Uri location, HttpWebRequestSettings settings)
        {
            return Http(HttpMethods.Head.ToString(), location, settings);
        }

        /// <summary>
        /// Creates and returns a <see cref="HttpWebResponse"/> from the specified URI <paramref name="location"/>.
        /// </summary>
        /// <param name="location">The URI to retrieve a <see cref="HttpWebResponse"/> from.</param>
        /// <returns>Returns a <see cref="HttpMethods.Get"/> response from an Internet resource.</returns>
        public static HttpWebResponse HttpGet(Uri location)
        {
            return HttpGet(location, HttpWebRequestSettings.CreateDefault());
        }

        /// <summary>
        /// Creates and returns a <see cref="HttpWebResponse"/> from the specified URI <paramref name="location"/>.
        /// </summary>
        /// <param name="location">The URI to retrieve a <see cref="HttpWebResponse"/> from.</param>
        /// <param name="settings">The settings that will be applied doing the HTTP request.</param>
        /// <returns>Returns a <see cref="HttpMethods.Get"/> response from an Internet resource.</returns>
        public static HttpWebResponse HttpGet(Uri location, HttpWebRequestSettings settings)
        {
            return Http(HttpMethods.Get.ToString(), location, settings);
        }

        /// <summary>
        /// Creates and returns a <see cref="HttpWebResponse"/> from the specified URI <paramref name="location"/>.
        /// </summary>
        /// <param name="location">The URI to retrieve a <see cref="HttpWebResponse"/> from.</param>
        /// <param name="contentType">The value of the Content-Type HTTP header.</param>
        /// <param name="content">The content value of the HTTP request body.</param>
        /// <returns>Returns a <see cref="HttpMethods.Put"/> response from an Internet resource.</returns>
        public static HttpWebResponse HttpPut(Uri location, string contentType, Stream content)
        {
            return Http(HttpMethods.Put.ToString(), location, contentType, content);
        }

        /// <summary>
        /// Creates and returns a <see cref="HttpWebResponse"/> from the specified URI <paramref name="location"/>.
        /// </summary>
        /// <param name="location">The URI to retrieve a <see cref="HttpWebResponse"/> from.</param>
        /// <param name="settings">The settings that will be applied doing the HTTP request.</param>
        /// <param name="content">The content value of the HTTP request body.</param>
        /// <returns>Returns a <see cref="HttpMethods.Put"/> response from an Internet resource.</returns>
        public static HttpWebResponse HttpPut(Uri location, HttpWebRequestSettings settings, Stream content)
        {
            return Http(HttpMethods.Put.ToString(), location, settings, content);
        }

        /// <summary>
        /// Creates and returns a <see cref="HttpWebResponse"/> from the specified URI <paramref name="location"/>.
        /// </summary>
        /// <param name="location">The URI to retrieve a <see cref="HttpWebResponse"/> from.</param>
        /// <param name="contentType">The value of the Content-Type HTTP header.</param>
        /// <param name="content">The content value of the HTTP request body.</param>
        /// <returns>Returns a <see cref="HttpMethods.Post"/> response from an Internet resource.</returns>
        public static HttpWebResponse HttpPost(Uri location, string contentType, Stream content)
        {
            return Http(HttpMethods.Post.ToString(), location, contentType, content);
        }

        /// <summary>
        /// Creates and returns a <see cref="HttpWebResponse"/> from the specified URI <paramref name="location"/>.
        /// </summary>
        /// <param name="location">The URI to retrieve a <see cref="HttpWebResponse"/> from.</param>
        /// <param name="settings">The settings that will be applied doing the HTTP request.</param>
        /// <param name="content">The content value of the HTTP request body.</param>
        /// <returns>Returns a <see cref="HttpMethods.Post"/> response from an Internet resource.</returns>
        public static HttpWebResponse HttpPost(Uri location, HttpWebRequestSettings settings, Stream content)
        {
            return Http(HttpMethods.Post.ToString(), location, settings, content);
        }

        /// <summary>
        /// Determines whether the specified <paramref name="headers"/> contains an entry with the specified <paramref name="header"/>.
        /// </summary>
        /// <param name="headers">The <see cref="WebHeaderCollection"/> to search for <paramref name="header"/>.</param>
        /// <param name="header">The header to locate in <paramref name="headers"/>.</param>
        /// <returns><c>true</c> if the specified <paramref name="headers"/> contains an entry with the <paramref name="header"/>; otherwise, <c>false</c>.</returns>
        public static bool ContainsHeader(WebHeaderCollection headers, HttpRequestHeader header)
        {
            return ContainsHeader(headers, header.ToString());
        }

        /// <summary>
        /// Determines whether the specified <paramref name="headers"/> contains an entry with the specified <paramref name="header"/>.
        /// </summary>
        /// <param name="headers">The <see cref="WebHeaderCollection"/> to search for <paramref name="header"/>.</param>
        /// <param name="header">The header to locate in <paramref name="headers"/>.</param>
        /// <returns><c>true</c> if the specified <paramref name="headers"/> contains an entry with the <paramref name="header"/>; otherwise, <c>false</c>.</returns>
        public static bool ContainsHeader(WebHeaderCollection headers, HttpResponseHeader header)
        {
            return ContainsHeader(headers, header.ToString());
        }

        /// <summary>
        /// Determines whether the specified <paramref name="headers"/> contains an entry with the specified <paramref name="header"/>.
        /// </summary>
        /// <param name="headers">The <see cref="WebHeaderCollection"/> to search for <paramref name="header"/>.</param>
        /// <param name="header">The header to locate in <paramref name="headers"/>.</param>
        /// <returns><c>true</c> if the specified <paramref name="headers"/> contains an entry with the <paramref name="header"/>; otherwise, <c>false</c>.</returns>
        public static bool ContainsHeader(WebHeaderCollection headers, string header)
        {
            return NameValueCollectionUtility.ContainsKey(headers, header);
        }
    }
}