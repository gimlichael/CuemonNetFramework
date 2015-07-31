using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Hosting;
using Cuemon.Caching;
using Cuemon.IO;

namespace Cuemon.Web
{
	public partial class GlobalModule
	{
		/// <summary>
		/// Gets or sets a value indicating whether client-caching of static content is enabled. Default is false.
		/// </summary>
		/// <value>
		///   <c>true</c> if client-caching of static content is enabled; otherwise, <c>false</c>.
		/// </value>
		/// <remarks>Client caching is a matter of allowing content - static as well as dynamic - to be cached locally on the client visiting your website.</remarks>
		public static bool EnableStaticClientCaching
		{
		    get; set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether client-caching of dynamic content is enabled. Default is false.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if client-caching of dynamic content is enabled; otherwise, <c>false</c>.
		/// </value>
		public static bool EnableDynamicClientCaching
		{
		    get; set;
		}

		/// <summary>
		/// Handles the expires headers of dynamic content - as in where an associated <see cref="HttpContext.Handler"/> has been assigned (such as aspx, ashx, asmx and so forth).
		/// </summary>
		/// <param name="context">The context of the ASP.NET application.</param>
		/// <remarks>Expires on dynamic content is default set to 5 minutes in the future IF query string and form variables is empty.</remarks>
		protected void HandleDynamicContentExpiresHeaders(HttpApplication context)
		{
			if (context == null) { throw new ArgumentNullException("context"); }
			this.HandleDynamicContentExpiresHeaders(context, TimeSpan.FromMinutes(5));
		}

		/// <summary>
		/// Handles the expires headers of dynamic content - as in where an associated <see cref="HttpContext.Handler"/> has been assigned (such as aspx, ashx, asmx and so forth).
		/// </summary>
		/// <param name="context">The context of the ASP.NET application.</param>
		/// <param name="expiresInterval">The interval added to <see cref="DateTime.UtcNow"/> for a calculated HTTP Expires header.</param>
		/// <remarks>Default is <see cref="HttpCacheability.Private"/> caching.</remarks>
		protected void HandleDynamicContentExpiresHeaders(HttpApplication context, TimeSpan expiresInterval)
		{
			this.HandleDynamicContentExpiresHeaders(context, expiresInterval, HttpCacheability.Private);
		}

        /// <summary>
        /// Handles the expires headers of dynamic content - as in where an associated <see cref="HttpContext.Handler"/> has been assigned (such as aspx, ashx, asmx and so forth).
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <param name="expiresInterval">The interval added to <see cref="DateTime.UtcNow"/> for a calculated HTTP Expires header.</param>
        /// <param name="cacheability">Sets the <b>Cache-Control</b> header to one of the values of <see cref="HttpCacheability"/>.</param>
        protected void HandleDynamicContentExpiresHeaders(HttpApplication context, TimeSpan expiresInterval, HttpCacheability cacheability)
        {
            Validator.ThrowIfNull(context, "context");
            this.HandleDynamicContentExpiresHeaders(context, expiresInterval, cacheability, GetCacheValidator(context.Request, context.Context.Handler ?? context.Context.CurrentHandler));
        }

        /// <summary>
        /// Handles the expires headers of dynamic content - as in where an associated <see cref="HttpContext.Handler"/> has been assigned (such as aspx, ashx, asmx and so forth).
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <param name="expiresInterval">The interval added to <see cref="DateTime.UtcNow"/> for a calculated HTTP Expires header.</param>
        /// <param name="cacheability">Sets the <b>Cache-Control</b> header to one of the values of <see cref="HttpCacheability"/>.</param>
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        protected virtual void HandleDynamicContentExpiresHeaders(HttpApplication context, TimeSpan expiresInterval, HttpCacheability cacheability, CacheValidator validator)
        {
            Validator.ThrowIfNull(context, "context");
            if (context.Response.StatusCode >= 400) { return; }
            if (context.Response.StatusCode == (int)HttpStatusCode.MovedPermanently ||
                context.Response.StatusCode == (int)HttpStatusCode.Redirect) // do not compute expires headers on 301 or 302's
            {
                HttpResponseUtility.DisableClientSideResourceCache(context.Response);
                return;
            }

            if (context.Context.Handler == null || context.Context.CurrentHandler == null) { return; }
            if (context.Request.QueryString.Count == 0 && context.Request.Form.Count == 0 && context.Context.Error == null) // always fetch page if the client has query, form or error data
            {
                DateTime expires = DateTime.UtcNow.AddTicks(expiresInterval.Ticks); ;
                ISearchEngineOptimizer optimizer = context.Context.Handler as ISearchEngineOptimizer;
                if (optimizer != null)
                {
                    context.Response.AddHeader("X-Change-Frequency", optimizer.ChangeFrequency.ToString());
                    context.Response.AddHeader("X-Crawler-Priority", optimizer.CrawlerPriority.ToString("0.0", CultureInfo.InvariantCulture));
                }
                HttpResponseUtility.SetClientSideContentCacheExpiresHeaders(context.Request, context.Response, validator, expires, cacheability);
            }
            else
            {
                HttpResponseUtility.DisableClientSideResourceCache(context.Response);
            }
        }

        /// <summary>
        /// Handles the expires headers of static content.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>Expires on static content is default set to 7 days in the future. Also, since it can be costly to parse last modified on static files these are default cached for a 4-hours sliding cache expiration. <see cref="HttpCacheability.Private"/> is default. You can override this method and change the values to your needs.</remarks>
        protected void HandleStaticContentExpiresHeaders(HttpApplication context)
		{
			if (context == null) { throw new ArgumentNullException("context"); }
			this.HandleStaticContentExpiresHeaders(context, TimeSpan.FromDays(7));
		}

		/// <summary>
		/// Handles the expires headers of static content.
		/// </summary>
		/// <param name="context">The context of the ASP.NET application.</param>
		/// <param name="expiresInterval">The interval added to <see cref="DateTime.UtcNow"/> for a calculated HTTP Expires header.</param>
		/// <remarks>Default is <see cref="HttpCacheability.Private"/> caching.</remarks>
		protected void HandleStaticContentExpiresHeaders(HttpApplication context, TimeSpan expiresInterval)
		{
			this.HandleStaticContentExpiresHeaders(context, expiresInterval, HttpCacheability.Private);
		}

        /// <summary>
        /// Handles the expires headers of static content.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <param name="expiresInterval">The interval added to <see cref="DateTime.UtcNow"/> for a calculated HTTP Expires header.</param>
        /// <param name="cacheability">Sets the <b>Cache-Control</b> header to one of the values of <see cref="HttpCacheability"/>.</param>
        protected void HandleStaticContentExpiresHeaders(HttpApplication context, TimeSpan expiresInterval, HttpCacheability cacheability)
        {
            Validator.ThrowIfNull(context, "context");
            CacheValidator validator = GetCacheValidator(context.Request, context.Context.CurrentHandler ?? context.Context.Handler);
            this.HandleStaticContentExpiresHeaders(context, expiresInterval, cacheability, validator);
        }

        /// <summary>
        /// Handles the expires headers of static content.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <param name="expiresInterval">The interval added to <see cref="DateTime.UtcNow"/> for a calculated HTTP Expires header.</param>
        /// <param name="cacheability">Sets the <b>Cache-Control</b> header to one of the values of <see cref="HttpCacheability"/>.</param>
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        protected virtual void HandleStaticContentExpiresHeaders(HttpApplication context, TimeSpan expiresInterval, HttpCacheability cacheability, CacheValidator validator)
        {
            Validator.ThrowIfNull(context, "context");
            if (context.Response.StatusCode >= 400) { return; }
            if (context.Response.StatusCode == (int)HttpStatusCode.MovedPermanently ||
                context.Response.StatusCode == (int)HttpStatusCode.Redirect) // do not compute expires headers on 301 or 302's
            {
                HttpResponseUtility.DisableClientSideResourceCache(context.Response);
                return;
            }

            IHttpHandler handler = context.Context.CurrentHandler ?? context.Context.Handler;
            if (handler != null) { return; } // not static content

            DateTime expires = DateTime.UtcNow.AddTicks(expiresInterval.Ticks);
            if (Equals(validator, CacheValidator.Default)) { return; }

            HttpResponseUtility.SetClientSideContentCacheExpiresHeaders(context.Request, context.Response, validator, expires, cacheability);
        }

        internal static CacheValidator GetCacheValidator(HttpRequest request, IHttpHandler handler)
	    {
            CacheValidator validator = CacheValidator.Default;

            ISearchEngineOptimizer optimizer = handler as ISearchEngineOptimizer;
            ICacheableHttpHandler cacheable = handler as ICacheableHttpHandler;
            if (cacheable != null)
            {
                validator = cacheable.GetCacheValidator(); // always favor this above all
            }
            else if (optimizer != null)
            {
                validator = new CacheValidator(optimizer.LastModified);
            }
            else
            {
                string mappedPath = HostingEnvironment.MapPath(request.Url.AbsolutePath);
                bool isPhysicalFile = File.Exists(mappedPath);
                if (isPhysicalFile) { validator = FileUtility.GetCacheValidator(mappedPath); }
            }
	        return validator;
	    }
	}
}