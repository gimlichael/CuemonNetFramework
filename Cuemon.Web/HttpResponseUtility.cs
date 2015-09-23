using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using Cuemon.Caching;
using Cuemon.Reflection;

namespace Cuemon.Web
{
    /// <summary>
    /// This utility class is designed to make <see cref="HttpResponse"/> operations easier to work with.
    /// </summary>
    public static class HttpResponseUtility
    {
        /// <summary>
        /// Redirects and instructs the client to a new URL <paramref name="location"/>.
        /// </summary>
        /// <param name="location">The target location of the redirect.</param>
        /// <remarks>Uses the static <see cref="HttpContext.Current"/> to retrieve an instance of the <see cref="HttpResponse"/> object.</remarks>
        public static void Redirect(Uri location)
        {
            Redirect(location, null);
        }

        /// <summary>
        /// Redirects and instructs the client to a new URL <paramref name="location"/>.
        /// </summary>
        /// <param name="location">The target location of the redirect.</param>
        /// <param name="newRelativeLocation">Changes the relative part of the specified <paramref name="location"/> leaving host authority and query-string intact.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="location"/> is null.
        /// </exception>
        /// <remarks>Uses the static <see cref="HttpContext.Current"/> to retrieve an instance of the <see cref="HttpResponse"/> object.</remarks>
        public static void Redirect(Uri location, string newRelativeLocation)
        {
            GlobalModule.CheckForHttpContextAvailability();
            if (location == null) { throw new ArgumentNullException("location"); }
            Redirect(HttpContext.Current, location, newRelativeLocation);
        }

        /// <summary>
        /// Redirects and instructs the client to a new URL <paramref name="location"/>.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <param name="location">The location.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="context"/> is null -or- <paramref name="location"/> is null.
        /// </exception>
        public static void Redirect(HttpContext context, Uri location)
        {
            if (context == null) { throw new ArgumentNullException("context"); }
            if (location == null) { throw new ArgumentNullException("location"); }
            context.Response.Redirect(location.OriginalString, false);
            context.ApplicationInstance.CompleteRequest();
        }

        /// <summary>
        /// Redirects and instructs the client to a new URL <paramref name="location"/>.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <param name="location">The target location of the redirect.</param>
        /// <param name="newRelativeLocation">Changes the relative part of the specified <paramref name="location"/> leaving host authority and query-string intact.</param>
        public static void Redirect(HttpContext context, Uri location, string newRelativeLocation)
        {
            Redirect(context, newRelativeLocation == null ? location : new Uri(HttpRequestUtility.GetHostAuthority(location), string.Format(CultureInfo.InvariantCulture, "{0}{1}", newRelativeLocation, location.Query)));
        }

        /// <summary>
        /// Redirects and instructs the client to a new permanent URL <paramref name="location"/>.
        /// </summary>
        /// <param name="location">The permanent target location of the redirect.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="location"/> is null.
        /// </exception>
        /// <remarks>Uses the static <see cref="HttpContext.Current"/> to retrieve an instance of the <see cref="HttpResponse"/> object.</remarks>
        public static void RedirectPermanently(Uri location)
        {
            if (location == null) { throw new ArgumentNullException("location"); }
            GlobalModule.CheckForHttpContextAvailability();
            RedirectPermanently(HttpContext.Current, location);
        }

        /// <summary>
        /// Redirects and instructs the client to a new permanent URL <paramref name="location"/>.
        /// </summary>
        /// <param name="location">The permanent target location.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="location"/> is null.
        /// </exception>
        /// <remarks>Uses the static <see cref="HttpContext.Current"/> to retrieve an instance of the <see cref="HttpResponse"/> object.</remarks>
        public static void RedirectPermanently(string location)
        {
            if (location == null) { throw new ArgumentNullException("location"); }
            GlobalModule.CheckForHttpContextAvailability();
            RedirectPermanently(HttpContext.Current, location);
        }

        /// <summary>
        /// Redirects and instructs the client to a new permanent URL <paramref name="location"/>.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <param name="location">The permanent target location.</param>
        public static void RedirectPermanently(HttpContext context, Uri location)
        {
            if (context == null) { throw new ArgumentNullException("context"); }
            if (location == null) { throw new ArgumentNullException("location"); }
            RedirectPermanently(context, location.OriginalString);
        }

        /// <summary>
        /// Redirects and instructs the client to a new permanent URL <paramref name="location"/>.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <param name="location">The permanent target location.</param>
        public static void RedirectPermanently(HttpContext context, string location)
        {
            if (context == null) { throw new ArgumentNullException("context"); }
            if (location == null) { throw new ArgumentNullException("location"); }
            context.Response.StatusDescription = "301 Moved Permanently";
            context.Response.StatusCode = (int)HttpStatusCode.MovedPermanently;
            context.Response.RedirectLocation = location;
            context.ApplicationInstance.CompleteRequest();
        }

        /// <summary>
        /// Disables all client side caching.
        /// </summary>
        /// <remarks>Uses the static <see cref="HttpContext.Current"/> to retrieve an instance of the <see cref="HttpResponse"/> class.</remarks>
        public static void DisableClientSideResourceCache()
        {
            GlobalModule.CheckForHttpContextAvailability();
            DisableClientSideResourceCache(HttpContext.Current.Response);
        }

        /// <summary>
        /// Disables all client side caching.
        /// </summary>
        /// <param name="response">An instance of the <see cref="HttpResponse"/> object.</param>
        public static void DisableClientSideResourceCache(HttpResponse response)
        {
            if (response == null) { throw new ArgumentNullException("response"); }
            response.Cache.SetNoStore();
            response.Cache.SetCacheability(HttpCacheability.NoCache);
            response.AppendHeader("Pragma", "no-cache");
            response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            response.Cache.SetNoTransforms();
        }

        /// <summary>
        /// A helper method designed to set the appropriate HTTP Content-Encoding header for the content being delivered to the client.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// <see cref="HttpContext.Current"/> is null.
        /// </exception>
        /// <remarks>
        /// Do note that <see cref="CompressionMethodScheme.Deflate"/> is avoided if possible. GZip is the de facto standard for HTTP compression.<br/>
        /// For more in-depth explanation of this choice, please refer to this link by Zoompf: http://zoompf.com/blog/2012/02/lose-the-wait-http-compression
        /// </remarks>
        public static void SetClientCompressionMethod()
        {
            GlobalModule.CheckForHttpContextAvailability();
            HttpContext context = HttpContext.Current;
            SetClientCompressionMethod(context.Response, context.Request);
        }

        /// <summary>
        /// A helper method designed to set the appropriate HTTP Content-Encoding header for the content being delivered to the client.
        /// </summary>
        /// <param name="response">An instance of the <see cref="HttpResponse"/> object.</param>
        /// <param name="request">An instance of the <see cref="HttpResponse"/> object.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="response"/> is null or <br/>
        /// <paramref name="request"/> is null.
        /// </exception>
        /// <remarks>
        /// Do note that <see cref="CompressionMethodScheme.Deflate"/> is avoided if possible. GZip is the de facto standard for HTTP compression.<br/>
        /// For more in-depth explanation of this choice, please refer to this link by Zoompf: http://zoompf.com/blog/2012/02/lose-the-wait-http-compression
        /// </remarks>
        public static void SetClientCompressionMethod(HttpResponse response, HttpRequest request)
        {
            if (response == null) { throw new ArgumentNullException("response"); }
            if (request == null) { throw new ArgumentNullException("request"); }
            IEnumerable<CompressionMethodScheme> compressionMethods = HttpRequestUtility.GetAcceptEncodingHeader(request);
            SetClientCompressionMethod(response, compressionMethods);
        }

        /// <summary>
        /// A helper method designed to set the appropriate HTTP Content-Encoding header for the content being delivered to the client.
        /// </summary>
        /// <param name="compressionMethods">A sequence of <see cref="CompressionMethodScheme"/> to choose the appropriate HTTP Content-Encoding header by.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="compressionMethods"/> is null.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// <see cref="HttpContext.Current"/> is null.
        /// </exception>
        /// <remarks>
        /// Do note that <see cref="CompressionMethodScheme.Deflate"/> is avoided if possible. GZip is the de facto standard for HTTP compression.<br/>
        /// For more in-depth explanation of this choice, please refer to this link by Zoompf: http://zoompf.com/blog/2012/02/lose-the-wait-http-compression
        /// </remarks>
        public static void SetClientCompressionMethod(IEnumerable<CompressionMethodScheme> compressionMethods)
        {
            GlobalModule.CheckForHttpContextAvailability();
            SetClientCompressionMethod(HttpContext.Current.Response, compressionMethods);
        }

        /// <summary>
        /// A helper method designed to set the appropriate HTTP Content-Encoding header for the content being delivered to the client.
        /// </summary>
        /// <param name="response">An instance of the <see cref="HttpResponse"/> object.</param>
        /// <param name="compressionMethods">A sequence of <see cref="CompressionMethodScheme"/> to choose the appropriate HTTP Content-Encoding header by.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="response"/> is null or <br/>
        /// <paramref name="compressionMethods"/> is null.
        /// </exception>
        /// <remarks>
        /// Do note that <see cref="CompressionMethodScheme.Deflate"/> is avoided if possible. GZip is the de facto standard for HTTP compression.<br/>
        /// For more in-depth explanation of this choice, please refer to this link by Zoompf: http://zoompf.com/blog/2012/02/lose-the-wait-http-compression
        /// </remarks>
        public static void SetClientCompressionMethod(HttpResponse response, IEnumerable<CompressionMethodScheme> compressionMethods)
        {
            if (response == null) { throw new ArgumentNullException("response"); }
            if (compressionMethods == null) { throw new ArgumentNullException("compressionMethods"); }
            CompressionMethodScheme compressionMethod = HttpRequestUtility.ParseAcceptEncoding(compressionMethods);
            SetClientCompressionMethod(response, compressionMethod);
        }

        /// <summary>
        /// A helper method designed to set the appropriate HTTP Content-Encoding header for the content being delivered to the client.
        /// </summary>
        /// <param name="response">An instance of the <see cref="HttpResponse"/> object.</param>
        /// <param name="compressionMethod">The compression method to apply to the HTTP Content-Encoding header.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="response"/> is null.
        /// </exception>
        /// <remarks>
        /// The HTTP Content-Encoding header is only applied if <paramref name="compressionMethod"/> equals <see cref="CompressionMethodScheme.Deflate"/> or <see cref="CompressionMethodScheme.GZip"/>.
        /// </remarks>
        public static void SetClientCompressionMethod(HttpResponse response, CompressionMethodScheme compressionMethod)
        {
            if (response == null) { throw new ArgumentNullException("response"); }
            switch (compressionMethod)
            {
                case CompressionMethodScheme.Deflate:
                case CompressionMethodScheme.GZip:
                    RemoveResponseHeader(response, "Content-Encoding");
                    response.AppendHeader("Content-Encoding", compressionMethod.ToString().ToLowerInvariant());
                    break;
            }
        }

        /// <summary>
        /// A helper method designed to remove the by <paramref name="name"/> specified HTTP header.
        /// </summary>
        /// <param name="response">An instance of the <see cref="HttpResponse"/> object.</param>
        /// <param name="name">The name of the HTTP header to remove from the output stream.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="response"/> is null or<br/>
        /// <paramref name="name"/> is null.
        /// </exception>
        /// <exception cref="Cuemon.ArgumentEmptyException">
        /// <paramref name="name"/> is empty.
        /// </exception>
        /// <remarks>This method is backward compatible to IIS 6.0.</remarks>
        public static void RemoveResponseHeader(HttpResponse response, string name)
        {
            if (response == null) { throw new ArgumentNullException("response"); }
            if (name == null) { throw new ArgumentNullException("name"); }
            if (name.Length == 0) { throw new ArgumentEmptyException("name"); }
            if (HttpRuntimeUtility.SupportsIisIntegratedPipelineMode)
            {
                if (response.Headers[name] != null) { response.Headers.Remove(name); }
            }
            else // this is crazy backward compatibility to IIS 6, but we need to find a way to avoid the risk of double header entries
            {
                FieldInfo headersInfo = ReflectionUtility.GetField(response.GetType(), "_customHeaders", ReflectionUtility.BindingInstancePublicAndPrivateNoneInherited);
                if (headersInfo != null)
                {
                    ArrayList headers = headersInfo.GetValue(response) as ArrayList;
                    if (headers != null)
                    {
                        if (headers.Count > 0)
                        {
                            List<int> indexesToRemove = new List<int>();
                            for (int i = 0; i < headers.Count; i++)
                            {
                                object header = headers[i];
                                PropertyInfo headerNameInfo = ReflectionUtility.GetProperty(header.GetType(), "Name", typeof(string), null, ReflectionUtility.BindingInstancePublicAndPrivateNoneInherited);
                                if (headerNameInfo != null)
                                {
                                    string headerName = headerNameInfo.GetValue(header, null) as string;
                                    if (headerName != null && headerName.Equals(name, StringComparison.OrdinalIgnoreCase))
                                    {
                                        indexesToRemove.Add(i);
                                    }
                                }
                            }
                            foreach (int indexToRemove in indexesToRemove) { headers.RemoveAt(indexToRemove); }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// A helper method designed to set the appropriate HTTP client-side related cache headers for the content being delivered to the client.
        /// </summary>
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        /// <param name="expires">The UTC date time value to expire the <see cref="HttpCacheability.Private"/> caching.</param>
        /// <remarks>Uses the static <see cref="HttpContext.Current"/> to retrieve an instance of the <see cref="HttpResponse"/> class.</remarks>
        /// <exception cref="InvalidOperationException">
        /// <see cref="HttpContext"/> is unavailable.
        /// </exception>
        public static void SetClientSideContentCacheExpiresHeaders(CacheValidator validator, DateTime expires)
        {
            GlobalModule.CheckForHttpContextAvailability();
            SetClientSideContentCacheExpiresHeaders(HttpContext.Current.Request, HttpContext.Current.Response, validator, expires);
        }

        /// <summary>
        /// A helper method designed to set the appropriate HTTP client-side related cache headers for the content being delivered to the client.
        /// </summary>
        /// <param name="request">An instance of the <see cref="HttpRequest"/> object.</param>
        /// <param name="response">An instance of the <see cref="HttpResponse"/> object.</param>
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        /// <param name="expires">The UTC date time value to expire the <see cref="HttpCacheability.Private"/> caching.</param>
        public static void SetClientSideContentCacheExpiresHeaders(HttpRequest request, HttpResponse response, CacheValidator validator, DateTime expires)
        {
            SetClientSideContentCacheExpiresHeaders(request, response, validator, expires, HttpCacheability.Private);
        }

        /// <summary>
        /// A helper method designed to set the appropriate HTTP client-side related cache headers for the content being delivered to the client.
        /// </summary>
        /// <param name="request">An instance of the <see cref="HttpRequest"/> object.</param>
        /// <param name="response">An instance of the <see cref="HttpResponse"/> object.</param>
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        /// <param name="expires">The UTC date time value to expire the <paramref name="cacheability"/> caching.</param>
        /// <param name="cacheability">Sets the <b>Cache-Control</b> header to one of the values of <see cref="HttpCacheability"/>.</param>
        /// <remarks>
        /// The <paramref name="validator"/> parameter is used to set the <b>Last-Modified</b> header in the response but also used to determined if the <b>If-Modified-Since</b> header in the request is equal to the specified <see cref="CacheValidator.Modified"/> value.
        /// If equal, HTTP content is not sent to the client and a status code of <see cref="HttpStatusCode.NotModified"/> is set.
        /// <br/>
        /// <br/>
        /// The <paramref name="expires"/> parameter is used to set the <b>Expires</b> header in the response.
        /// <br/>
        /// <br/>
        /// The <paramref name="cacheability"/> parameter is used to guide and set the <b>Cache-Control</b> header and <b>Pragma</b> header in the response.
        /// <br/>
        /// <br/>
        /// The <paramref name="validator"/> parameter is also used to set the <b>ETag</b> header in the response but also used to determined if the <b>If-None-Match</b> header in the request is equal to the specified <see cref="CacheValidator.Checksum"/> using a ruleset of <see cref="StringComparison.Ordinal"/>.
        /// If equal, HTTP content is not sent to the client and a status code of <see cref="HttpStatusCode.NotModified"/> is set.
        /// <br/>
        /// <br/>
        /// <b>Note:</b> The <b>If-None-Match</b> header trumps the <b>If-Modified-Since</b> header.
        /// </remarks>
        public static void SetClientSideContentCacheExpiresHeaders(HttpRequest request, HttpResponse response, CacheValidator validator, DateTime expires, HttpCacheability cacheability)
        {
            if (request == null) { throw new ArgumentNullException("request"); }
            if (response == null) { throw new ArgumentNullException("response"); }
            if (validator == null) { throw new ArgumentNullException("validator"); }

            #region Clear ClientCache Related Headers
            RemoveResponseHeader(response, "Expires");
            RemoveResponseHeader(response, "Cache-Control");
            RemoveResponseHeader(response, "Pragma");
            RemoveResponseHeader(response, "Last-Modified");
            RemoveResponseHeader(response, "ETag");
            #endregion

            HttpStatusCode statusCode;
            WebHeaderCollection headers = CreateClientSideContentCacheExpiresHeaders(validator, expires, cacheability, HttpRequestUtility.IsClientSideResourceCached, request.Headers, out statusCode);
            response.Headers.Add(headers);

            response.StatusCode = (int)statusCode;
            if (statusCode == HttpStatusCode.NotModified) { response.SuppressContent = true; }
        }

        /// <summary>
        /// A helper method designed to set the appropriate HTTP client-side related cache headers for the content being delivered to the client.
        /// </summary>
        /// <param name="lastModified">The UTC date time value of the content being delivered last modified time stamp.</param>
        /// <param name="expires">The UTC date time value to expire the <see cref="HttpCacheability.Private"/> caching.</param>
        /// <remarks>Uses the static <see cref="HttpContext.Current"/> to retrieve an instance of the <see cref="HttpResponse"/> class.</remarks>
        /// <exception cref="InvalidOperationException">
        /// <see cref="HttpContext"/> is unavailable.
        /// </exception>
        public static void SetClientSideContentCacheExpiresHeaders(DateTime lastModified, DateTime expires)
        {
            GlobalModule.CheckForHttpContextAvailability();
            SetClientSideContentCacheExpiresHeaders(HttpContext.Current.Request, HttpContext.Current.Response, lastModified, expires);
        }

        /// <summary>
        /// A helper method designed to set the appropriate HTTP client-side related cache headers for the content being delivered to the client.
        /// </summary>
        /// <param name="request">An instance of the <see cref="HttpRequest"/> object.</param>
        /// <param name="response">An instance of the <see cref="HttpResponse"/> object.</param>
        /// <param name="lastModified">The UTC date time value of the content being delivered last modified time stamp.</param>
        /// <param name="expires">The UTC date time value to expire the <see cref="HttpCacheability.Private"/> caching.</param>
        public static void SetClientSideContentCacheExpiresHeaders(HttpRequest request, HttpResponse response, DateTime lastModified, DateTime expires)
        {
            SetClientSideContentCacheExpiresHeaders(request, response, lastModified, expires, HttpCacheability.Private);
        }

        /// <summary>
        /// A helper method designed to set the appropriate HTTP client-side related cache headers for the content being delivered to the client.
        /// </summary>
        /// <param name="request">An instance of the <see cref="HttpRequest"/> object.</param>
        /// <param name="response">An instance of the <see cref="HttpResponse"/> object.</param>
        /// <param name="lastModified">The UTC date time value of the content being delivered last modified time stamp.</param>
        /// <param name="expires">The UTC date time value to expire the <paramref name="cacheability"/> caching.</param>
        /// <param name="cacheability">Sets the <b>Cache-Control</b> header to one of the values of <see cref="HttpCacheability"/>.</param>
        /// <remarks>
        /// The <paramref name="lastModified"/> parameter is used to set the <b>Last-Modified</b> header in the response but also used to determined if the <b>If-Modified-Since</b> header in the request is equal to the specified <paramref name="lastModified"/> value.
        /// If equal, HTTP content is not sent to the client and a status code of <see cref="HttpStatusCode.NotModified"/> is set.
        /// <br/>
        /// <br/>
        /// The <paramref name="expires"/> parameter is used to set the <b>Expires</b> header in the response.
        /// <br/>
        /// <br/>
        /// The <paramref name="cacheability"/> parameter is used to guide and set the <b>Cache-Control</b> header and <b>Pragma</b> header in the response.
        /// </remarks>
        public static void SetClientSideContentCacheExpiresHeaders(HttpRequest request, HttpResponse response, DateTime lastModified, DateTime expires, HttpCacheability cacheability)
        {
            SetClientSideContentCacheExpiresHeaders(request, response, lastModified, expires, cacheability, null);
        }

        /// <summary>
        /// A helper method designed to set the appropriate HTTP client-side related cache headers for the content being delivered to the client.
        /// </summary>
        /// <param name="request">An instance of the <see cref="HttpRequest"/> object.</param>
        /// <param name="response">An instance of the <see cref="HttpResponse"/> object.</param>
        /// <param name="lastModified">The UTC date time value of the content being delivered last modified time stamp.</param>
        /// <param name="expires">The UTC date time value to expire the <paramref name="cacheability"/> caching.</param>
        /// <param name="cacheability">Sets the <b>Cache-Control</b> header to one of the values of <see cref="HttpCacheability"/>.</param>
        /// <param name="entityTag">Set the <b>ETag</b> header to the specified value.</param>
        /// <remarks>
        /// The <paramref name="lastModified"/> parameter is used to set the <b>Last-Modified</b> header in the response but also used to determined if the <b>If-Modified-Since</b> header in the request is equal to the specified <paramref name="lastModified"/> value.
        /// If equal, HTTP content is not sent to the client and a status code of <see cref="HttpStatusCode.NotModified"/> is set.
        /// <br/>
        /// <br/>
        /// The <paramref name="expires"/> parameter is used to set the <b>Expires</b> header in the response.
        /// <br/>
        /// <br/>
        /// The <paramref name="cacheability"/> parameter is used to guide and set the <b>Cache-Control</b> header and <b>Pragma</b> header in the response.
        /// <br/>
        /// <br/>
        /// The <paramref name="entityTag"/> parameter is used to set the <b>ETag</b> header in the response but also used to determined if the <b>If-None-Match</b> header in the request is equal to the specified <paramref name="entityTag"/> using a ruleset of <see cref="StringComparison.Ordinal"/>.
        /// If equal, HTTP content is not sent to the client and a status code of <see cref="HttpStatusCode.NotModified"/> is set.
        /// <br/>
        /// <br/>
        /// <b>Note:</b> The <b>If-None-Match</b> header trumps the <b>If-Modified-Since</b> header.
        /// </remarks>
        public static void SetClientSideContentCacheExpiresHeaders(HttpRequest request, HttpResponse response, DateTime lastModified, DateTime expires, HttpCacheability cacheability, string entityTag)
        {
            SetClientSideContentCacheExpiresHeaders(request, response, new CacheValidator(lastModified, lastModified, entityTag), expires, cacheability);
        }

        /// <summary>
        /// Creates the appropriate HTTP client-side related cache headers for the content being delivered to the client.
        /// </summary>
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        /// <param name="expires">The UTC date time value to expire the <paramref name="cacheability"/> caching.</param>
        /// <param name="cacheability">Sets the <b>Cache-Control</b> header to one of the values of <see cref="HttpCacheability"/>.</param>
        /// <param name="isClientSideResourceCached">The function delegate that will determine whether a cached version of the content is found client-side.</param>
        /// <param name="headers">A collection of HTTP request headers.</param>
        /// <param name="statusCode">The status code.</param>
        /// <returns>A <see cref="WebHeaderCollection"/> instance that contains HTTP client-side related cache headers for the content being delivered to the client.</returns>
        /// <remarks>
        /// The <paramref name="validator"/> parameter is used to set the <b>Last-Modified</b> and <b>ETag</b> header in the response.
        /// <br/>
        /// <br/>
        /// The <paramref name="expires"/> parameter is used to set the <b>Expires</b> header in the response.
        /// <br/>
        /// <br/>
        /// The <paramref name="cacheability"/> parameter is used to guide and set the <b>Cache-Control</b> header and <b>Pragma</b> header in the response.
        /// </remarks>
        public static WebHeaderCollection CreateClientSideContentCacheExpiresHeaders(CacheValidator validator, DateTime expires, HttpCacheability cacheability, Doer<CacheValidator, NameValueCollection, bool> isClientSideResourceCached, NameValueCollection headers, out HttpStatusCode statusCode)
        {
            var factory = DoerFactory.Create(isClientSideResourceCached, validator, headers);
            return CreateClientSideContentCacheExpiresHeadersCore(factory, validator, expires, cacheability, out statusCode);
        }

        /// <summary>
        /// Creates the appropriate HTTP client-side related cache headers for the content being delivered to the client.
        /// </summary>
        /// <param name="lastModified">The UTC date time value of the content being delivered last modified time stamp.</param>
        /// <param name="expires">The UTC date time value to expire the <paramref name="cacheability"/> caching.</param>
        /// <param name="cacheability">Sets the <b>Cache-Control</b> header to one of the values of <see cref="HttpCacheability"/>.</param>
        /// <param name="isClientSideResourceCached">The function delegate that will determine whether a cached version of the content is found client-side.</param>
        /// <param name="headers">A collection of HTTP request headers.</param>
        /// <param name="statusCode">The status code.</param>
        /// <returns>A <see cref="WebHeaderCollection"/> instance that contains HTTP client-side related cache headers for the content being delivered to the client.</returns>
        /// <remarks>
        /// The <paramref name="lastModified"/> parameter is used to set the <b>Last-Modified</b> header in the response.
        /// <br/>
        /// <br/>
        /// The <paramref name="expires"/> parameter is used to set the <b>Expires</b> header in the response.
        /// <br/>
        /// <br/>
        /// The <paramref name="cacheability"/> parameter is used to guide and set the <b>Cache-Control</b> header and <b>Pragma</b> header in the response.
        /// </remarks>
        public static WebHeaderCollection CreateClientSideContentCacheExpiresHeaders(DateTime lastModified, DateTime expires, HttpCacheability cacheability, Doer<DateTime, NameValueCollection, bool> isClientSideResourceCached, NameValueCollection headers, out HttpStatusCode statusCode)
        {
            CacheValidator validator = new CacheValidator(lastModified);
            var factory = DoerFactory.Create(isClientSideResourceCached, lastModified, headers);
            return CreateClientSideContentCacheExpiresHeadersCore(factory, validator, expires, cacheability, out statusCode);
        }

        /// <summary>
        /// Creates the appropriate HTTP client-side related cache headers for the content being delivered to the client.
        /// </summary>
        /// <param name="lastModified">The UTC date time value of the content being delivered last modified time stamp.</param>
        /// <param name="expires">The UTC date time value to expire the <paramref name="cacheability"/> caching.</param>
        /// <param name="cacheability">Sets the <b>Cache-Control</b> header to one of the values of <see cref="HttpCacheability"/>.</param>
        /// <param name="isClientSideResourceCached">The function delegate that will determine whether a cached version of the content is found client-side.</param>
        /// <param name="headers">A collection of HTTP request headers.</param>
        /// <param name="entityTag">The server side entity tag to compare with the client side equivalent.</param>
        /// <param name="statusCode">The status code.</param>
        /// <returns>A <see cref="WebHeaderCollection"/> instance that contains HTTP client-side related cache headers for the content being delivered to the client.</returns>
        /// <remarks>
        /// The <paramref name="lastModified"/> parameter is used to set the <b>Last-Modified</b> header in the response.
        /// <br/>
        /// <br/>
        /// The <paramref name="expires"/> parameter is used to set the <b>Expires</b> header in the response.
        /// <br/>
        /// <br/>
        /// The <paramref name="cacheability"/> parameter is used to guide and set the <b>Cache-Control</b> header and <b>Pragma</b> header in the response.
        /// <br/>
        /// <br/>
        /// The <paramref name="entityTag"/> parameter is used to set the <b>ETag</b> header in the response.
        /// </remarks>
        public static WebHeaderCollection CreateClientSideContentCacheExpiresHeaders(DateTime lastModified, DateTime expires, HttpCacheability cacheability, Doer<DateTime, NameValueCollection, string, bool> isClientSideResourceCached, NameValueCollection headers, string entityTag, out HttpStatusCode statusCode)
        {
            CacheValidator validator = new CacheValidator(lastModified, lastModified, entityTag);
            var factory = DoerFactory.Create(isClientSideResourceCached, validator.GetMostSignificant(), headers, validator.Checksum);
            return CreateClientSideContentCacheExpiresHeadersCore(factory, validator, expires, cacheability, out statusCode);
        }

        private static WebHeaderCollection CreateClientSideContentCacheExpiresHeadersCore<TTuple>(DoerFactory<TTuple, bool> factory, CacheValidator validator, DateTime expires, HttpCacheability cacheability, out HttpStatusCode statusCode) where TTuple : Template
        {
            WebHeaderCollection headers = new WebHeaderCollection();
            DateTime currentUtcDate = DateTime.UtcNow;
            TimeSpan maxage = currentUtcDate >= expires ? TimeSpan.Zero : (expires - currentUtcDate);
            string entityTag = null;

            if (validator.Strength != ChecksumStrength.None)
            {
                entityTag = (validator.Strength == ChecksumStrength.Weak && !validator.Checksum.StartsWith("W/", StringComparison.OrdinalIgnoreCase)) ? string.Concat("W/", "\"", validator.Checksum, "\"") : string.Concat("\"", validator.Checksum, "\"");
            }

            bool suppressContent = factory.ExecuteMethod();

            StringBuilder cacheControl = new StringBuilder();
            switch (cacheability)
            {
                case HttpCacheability.Server:
                case HttpCacheability.NoCache:
                    cacheControl.Append("no-cache, no-store, ");
                    headers.Add(HttpResponseHeader.Pragma, "no-cache");
                    break;
                case HttpCacheability.ServerAndPrivate:
                case HttpCacheability.Private:
                    cacheControl.Append("private, ");
                    break;
                case HttpCacheability.Public:
                    cacheControl.Append("public, ");
                    break;
            }

            if (cacheability != HttpCacheability.NoCache)
            {
                if (expires > DateTime.MinValue)
                {
                    cacheControl.Append("must-revalidate, ");
                    cacheControl.AppendFormat("max-age={0}, ", ((long)maxage.TotalSeconds).ToString(CultureInfo.InvariantCulture));
                    headers.Add(HttpResponseHeader.Expires, expires.ToString("R", DateTimeFormatInfo.InvariantInfo));
                    if (!suppressContent) { headers.Add(HttpResponseHeader.LastModified, DateTimeUtility.GetLowestValue(currentUtcDate, validator.GetMostSignificant()).ToString("R", DateTimeFormatInfo.InvariantInfo)); }
                    if (validator.Strength != ChecksumStrength.None && entityTag != null) { headers.Add(HttpResponseHeader.ETag, entityTag); }
                }
            }
            else
            {
                headers.Add(HttpResponseHeader.Expires, expires.ToString("R", DateTimeFormatInfo.InvariantInfo));
            }

            cacheControl.Append("no-transform, ");
            headers.Add(HttpResponseHeader.CacheControl, cacheControl.ToString(0, cacheControl.Length - 2));
            statusCode = suppressContent ? HttpStatusCode.NotModified : HttpStatusCode.OK;
            return headers;
        }
    }
}