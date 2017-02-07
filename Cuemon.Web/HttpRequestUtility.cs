using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Cuemon.Integrity;
using Cuemon.Collections.Generic;
using Cuemon.Net;

namespace Cuemon.Web
{
    /// <summary>
    /// This utility class is designed to make <see cref="HttpRequest"/> operations easier to work with.
    /// </summary>
    public static class HttpRequestUtility
    {
        /// <summary>
        /// Gets the raw URL of the current <paramref name="request"/>.
        /// </summary>
        /// <param name="request">An instance of the <see cref="HttpRequest"/> object.</param>
        /// <returns>The raw URL of the specified <paramref name="request"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="request"/> is null.
        /// </exception>
        /// <remarks>For reasons unknown ASP.NET/IIS make changes to the <see cref="HttpRequest.Url"/> property. This method will correct this by combining the <see cref="HttpRequest.RawUrl"/> property with the former mentioned property.</remarks>
	    public static Uri RawUrl(HttpRequest request)
        {
            if (request == null) { throw new ArgumentNullException(nameof(request)); }
            return new Uri(request.Url, request.RawUrl);
        }

        /// <summary>
        /// Determines whether the specified <paramref name="request"/> is solely executed by the server local.
        /// </summary>
        /// <param name="request">An instance of <see cref="HttpRequest"/> object.</param>
        /// <returns><c>true</c> if the specified <paramref name="request"/> is solely executed by the server local; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This is a qualified assumption; if the following condition are meet, this method returns <c>true</c>:
        /// <list type="table">
        ///     <listheader>
        ///         <term>Condition</term>
        ///         <description>Value</description>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="HttpRequest.IsLocal"/></term>
        ///         <description><c>true</c></description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="HttpRequest.UserAgent"/></term>
        ///         <description><c>null</c></description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="HttpRequest.UserLanguages"/></term>
        ///         <description><c>null</c></description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="HttpRequest.UrlReferrer"/></term>
        ///         <description><c>null</c></description>
        ///     </item>
        /// </list>
        /// </remarks>
        public static bool IsServerLocal(HttpRequest request)
        {
            if (request == null) { throw new ArgumentNullException(nameof(request)); }
            return (request.IsLocal && (request.UserAgent == null && request.UserLanguages == null && request.UrlReferrer == null));
        }

        internal static bool IsStandaloneServerLocal(HttpRequest request)
        {
            return (IsServerLocal(request) && GlobalModule.IsStandaloneWebsite);
        }

        /// <summary>
        /// Gets the host authority information from the current <see cref="HttpContext"/> as an <see cref="Uri"/>.
        /// </summary>
        /// <returns>The host authority information from the current <see cref="HttpContext"/> as an <see cref="Uri"/>.</returns>
        /// <remarks>For more information about host authority information please have a look at this URI scheme page @ Wikipedia: http://en.wikipedia.org/wiki/URI_scheme</remarks>
        public static Uri GetHostAuthority()
        {
            GlobalModule.CheckForHttpContextAvailability();
            return GetHostAuthority(HttpContext.Current.Request);
        }

        /// <summary>
        /// Gets the host authority information from the specified <paramref name="request"/> as an <see cref="Uri"/>.
        /// </summary>
        /// <param name="request">An instance of <see cref="HttpRequest"/> object.</param>
        /// <returns>The host authority information from the specified <paramref name="request"/> as an <see cref="Uri"/>.</returns>
        /// <remarks>For more information about host authority information please have a look at this URI scheme page @ Wikipedia: http://en.wikipedia.org/wiki/URI_scheme</remarks>
        public static Uri GetHostAuthority(HttpRequest request)
        {
            if (request == null) { throw new ArgumentNullException(nameof(request)); }
            return GetHostAuthority(request.Url);
        }

        /// <summary>
        /// Gets the host authority information from the specified <paramref name="uri"/> as an <see cref="Uri"/>.
        /// </summary>
        /// <param name="uri">An instance of <see cref="Uri"/> to get the host authority information from.</param>
        /// <returns>The host authority information from the specified <paramref name="uri"/> as an <see cref="Uri"/>.</returns>
        /// <remarks>
        /// For more information about host authority information please have a look at this URI scheme page @ Wikipedia: http://en.wikipedia.org/wiki/URI_scheme
        /// </remarks>
        public static Uri GetHostAuthority(Uri uri)
        {
            if (uri == null) { throw new ArgumentNullException(nameof(uri)); }
            return new Uri(string.Format(CultureInfo.InvariantCulture, "{0}/", uri.GetLeftPart(UriPartial.Authority)));
        }

        /// <summary>
        /// Resolves the string representation of an Accept header and converts the values to an <see cref="IEnumerable{ContentType}"/> equivalent using the current <see cref="HttpContext"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">This exception is thrown if HttpContext is unavailable.</exception>
        /// <returns>A sequence of <see cref="ContentType"/> values equivalent to the values contained in the Accept header, if the resolvement succeeded; otherwise defaults to */* value.</returns>
        public static IEnumerable<ContentType> GetAcceptHeader()
        {
            GlobalModule.CheckForHttpContextAvailability();
            return GetAcceptHeader(HttpContext.Current.Request);
        }

        /// <summary>
        /// Resolves the string representation of an Accept header and converts the values to an <see cref="IEnumerable{ContentType}"/> equivalent using the specified <paramref name="request"/> object.
        /// </summary>
        /// <param name="request">An instance of the <see cref="HttpRequest"/> object.</param>
        /// <returns>A sequence of <see cref="ContentType"/> values equivalent to the values contained in the Accept header, if the resolvement succeeded; otherwise defaults to */* value.</returns>
        public static IEnumerable<ContentType> GetAcceptHeader(HttpRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            string acceptHeader = request.Headers["Accept"];
            if (!string.IsNullOrEmpty(acceptHeader))
            {
                SortedDictionary<double, string> acceptResults = new SortedDictionary<double, string>(); // *) lowest first - highest last
                double priority = 20;
                string[] acceptOptions = acceptHeader.Split(',');
                foreach (string acceptOption in acceptOptions)
                {
                    double currentPriority = priority;
                    string[] accept = acceptOption.Split(';');
                    string currentAccept = accept[0].Trim();
                    if (accept.Length > 1)
                    {
                        Regex regex = new Regex(@"\d.\d|\d");
                        Match parsedPriority = regex.Match(accept[1]);
                        if (!string.IsNullOrEmpty(parsedPriority.Value))
                        {
                            currentPriority = Convert.ToDouble(parsedPriority.Value, CultureInfo.InvariantCulture);
                        }
                    }
                    acceptResults.Add(currentPriority, currentAccept);
                    priority--;
                }

                foreach (KeyValuePair<double, string> acceptResult in EnumerableUtility.Reverse(acceptResults))
                {
                    yield return new ContentType(acceptResult.Value); // reverse the list, so highest values are first, and lowest last (prioritized)
                }
            }
            else
            {
                yield return new ContentType("*/*");
            }
        }

        /// <summary>
        /// Resolves the string representation of a Content-Type header and converts the values to an <see cref="ContentType"/> equivalent using the current <see cref="HttpContext"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">This exception is thrown if HttpContext is unavailable.</exception>
        /// <returns>A <see cref="ContentType"/> object equivalent to the value contained in the Content-Type header, if the resolvement succeeded; otherwise null.</returns>
        public static ContentType GetContentTypeHeader()
        {
            GlobalModule.CheckForHttpContextAvailability();
            return GetContentTypeHeader(HttpContext.Current.Request);
        }

        /// <summary>
        /// Resolves the string representation of a Content-Type header and converts the values to an <see cref="ContentType"/> equivalent using the specified <paramref name="request"/> object.
        /// </summary>
        /// <param name="request">An instance of the <see cref="HttpRequest"/> object.</param>
        /// <returns>A <see cref="ContentType"/> object equivalent to the value contained in the Content-Type header, if the resolvement succeeded; otherwise null.</returns>
        public static ContentType GetContentTypeHeader(HttpRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            string contentTypeHeader = request.Headers["Content-Type"];
            return !string.IsNullOrEmpty(contentTypeHeader) ? new ContentType(contentTypeHeader) : null;
        }

        /// <summary>
        /// Resolves the string representation of an Accept-Encoding header and converts the values to an <see cref="IEnumerable{CompressionMethodScheme}"/> equivalent using the current <see cref="HttpContext"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">This exception is thrown if HttpContext is unavailable.</exception>
        /// <returns>A sequence of <see cref="CompressionMethodScheme"/> values equivalent to the values contained in the Accept-Encoding header, if the resolvement succeeded; otherwise defaults to <see cref="CompressionMethodScheme.Identity"/> value.</returns>
        public static IEnumerable<CompressionMethodScheme> GetAcceptEncodingHeader()
        {
            GlobalModule.CheckForHttpContextAvailability();
            return GetAcceptEncodingHeader(HttpContext.Current.Request);
        }

        /// <summary>
        /// Resolves the string representation of an Accept-Encoding header and converts the values to an <see cref="IEnumerable{CompressionMethodScheme}"/> equivalent using the specified <paramref name="request"/> object.
        /// </summary>
        /// <param name="request">An instance of the <see cref="HttpRequest"/> object.</param>
        /// <returns>A sequence of <see cref="CompressionMethodScheme"/> values equivalent to the values contained in the Accept-Encoding header, if the resolvement succeeded; otherwise defaults to <see cref="CompressionMethodScheme.Identity"/> value.</returns>
        public static IEnumerable<CompressionMethodScheme> GetAcceptEncodingHeader(HttpRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            string acceptEncodingHeader = request.Headers["Accept-Encoding"];
            if (!string.IsNullOrEmpty(acceptEncodingHeader))
            {
                IEnumerable<KeyValuePair<double, CompressionMethodScheme>> compressionSchemes = new SortedDictionary<double, CompressionMethodScheme>(); // *) lowest first - highest last
                double priority = 20;
                string[] acceptEncodingOptions = acceptEncodingHeader.Split(',');
                foreach (string acceptEncodingOption in acceptEncodingOptions)
                {
                    double currentPriority = priority;
                    string[] acceptEncoding = acceptEncodingOption.Split(';');
                    CompressionMethodScheme currentCompressionScheme = ParseAcceptEncoding(acceptEncoding[0]);
                    if (acceptEncoding.Length > 1)
                    {
                        Regex regex = new Regex(@"\d.\d|\d");
                        Match parsedPriority = regex.Match(acceptEncoding[1]);
                        if (!string.IsNullOrEmpty(parsedPriority.Value))
                        {
                            currentPriority = Convert.ToDouble(parsedPriority.Value, CultureInfo.InvariantCulture);
                            if (currentPriority == 0)
                            {
                                currentCompressionScheme = acceptEncoding[0].Trim() == "*" ? CompressionMethodScheme.Identity : CompressionMethodScheme.None;
                            }
                        }
                    }
                    if (!((SortedDictionary<double, CompressionMethodScheme>)compressionSchemes).ContainsKey(currentPriority)) { ((SortedDictionary<double, CompressionMethodScheme>)compressionSchemes).Add(currentPriority, currentCompressionScheme); }
                    priority--;
                }
                compressionSchemes = EnumerableUtility.Reverse(compressionSchemes);
                foreach (KeyValuePair<double, CompressionMethodScheme> compressionScheme in compressionSchemes)
                {
                    yield return compressionScheme.Value; // reverse the list, so highest values are first, and lowest last (prioritized)
                }
            }
            else
            {
                yield return CompressionMethodScheme.Identity;
            }
        }

        /// <summary>
        /// This method will parse a sequence of <see cref="CompressionMethodScheme"/> and return the most appropriate <see cref="CompressionMethodScheme"/>.
        /// </summary>
        /// <returns>The most appropriate <see cref="CompressionMethodScheme"/> from the by <see cref="HttpRequest"/> resolved HTTP Accept-Encoding header.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// <see cref="HttpContext.Current"/> is null.
        /// </exception>
        /// <remarks>
        /// Do note that <see cref="CompressionMethodScheme.Deflate"/> is avoided if possible. GZip is the de facto standard for HTTP compression.<br/>
        /// For more in-depth explanation of this choice, please refer to this link by Zoompf: http://zoompf.com/blog/2012/02/lose-the-wait-http-compression
        /// </remarks>
        public static CompressionMethodScheme ParseAcceptEncoding()
        {
            GlobalModule.CheckForHttpContextAvailability();
            return HttpRequestUtility.ParseAcceptEncoding(HttpContext.Current.Request);
        }

        /// <summary>
        /// This method will parse a sequence of <see cref="CompressionMethodScheme"/> and return the most appropriate <see cref="CompressionMethodScheme"/>.
        /// </summary>
        /// <param name="request">An instance of the <see cref="HttpRequest"/> object.</param>
        /// <returns>The most appropriate <see cref="CompressionMethodScheme"/> from the by <paramref name="request"/> resolved HTTP Accept-Encoding header.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="request"/> is null.
        /// </exception>
        /// <remarks>
        /// Do note that <see cref="CompressionMethodScheme.Deflate"/> is avoided if possible. GZip is the de facto standard for HTTP compression.<br/>
        /// For more in-depth explanation of this choice, please refer to this link by Zoompf: http://zoompf.com/blog/2012/02/lose-the-wait-http-compression
        /// </remarks>
        public static CompressionMethodScheme ParseAcceptEncoding(HttpRequest request)
        {
            if (request == null) { throw new ArgumentNullException(nameof(request)); }
            IEnumerable<CompressionMethodScheme> compressionMethods = GetAcceptEncodingHeader(request);
            return ParseAcceptEncoding(compressionMethods);
        }

        /// <summary>
        /// This method will parse a sequence of <see cref="CompressionMethodScheme"/> and return the most appropriate <see cref="CompressionMethodScheme"/>.
        /// </summary>
        /// <param name="compressionMethods">A sequence of <see cref="CompressionMethodScheme"/> to choose the appropriate <see cref="CompressionMethodScheme"/> from.</param>
        /// <returns>The most appropriate <see cref="CompressionMethodScheme"/> from the specified <paramref name="compressionMethods"/> sequence.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="compressionMethods"/> is null.
        /// </exception>
        /// <remarks>
        /// Do note that <see cref="CompressionMethodScheme.Deflate"/> is avoided if possible. GZip is the de facto standard for HTTP compression.<br/>
        /// For more in-depth explanation of this choice, please refer to this link by Zoompf: http://zoompf.com/blog/2012/02/lose-the-wait-http-compression
        /// </remarks>
        public static CompressionMethodScheme ParseAcceptEncoding(IEnumerable<CompressionMethodScheme> compressionMethods)
        {
            if (compressionMethods == null) { throw new ArgumentNullException(nameof(compressionMethods)); }
            List<CompressionMethodScheme> compressionMethodSchemes = new List<CompressionMethodScheme>(compressionMethods);
            CompressionMethodScheme compressionMethod = EnumerableUtility.FirstOrDefault(compressionMethodSchemes);
            if (compressionMethodSchemes.Contains(CompressionMethodScheme.GZip)) // changed from default DEFLATE to GZIP (http://zoompf.com/blog/2012/02/lose-the-wait-http-compression)
            {
                compressionMethod = CompressionMethodScheme.GZip;
            }
            return compressionMethod;
        }

        /// <summary>
        /// This method will parse a HTTP Accept-Encoding string and returns its <see cref="CompressionMethodScheme"/> equivalent.
        /// </summary>
        /// <param name="acceptEncoding">The HTTP Accept-Encoding string to parse.</param>
        /// <returns>A <see cref="CompressionMethodScheme"/> equivalent to the specified <paramref name="acceptEncoding"/>.</returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <remarks>This method will default to <see cref="CompressionMethodScheme.Identity"/> in case of an unknown HTTP Accept-Encoding.</remarks>
		public static CompressionMethodScheme ParseAcceptEncoding(string acceptEncoding)
        {
            if (acceptEncoding == null) { throw new ArgumentNullException(nameof(acceptEncoding)); }
            acceptEncoding = acceptEncoding.Trim().ToUpperInvariant();
            switch (acceptEncoding)
            {
                case "COMPRESS":
                    return CompressionMethodScheme.Compress;
                case "DEFLATE":
                    return CompressionMethodScheme.Deflate;
                case "*":
                case "GZIP":
                    return CompressionMethodScheme.GZip;
                default:
                    return CompressionMethodScheme.Identity;
            }
        }

        /// <summary>
        /// Determines whether a cached version of the content of the resource is found client-side given the specified <paramref name="validator"/>.
        /// </summary>
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        /// <returns>
        /// 	<c>true</c> if a cached version of the content of the resource is found client-side given the specified <paramref name="validator"/>; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="validator"/> is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <see cref="HttpContext"/> is unavailable.
        /// </exception>
        public static bool IsClientSideResourceCached(CacheValidator validator)
        {
            if (validator == null) { throw new ArgumentNullException(nameof(validator)); }
            return HttpRequestUtility.IsClientSideResourceCached(validator, HttpContext.Current.Request);
        }

        /// <summary>
        /// Determines whether a cached version of the content of the resource is found client-side given the specified <paramref name="validator"/>.
        /// </summary>
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        /// <param name="request">An instance of the <see cref="HttpRequest"/> object.</param>
        /// <returns>
        /// 	<c>true</c> if a cached version of the content of the resource is found client-side given the specified <paramref name="validator"/>; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="validator"/> is null - or - <paramref name="request"/> is null.
        /// </exception>
        public static bool IsClientSideResourceCached(CacheValidator validator, HttpRequest request)
        {
            if (validator == null) { throw new ArgumentNullException(nameof(validator)); }
            if (request == null) { throw new ArgumentNullException(nameof(request)); }
            return IsClientSideResourceCached(validator, request.Headers);
        }

        /// <summary>
        /// Determines whether a cached version of the content of the resource is found client-side given the specified <paramref name="validator"/>.
        /// </summary>
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        /// <param name="headers">A collection of HTTP request headers.</param>
        /// <returns>
        /// 	<c>true</c> if a cached version of the content of the resource is found client-side given the specified <paramref name="validator"/>; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="validator"/> is null - or - <paramref name="headers" /> is null.
        /// </exception>
        public static bool IsClientSideResourceCached(CacheValidator validator, NameValueCollection headers)
        {
            if (validator == null) { throw new ArgumentNullException(nameof(validator)); }
            if (headers == null) { throw new ArgumentNullException(nameof(headers)); }
            return HttpRequestUtility.IsClientSideResourceCached(validator, WebHeaderCollectionConverter.FromNameValueCollection(headers));
        }

        /// <summary>
        /// Determines whether a cached version of the requested resource is found client-side given the specified <paramref name="validator"/>.
        /// </summary>
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        /// <param name="headers">A collection of HTTP request headers.</param>
        /// <returns>
        ///     <c>true</c> if a cached version of the requested content is found client-side given the specified <paramref name="validator"/>; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="validator"/> is null - or - <paramref name="headers" /> is null.
        /// </exception>
        public static bool IsClientSideResourceCached(CacheValidator validator, WebHeaderCollection headers)
        {
            if (validator == null) { throw new ArgumentNullException(nameof(validator)); }
            if (headers == null) { throw new ArgumentNullException(nameof(headers)); }
            DateTime lastModified = validator.GetMostSignificant();
            lastModified = new DateTime(lastModified.Year, lastModified.Month, lastModified.Day, lastModified.Hour, lastModified.Minute, lastModified.Second, DateTimeKind.Utc); // make sure, that modified has the same format as the if-modified-since header
            DateTime result;
            TryGetLastModifiedHeader(headers, out result);
            bool isClientSideContentCached = (lastModified != DateTime.MinValue) && (result.ToUniversalTime() >= lastModified);
            if (isClientSideContentCached && validator.Strength != ChecksumStrength.None)
            {
                string clientSideContentEntityTag;
                if (TryGetEntityTagHeader(headers, out clientSideContentEntityTag))
                {
                    if (clientSideContentEntityTag.StartsWith("W/", StringComparison.OrdinalIgnoreCase)) { clientSideContentEntityTag = clientSideContentEntityTag.Remove(0, 2); }
                    int indexOfStartQuote = clientSideContentEntityTag.IndexOf('"');
                    int indexOfEndQuote = clientSideContentEntityTag.LastIndexOf('"');
                    if (indexOfStartQuote == 0 &&
                        (indexOfEndQuote > 2 && indexOfEndQuote == clientSideContentEntityTag.Length - 1))
                    {
                        clientSideContentEntityTag = clientSideContentEntityTag.Remove(indexOfEndQuote, 1);
                        clientSideContentEntityTag = clientSideContentEntityTag.Remove(indexOfStartQuote, 1);
                    }

                    isClientSideContentCached = validator.Checksum.ToHexadecimal().Equals(clientSideContentEntityTag, StringComparison.OrdinalIgnoreCase);
                }
            }
            return isClientSideContentCached;
        }

        /// <summary>
        /// Determines whether a cached version of the requested resource is found client-side given the last modified <see cref="DateTime"/> value.
        /// </summary>
        /// <param name="lastModified">A <see cref="DateTime"/> value indicating when the content of the resource was last modified.</param>
        /// <returns>
        /// 	<c>true</c> if a cached version of the requested resource is found client-side given the last modified <see cref="DateTime"/> value; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// <see cref="HttpContext"/> is unavailable.
        /// </exception>
		public static bool IsClientSideResourceCached(DateTime lastModified)
        {
            return HttpRequestUtility.IsClientSideResourceCached(lastModified, HttpContext.Current.Request);
        }

        /// <summary>
        /// Determines whether a cached version of the requested resource is found client-side given the last modified <see cref="DateTime"/> value.
        /// </summary>
        /// <param name="lastModified">A <see cref="DateTime"/> value indicating when the content of the resource was last modified.</param>
        /// <param name="request">An instance of the <see cref="HttpRequest"/> object.</param>
        /// <returns>
        /// 	<c>true</c> if a cached version of the requested resource is found client-side given the last modified <see cref="DateTime"/> value; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="request"/> is null.
        /// </exception>
        public static bool IsClientSideResourceCached(DateTime lastModified, HttpRequest request)
        {
            if (request == null) { throw new ArgumentNullException(nameof(request)); }
            return IsClientSideResourceCached(lastModified, request.Headers);
        }

        /// <summary>
        /// Determines whether a cached version of the requested resource is found client-side given the last modified <see cref="DateTime" /> value.
        /// </summary>
        /// <param name="lastModified">A <see cref="DateTime"/> value indicating when the content of the resource was last modified.</param>
        /// <param name="headers">A collection of HTTP request headers.</param>
        /// <returns>
        ///     <c>true</c> if a cached version of the requested content is found client-side given the <paramref name="lastModified"/> <see cref="DateTime" /> value; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="headers" /> is null.
        /// </exception>
        public static bool IsClientSideResourceCached(DateTime lastModified, NameValueCollection headers)
        {
            if (headers == null) { throw new ArgumentNullException(nameof(headers)); }
            return IsClientSideResourceCached(lastModified, headers, null);
        }

        /// <summary>
        /// Determines whether a cached version of the requested resource is found client-side given the last modified <see cref="DateTime" /> value and a match is determined from the server side <paramref name="entityTag"/> and client side equivalent.
        /// </summary>
        /// <param name="lastModified">A <see cref="DateTime"/> value indicating when the content of the resource was last modified.</param>
        /// <param name="headers">A collection of HTTP request headers.</param>
        /// <param name="entityTag">The server side entity tag to compare with the client side equivalent.</param>
        /// <returns>
        ///     <c>true</c> if a cached version of the requested content is found client-side given the <paramref name="lastModified"/> <see cref="DateTime" /> value and a match is determined from the server side <paramref name="entityTag"/> and client side equivalent; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="headers" /> is null.
        /// </exception>
        /// <remarks>
        /// This method performs an ordinal (case-sensitive and culture-insensitive) comparison against the server side <paramref name="entityTag"/> and client side equivalent.
        /// The search begins at the first character position of this string and continues through the last character position.
        /// </remarks>
        public static bool IsClientSideResourceCached(DateTime lastModified, NameValueCollection headers, string entityTag)
        {
            return HttpRequestUtility.IsClientSideResourceCached(lastModified, WebHeaderCollectionConverter.FromNameValueCollection(headers), entityTag);
        }

        /// <summary>
        /// Determines whether a cached version of the requested resource is found client-side given the last modified <see cref="DateTime" /> value.
        /// </summary>
        /// <param name="lastModified">A <see cref="DateTime"/> value indicating when the content of the resource was last modified.</param>
        /// <param name="headers">A collection of HTTP request headers.</param>
        /// <returns>
        ///     <c>true</c> if a cached version of the requested content is found client-side given the <paramref name="lastModified"/> <see cref="DateTime" /> value; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="headers" /> is null.
        /// </exception>
        public static bool IsClientSideResourceCached(DateTime lastModified, WebHeaderCollection headers)
        {
            if (headers == null) { throw new ArgumentNullException(nameof(headers)); }
            return IsClientSideResourceCached(lastModified, headers, null);
        }

        /// <summary>
        /// Determines whether a cached version of the requested resource is found client-side given the last modified <see cref="DateTime" /> value and a match is determined from the server side <paramref name="entityTag"/> and client side equivalent.
        /// </summary>
        /// <param name="lastModified">A <see cref="DateTime"/> value indicating when the content of the resource was last modified.</param>
        /// <param name="headers">A collection of HTTP request headers.</param>
        /// <param name="entityTag">The server side entity tag to compare with the client side equivalent.</param>
        /// <returns>
        ///     <c>true</c> if a cached version of the requested content is found client-side given the <paramref name="lastModified"/> <see cref="DateTime" /> value and a match is determined from the server side <paramref name="entityTag"/> and client side equivalent; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="headers" /> is null.
        /// </exception>
        /// <remarks>
        /// This method performs an ordinal (case-sensitive and culture-insensitive) comparison against the server side <paramref name="entityTag"/> and client side equivalent.
        /// The search begins at the first character position of this string and continues through the last character position.
        /// </remarks>
        public static bool IsClientSideResourceCached(DateTime lastModified, WebHeaderCollection headers, string entityTag)
        {
            if (headers == null) { throw new ArgumentNullException(nameof(headers)); }
            CacheValidator validator = new CacheValidator(lastModified, lastModified, entityTag);
            return IsClientSideResourceCached(validator, headers);
        }

        /// <summary>
        /// Resolves the entity tag string representation of an <b>If-None-Match</b> header.
        /// </summary>
        /// <param name="result">When this method returns, contains the entity tag specified by the <b>If-None-Match</b> header, if found; otherwise null. This parameter is passed uninitialized.</param>
        /// <returns>
        ///     <c>true</c> if the <b>If-None-Match</b> header was found in the <see cref="HttpContext"/>; otherwise, false.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// <see cref="HttpContext"/> is unavailable.
        /// </exception>
        public static bool TryGetEntityTagHeader(out string result)
        {
            GlobalModule.CheckForHttpContextAvailability();
            return TryGetEntityTagHeader(HttpContext.Current.Request, out result);
        }

        /// <summary>
        /// Resolves the entity tag string representation of an <b>If-None-Match</b> header.
        /// </summary>
        /// <param name="request">An instance of the <see cref="HttpRequest"/> object.</param>
        /// <param name="result">When this method returns, contains the entity tag specified by the <b>If-None-Match</b> header, if found; otherwise null. This parameter is passed uninitialized.</param>
        /// <returns>
        ///     <c>true</c> if the <b>If-None-Match</b> header was found in the <paramref name="request"/>; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="request"/> is null.
        /// </exception>
        public static bool TryGetEntityTagHeader(HttpRequest request, out string result)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            return TryGetEntityTagHeader(request.Headers, out result);
        }

        /// <summary>
        /// Resolves the entity tag string representation of an <b>If-None-Match</b> header.
        /// </summary>
        /// <param name="headers">A collection of HTTP headers.</param>
        /// <param name="result">When this method returns, contains the entity tag specified by the <b>If-None-Match</b> header, if found; otherwise null. This parameter is passed uninitialized.</param>
        /// <returns>
        ///     <c>true</c> if the <b>If-None-Match</b> header was found in the <paramref name="headers"/>; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="headers"/> is null.
        /// </exception>
        public static bool TryGetEntityTagHeader(NameValueCollection headers, out string result)
        {
            if (headers == null) { throw new ArgumentNullException(nameof(headers)); }
            return TryGetEntityTagHeader(WebHeaderCollectionConverter.FromNameValueCollection(headers), out result);
        }

        /// <summary>
        /// Resolves the entity tag string representation of an <b>If-None-Match</b> header.
        /// </summary>
        /// <param name="headers">A collection of HTTP headers.</param>
        /// <param name="result">When this method returns, contains the entity tag specified by the <b>If-None-Match</b> header, if found; otherwise null. This parameter is passed uninitialized.</param>
        /// <returns>
        ///     <c>true</c> if the <b>If-None-Match</b> header was found in the <paramref name="headers"/>; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="headers"/> is null.
        /// </exception>
        public static bool TryGetEntityTagHeader(WebHeaderCollection headers, out string result)
        {
            if (headers == null) { throw new ArgumentNullException(nameof(headers)); }
            string header = headers[HttpRequestHeader.IfNoneMatch];
            result = header;
            return !string.IsNullOrEmpty(result);
        }

        /// <summary>
        /// Resolves the string representation of an If-Modified-Since header and converts the value to its <see cref="DateTime"/> equivalent using the current <see cref="HttpContext"/>.
        /// </summary>
        /// <param name="result">When this method returns, contains the <see cref="DateTime"/> value equivalent to the date and time contained in <b>If-Modified-Since</b> header, if the conversion succeeded, or <see cref="DateTime.MinValue"/> if resolvement failed. The conversion fails if no <b>If-Modified-Since</b> header can be resolved. This parameter is passed uninitialized.</param>
        /// <returns>
        ///     <c>true</c> if the <b>If-Modified-Since</b> header was found in the <see cref="HttpContext"/>; otherwise, false.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// <see cref="HttpContext"/> is unavailable.
        /// </exception>
        public static bool TryGetLastModifiedHeader(out DateTime result)
        {
            GlobalModule.CheckForHttpContextAvailability();
            return TryGetLastModifiedHeader(HttpContext.Current.Request, out result);
        }

        /// <summary>
        /// Resolves the string representation of an If-Modified-Since header and converts the value to its <see cref="DateTime"/> equivalent.
        /// </summary>
        /// <param name="request">An instance of the <see cref="HttpRequest"/> object.</param>
        /// <param name="result">When this method returns, contains the <see cref="DateTime"/> value equivalent to the date and time contained in <b>If-Modified-Since</b> header, if the conversion succeeded, or <see cref="DateTime.MinValue"/> if resolvement failed. The conversion fails if no <b>If-Modified-Since</b> header can be resolved. This parameter is passed uninitialized.</param>
        /// <returns>
        ///     <c>true</c> if the <b>If-Modified-Since</b> header was found in the <paramref name="request"/>; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="request"/> is null.
        /// </exception>
        public static bool TryGetLastModifiedHeader(HttpRequest request, out DateTime result)
        {
            if (request == null) { throw new ArgumentNullException(nameof(request)); }
            return TryGetLastModifiedHeader(request.Headers, out result);
        }

        /// <summary>
        /// Resolves the string representation of an If-Modified-Since header and converts the value to its <see cref="DateTime"/> equivalent.
        /// </summary>
        /// <param name="headers">A collection of HTTP headers.</param>
        /// <param name="result">When this method returns, contains the <see cref="DateTime"/> value equivalent to the date and time contained in <b>If-Modified-Since</b> header, if the conversion succeeded, or <see cref="DateTime.MinValue"/> if resolvement failed. The conversion fails if no <b>If-Modified-Since</b> header can be resolved. This parameter is passed uninitialized.</param>
        /// <returns>
        ///     <c>true</c> if the <b>If-Modified-Since</b> header was found in the <paramref name="headers"/>; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="headers"/> is null.
        /// </exception>
        public static bool TryGetLastModifiedHeader(NameValueCollection headers, out DateTime result)
        {
            if (headers == null) { throw new ArgumentNullException(nameof(headers)); }
            return TryGetLastModifiedHeader(WebHeaderCollectionConverter.FromNameValueCollection(headers), out result);
        }

        /// <summary>
        /// Resolves the string representation of an If-Modified-Since header and converts the value to its <see cref="DateTime"/> equivalent.
        /// </summary>
        /// <param name="headers">A collection of HTTP headers.</param>
        /// <param name="result">When this method returns, contains the <see cref="DateTime"/> value equivalent to the date and time contained in <b>If-Modified-Since</b> header, if the conversion succeeded, or <see cref="DateTime.MinValue"/> if resolvement failed. The conversion fails if no <b>If-Modified-Since</b> header can be resolved. This parameter is passed uninitialized.</param>
        /// <returns>
        ///     <c>true</c> if the <b>If-Modified-Since</b> header was found in the <paramref name="headers"/>; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="headers"/> is null.
        /// </exception>
        public static bool TryGetLastModifiedHeader(WebHeaderCollection headers, out DateTime result)
        {
            if (headers == null) { throw new ArgumentNullException(nameof(headers)); }
            result = DateTime.MinValue;
            string header = headers[HttpRequestHeader.IfModifiedSince];
            if (!string.IsNullOrEmpty(header))
            {
                return DateTime.TryParse(header, out result);
            }
            return false;
        }

        /// <summary>
        /// Combines the specified query string <paramref name="sources"/> into one <see cref="String"/> equivalent field-value pairs as specified by the W3C.
        /// </summary>
        /// <param name="sources">A variable number of query string <paramref name="sources"/> to combine into one <see cref="String"/>.</param>
        /// <returns>A variable number of query string <paramref name="sources"/> combined into one <see cref="String"/> equivalent field-value pairs as specified by the W3C.</returns>
        public static string CombineFieldValuePairs(params string[] sources)
        {
            return CombineFieldValuePairs(FieldValueSeparator.Ampersand, sources);
        }

        /// <summary>
        /// Combines the specified query string, header or a form-data <paramref name="sources"/> into one <see cref="String"/> equivalent field-value pairs as specified by the W3C.
        /// </summary>
        /// <param name="sources">A variable number of query string, header or a form-data <paramref name="sources"/> to combine into one <see cref="String"/>.</param>
        /// <param name="separator">The <see cref="FieldValueSeparator"/> to use in the combination.</param>
        /// <returns>A variable number of query string, header or a form-data <paramref name="sources"/> combined into one <see cref="String"/> equivalent field-value pairs as specified by the W3C.</returns>
        public static string CombineFieldValuePairs(FieldValueSeparator separator, params string[] sources)
        {
            if (sources == null) { throw new ArgumentNullException(nameof(sources)); }
            List<NameValueCollection> nameValues = new List<NameValueCollection>();
            foreach (string fieldValuePairs in sources)
            {
                nameValues.Add(ParseFieldValuePairs(fieldValuePairs, separator));
            }
            return ParseFieldValuePairs(CombineFieldValuePairs(separator, nameValues.ToArray()), separator);
        }

        /// <summary>
        /// Combines the specified query string <paramref name="sources"/> into one <see cref="NameValueCollection"/> equivalent field-value pairs.
        /// </summary>
        /// <param name="sources">A variable number of query string <paramref name="sources"/> to combine into one <see cref="NameValueCollection"/>.</param>
        /// <returns>A variable number of query string <paramref name="sources"/> combined into one <see cref="NameValueCollection"/> equivalent field-value pairs.</returns>
        public static NameValueCollection CombineFieldValuePairs(params NameValueCollection[] sources)
        {
            return CombineFieldValuePairs(FieldValueSeparator.Ampersand, sources);
        }

        /// <summary>
        /// Combines the specified query string, header or a form-data <paramref name="sources"/> into one <see cref="NameValueCollection"/> equivalent field-value pairs.
        /// </summary>
        /// <param name="sources">A variable number of query string, header or a form-data <paramref name="sources"/> to combine into one <see cref="NameValueCollection"/>.</param>
        /// <param name="separator">The <see cref="FieldValueSeparator"/> to use in the combination.</param>
        /// <returns>A variable number of query string, header or a form-data <paramref name="sources"/> combined into one <see cref="NameValueCollection"/> equivalent field-value pairs.</returns>
        public static NameValueCollection CombineFieldValuePairs(FieldValueSeparator separator, params NameValueCollection[] sources)
        {
            if (sources == null) { throw new ArgumentNullException(nameof(sources)); }
            StringBuilder mergedFieldValuePairs = new StringBuilder(separator == FieldValueSeparator.Ampersand ? "?" : "");
            foreach (NameValueCollection fieldValuePairs in sources)
            {
                if (fieldValuePairs.Count == 0) { continue; }
                mergedFieldValuePairs.Append(separator == FieldValueSeparator.Ampersand ? ParseFieldValuePairs(fieldValuePairs, separator).Remove(0, 1) : ParseFieldValuePairs(fieldValuePairs, separator));
                mergedFieldValuePairs.Append(GetSeparator(separator));
            }
            if (mergedFieldValuePairs.Length > 0) { mergedFieldValuePairs.Remove(mergedFieldValuePairs.Length - 1, 1); }
            return ParseFieldValuePairs(mergedFieldValuePairs.ToString(), separator);
        }

        /// <summary>
        /// Parses a query string into a <see cref="String"/> equivalent field-value pairs as specified by the W3C.
        /// </summary>
        /// <param name="fieldValuePairs">The query string to parse.</param>
        /// <returns>A <see cref="String"/> value equivalent to the values in the <paramref name="fieldValuePairs"/>.</returns>
        public static string ParseFieldValuePairs(NameValueCollection fieldValuePairs)
        {
            return ParseFieldValuePairs(fieldValuePairs, FieldValueSeparator.Ampersand);
        }

        /// <summary>
        /// Parses a query string, header or a form-data into a <see cref="String"/> equivalent field-value pairs as specified by the W3C.
        /// </summary>
        /// <param name="fieldValuePairs">The query string, header or form-data values to parse.</param>
        /// <param name="separator">The <see cref="FieldValueSeparator"/> to use in the conversion.</param>
        /// <returns>A <see cref="String"/> value equivalent to the values in the <paramref name="fieldValuePairs"/>.</returns>
        public static string ParseFieldValuePairs(NameValueCollection fieldValuePairs, FieldValueSeparator separator)
        {
            return ParseFieldValuePairs(fieldValuePairs, separator, true);
        }

        /// <summary>
        /// Parses a query string, header or a form-data into a <see cref="String"/> equivalent field-value pairs as specified by the W3C.
        /// </summary>
        /// <param name="fieldValuePairs">The query string, header or form-data values to parse.</param>
        /// <param name="separator">The <see cref="FieldValueSeparator"/> to use in the conversion.</param>
        /// <param name="urlEncode">Encodes the output URL string. Default is true.</param>
        /// <returns>A <see cref="String"/> value equivalent to the values in the <paramref name="fieldValuePairs"/>.</returns>
        public static string ParseFieldValuePairs(NameValueCollection fieldValuePairs, FieldValueSeparator separator, bool urlEncode)
        {
            if (fieldValuePairs == null) throw new ArgumentNullException(nameof(fieldValuePairs));
            char characterSeperator = GetSeparator(separator);
            StringBuilder builder = new StringBuilder(separator == FieldValueSeparator.Ampersand ? "?" : "");
            foreach (string item in fieldValuePairs)
            {
                builder.AppendFormat("{0}={1}", item, urlEncode ? HttpUtility.UrlEncode(HttpUtility.UrlDecode(fieldValuePairs[item])) : fieldValuePairs[item]); // the HttpUtility.UrlDecode is called when used outside of IIS .. IIS auto UrlEncode .. why we have to assure, that we don't double UrlEncode ..
                builder.Append(characterSeperator);
            }
            if (builder.Length > 0 && separator == FieldValueSeparator.Ampersand) { builder.Remove(builder.Length - 1, 1); }
            return builder.ToString();
        }

        /// <summary>
        /// Parses a query string into a <see cref="NameValueCollection"/> equivalent field-value pairs.
        /// </summary>
        /// <param name="fieldValuePairs">The query string to parse.</param>
        /// <returns>A <see cref="NameValueCollection"/> value equivalent to the values in the <paramref name="fieldValuePairs"/>.</returns>
        public static NameValueCollection ParseFieldValuePairs(string fieldValuePairs)
        {
            return ParseFieldValuePairs(fieldValuePairs, FieldValueSeparator.Ampersand);
        }

        /// <summary>
        /// Parses a query string, header or a form-data into a <see cref="NameValueCollection"/> equivalent field-value pairs.
        /// </summary>
        /// <param name="fieldValuePairs">The query string, header or form-data values to parse.</param>
        /// <param name="separator">The <see cref="FieldValueSeparator"/> to use in the conversion.</param>
        /// <returns>A <see cref="NameValueCollection"/> value equivalent to the values in the <paramref name="fieldValuePairs"/>.</returns>
        public static NameValueCollection ParseFieldValuePairs(string fieldValuePairs, FieldValueSeparator separator)
        {
            return ParseFieldValuePairs(fieldValuePairs, separator, false);
        }

        /// <summary>
        /// Parses a query string, header or a form-data into a <see cref="NameValueCollection"/> equivalent field-value pairs.
        /// </summary>
        /// <param name="fieldValuePairs">The query string, header or form-data values to parse.</param>
        /// <param name="urlDecode">Converts <paramref name="fieldValuePairs"/> that has been encoded for transmission in a URL into a decoded string. Default is false.</param>
        /// <param name="separator">The <see cref="FieldValueSeparator"/> to use in the conversion.</param>
        /// <returns>A <see cref="NameValueCollection"/> value equivalent to the values in the <paramref name="fieldValuePairs"/>.</returns>
        public static NameValueCollection ParseFieldValuePairs(string fieldValuePairs, FieldValueSeparator separator, bool urlDecode)
        {
            if (fieldValuePairs == null) { throw new ArgumentNullException(nameof(fieldValuePairs)); }
            NameValueCollection modifiedFieldValuePairs = new NameValueCollection();
            if (fieldValuePairs.Length == 0) { return modifiedFieldValuePairs; }
            char characterSeperator = GetSeparator(separator);
            if (separator == FieldValueSeparator.Ampersand && fieldValuePairs.StartsWith("?", StringComparison.OrdinalIgnoreCase)) { fieldValuePairs = fieldValuePairs.Remove(0, 1); }
            string[] namesAndValues = fieldValuePairs.Split(characterSeperator);
            foreach (string nameAndValue in namesAndValues)
            {
                int equalLocation = nameAndValue.IndexOf("=", StringComparison.OrdinalIgnoreCase);
                if (equalLocation < 0) { continue; } // we have no parameter values, just a value pair like lcid=1030& or lcid=1030&test
                string value = equalLocation == nameAndValue.Length ? null : urlDecode ? HttpUtility.UrlDecode(nameAndValue.Substring(equalLocation + 1)) : nameAndValue.Substring(equalLocation + 1);
                modifiedFieldValuePairs.Add(nameAndValue.Substring(0, nameAndValue.IndexOf("=", StringComparison.OrdinalIgnoreCase)), value);
            }
            return modifiedFieldValuePairs;
        }

        private static char GetSeparator(FieldValueSeparator separator)
        {
            switch (separator)
            {
                case FieldValueSeparator.Ampersand:
                    return '&';
                case FieldValueSeparator.Semicolon:
                    return ';';
            }
            throw new ArgumentOutOfRangeException(nameof(separator));
        }

        /// <summary>
        /// Sanitizes the query string, header or form-data from the specified arguments, using all of the keys gathered in <paramref name="fieldValuePairs"/> and <see cref="FieldValueSeparator.Ampersand"/> as the separator.
        /// </summary>
        /// <param name="fieldValuePairs">The query string, header or form-data to sanitize.</param>
        /// <param name="filter">The filter action to perform on the <paramref name="fieldValuePairs"/>.</param>
        /// <returns>A sanitized <see cref="String"/> equivalent of <paramref name="fieldValuePairs"/>.</returns>
        public static string SanitizeFieldValuePairs(string fieldValuePairs, FieldValueFilter filter)
        {
            if (fieldValuePairs == null) throw new ArgumentNullException(nameof(fieldValuePairs));
            NameValueCollection tempFieldValuePairs = ParseFieldValuePairs(fieldValuePairs);
            return SanitizeFieldValuePairs(fieldValuePairs, filter, tempFieldValuePairs.AllKeys);
        }

        /// <summary>
        /// Sanitizes the query string from the specified arguments, using <see cref="FieldValueSeparator.Ampersand"/> as the separator.
        /// </summary>
        /// <param name="fieldValuePairs">The query string to sanitize.</param>
        /// <param name="filter">The filter action to perform on the <paramref name="fieldValuePairs"/>.</param>
        /// <param name="keys">The keys to use in the filter process.</param>
        /// <returns>A sanitized <see cref="String"/> equivalent of <paramref name="fieldValuePairs"/>.</returns>
        public static string SanitizeFieldValuePairs(string fieldValuePairs, FieldValueFilter filter, params string[] keys)
        {
            return SanitizeFieldValuePairs(fieldValuePairs, filter, FieldValueSeparator.Ampersand, keys);
        }

        /// <summary>
        /// Sanitizes the query string, header or form-data from the specified arguments.
        /// </summary>
        /// <param name="fieldValuePairs">The query string, header or form-data to sanitize.</param>
        /// <param name="filter">The filter action to perform on the <paramref name="fieldValuePairs"/>.</param>
        /// <param name="separator">The <see cref="FieldValueSeparator"/> to use in the result.</param>
        /// <param name="keys">The keys to use in the filter process.</param>
        /// <returns>A sanitized <see cref="String"/> equivalent of <paramref name="fieldValuePairs"/>.</returns>
        public static string SanitizeFieldValuePairs(string fieldValuePairs, FieldValueFilter filter, FieldValueSeparator separator, IEnumerable<string> keys)
        {
            if (fieldValuePairs == null) throw new ArgumentNullException(nameof(fieldValuePairs));
            if (keys == null) throw new ArgumentNullException(nameof(keys));
            NameValueCollection newFieldValuePairs = ParseFieldValuePairs(fieldValuePairs, separator);
            return ParseFieldValuePairs(SanitizeFieldValuePairs(newFieldValuePairs, filter, keys), separator);
        }

        /// <summary>
        /// Sanitizes the query string, header or form-data from the specified arguments, using all of the keys gathered in <paramref name="fieldValuePairs"/> and <see cref="FieldValueSeparator.Ampersand"/> as the separator.
        /// </summary>
        /// <param name="fieldValuePairs">The query string, header or form-data to sanitize.</param>
        /// <param name="filter">The filter action to perform on the <paramref name="fieldValuePairs"/>.</param>
        /// <returns>A sanitized <see cref="NameValueCollection"/> equivalent of <paramref name="fieldValuePairs"/>.</returns>
        public static NameValueCollection SanitizeFieldValuePairs(NameValueCollection fieldValuePairs, FieldValueFilter filter)
        {
            if (fieldValuePairs == null) throw new ArgumentNullException(nameof(fieldValuePairs));
            return SanitizeFieldValuePairs(fieldValuePairs, filter, fieldValuePairs.AllKeys);
        }

        /// <summary>
        /// Sanitizes the query string, header or form-data from the specified arguments, using <see cref="FieldValueSeparator.Ampersand"/> as the separator.
        /// </summary>
        /// <param name="fieldValuePairs">The query string, header or form-data to sanitize.</param>
        /// <param name="filter">The filter action to perform on the <paramref name="fieldValuePairs"/>.</param>
        /// <param name="keys">The keys to use in the filter process.</param>
        /// <returns>A sanitized <see cref="NameValueCollection"/> equivalent of <paramref name="fieldValuePairs"/>.</returns>
        public static NameValueCollection SanitizeFieldValuePairs(NameValueCollection fieldValuePairs, FieldValueFilter filter, params string[] keys)
        {
            return SanitizeFieldValuePairs(fieldValuePairs, filter, (IEnumerable<string>)keys);
        }

        /// <summary>
        /// Sanitizes the query string, header or form-data from the specified arguments.
        /// </summary>
        /// <param name="fieldValuePairs">The query string, header or form-data to sanitize.</param>
        /// <param name="filter">The filter action to perform on the <paramref name="fieldValuePairs"/>.</param>
        /// <param name="keys">The keys to use in the filter process.</param>
        /// <returns>A sanitized <see cref="NameValueCollection"/> equivalent of <paramref name="fieldValuePairs"/>.</returns>
        public static NameValueCollection SanitizeFieldValuePairs(NameValueCollection fieldValuePairs, FieldValueFilter filter, IEnumerable<string> keys)
        {
            if (fieldValuePairs == null) throw new ArgumentNullException(nameof(fieldValuePairs));
            if (keys == null) throw new ArgumentNullException(nameof(keys));
            NameValueCollection modifiedFieldValuePairs = new NameValueCollection(fieldValuePairs);
            switch (filter)
            {
                case FieldValueFilter.RemoveDublets:
                    foreach (string key in keys)
                    {
                        string[] values = modifiedFieldValuePairs[key].Split(',');
                        int zeroBasedIndex = values.Length - 1;
                        if (zeroBasedIndex >= 0) { modifiedFieldValuePairs[key] = values[zeroBasedIndex]; }
                    }
                    break;
                case FieldValueFilter.Remove:
                    foreach (string key in keys)
                    {
                        modifiedFieldValuePairs.Remove(key);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(filter));
            }
            return modifiedFieldValuePairs;
        }
    }
}