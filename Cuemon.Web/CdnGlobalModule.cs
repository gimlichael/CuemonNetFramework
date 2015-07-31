using System;
using System.Text.RegularExpressions;
using System.Web;
using Cuemon.Caching;

namespace Cuemon.Web
{
    /// <summary>
    /// A <see cref="GlobalModule"/> implementation that is tweaked for a Content Delivery Network (CDN) role with ASP.NET as the runtime platform.
    /// </summary>
    public class CdnGlobalModule : GlobalModule
    {
        private static readonly Regex CompiledMd5PatternExpression = new Regex("([a-f0-9]{32})", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Provides access to the ApplicationStart event that occurs when an AppPool is first started.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked only once as the first event in the HTTP pipeline chain of execution when ASP.NET responds to a request.</remarks>
        protected override void OnApplicationStart(HttpApplication context)
        {
            base.OnApplicationStart(context);
            EnableTokenParsingForClientCaching = true;
        }

        /// <summary>
        /// Provides access to the AuthorizeRequest event of the <see cref="HttpApplication"/> control.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked when a security module has verified user authorization.</remarks>
        protected override void OnAuthorizeRequest(HttpApplication context)
        {
            if (EnableTokenParsingForClientCaching) { this.HandleTokenParsingUrlRouting(context); }
        }

        /// <summary>
        ///  Handles the URL routing of the result from the query-string token parsing.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        protected virtual void HandleTokenParsingUrlRouting(HttpApplication context)
        {
            if (context == null) { throw new ArgumentNullException("context"); }
            if (!IsHtmlRelatedContent(context)) { return; }
            string rawUrl = context.Request.RawUrl;
            Match match = CompiledMd5PatternExpression.Match(rawUrl);
            if (match.Success)
            {
                //HttpResponseUtility.RedirectPermanently(rawUrl.Replace(match.Value, "")); // consider redirect
                context.Context.RewritePath(rawUrl.Replace(match.Value, ""), false);
            }
        }

        /// <summary>
        /// Handles the expires headers of static content.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <param name="expiresInterval">The interval added to <see cref="DateTime.UtcNow"/> for a calculated HTTP Expires header.</param>
        /// <param name="cacheability">Sets the <b>Cache-Control</b> header to one of the values of <see cref="HttpCacheability"/>.</param>
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        protected override void HandleStaticContentExpiresHeaders(HttpApplication context, TimeSpan expiresInterval, HttpCacheability cacheability, CacheValidator validator)
        {
            base.HandleStaticContentExpiresHeaders(context, expiresInterval, HttpCacheability.Public, validator);
        }

        /// <summary>
        /// Handles the expires headers of dynamic content - as in where an associated <see cref="HttpContext.Handler"/> has been assigned (such as aspx, ashx, asmx and so forth).
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <param name="expiresInterval">The interval added to <see cref="DateTime.UtcNow"/> for a calculated HTTP Expires header.</param>
        /// <param name="cacheability">Sets the <b>Cache-Control</b> header to one of the values of <see cref="HttpCacheability"/>.</param>
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        protected override void HandleDynamicContentExpiresHeaders(HttpApplication context, TimeSpan expiresInterval, HttpCacheability cacheability, CacheValidator validator)
        {
            base.HandleDynamicContentExpiresHeaders(context, expiresInterval, HttpCacheability.Public, validator);
        }

        /// <summary>
        /// Provides access to the PreRequestHandlerExecute event of the <see cref="HttpApplication"/> control.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked just before ASP.NET starts executing an event handler.</remarks>
        protected override void OnPreRequestHandlerExecute(HttpApplication context)
        {
            if (context == null) { throw new ArgumentNullException("context"); }
        }

        /// <summary>
        /// Provides access to the PreSendRequestHeaders event of the <see cref="HttpApplication"/> control.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked just before ASP.NET sends HTTP headers to the client.</remarks>
        protected override void OnPreSendRequestHeaders(HttpApplication context)
        {
            if (context == null) { throw new ArgumentNullException("context"); }
            if (EnableDynamicClientCaching)
            {
                if (!HttpRequestUtility.IsStandaloneServerLocal(context.Request))
                {
                    this.HandleDynamicContentExpiresHeaders(context);
                }
            }
            if (EnableStaticClientCaching)
            {
                if (!HttpRequestUtility.IsStandaloneServerLocal(context.Request))
                {
                    this.HandleStaticContentExpiresHeaders(context);
                }
            }
            this.HandleCompressionHeaders(context);
            context.Response.AppendHeader("Access-Control-Allow-Origin", "*");
        }
    }
}
