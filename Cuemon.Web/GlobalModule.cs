using System;
using System.Net;
using System.Reflection;
using System.Web;
using Cuemon.Collections.Generic;
using Cuemon.Runtime.Caching;
using Cuemon.Web.Compilation;

namespace Cuemon.Web
{
    /// <summary>
    /// An <see cref="IHttpModule"/> substitute that shares and exceeds the old-school Global.asax implementation, while providing diagnostics capabilities.
    /// </summary>
    /// <remarks>
    /// For more information and a quick overview of the ASP.NET Application and Page Lifecycle, have a look here: http://files.meetup.com/1060099/asp.net_life_cycle_cheatsheet.pdf
    /// </remarks>
    public partial class GlobalModule : ApplicationEventBinderModule<GlobalModule>
    {
        /// <summary>
        /// Returns an object that can be used to synchronize access to the <see cref="GlobalModule"/>.
        /// </summary>
        protected static readonly object Locker = new object();

        private static readonly DateTime ModuleCreated = DateTime.UtcNow;
        internal const string CacheGroup = "Cuemon.GlobalModule";

        #region Properties
        /// <summary>
        /// Gets the assembly that is part of this ASP.NET application.
        /// </summary>
        /// <value>The assembly that is part of this ASP.NET application.</value>
        /// <remarks>This property is set just before <see cref="ApplicationEventBinderModule{T}.OnApplicationStart"/> is invoked. Because of the nature in ASP.NET this property may contain null.</remarks>
        public Assembly WebEntryAssembly { get; private set; }

        /// <summary>
        /// Gets a reference to the <see cref="CachingManager.Cache"/> object.
        /// </summary>
        public CacheCollection Cache
        {
            get { return CachingManager.Cache; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this module can run as a standalone website. Default is <c>false</c>.
        /// </summary>
        /// <value><c>true</c> if this this module can run as a standalone website; otherwise, <c>false</c>.</value>
        /// <remarks>This property will instruct the module to use internal methods optimized for a standalone website.</remarks>
        public static bool IsStandaloneWebsite { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the elapsed time expressed as a <see cref="TimeSpan"/> since the application pool, where this <see cref="GlobalModule"/> is residing, was started.
        /// </summary>
        public static TimeSpan GetAppPoolUptime()
        {
            return (DateTime.UtcNow - ModuleCreated);
        }

        /// <summary>
        /// Discovers the assembly that is part of this ASP.NET application and assign the value to <see cref="WebEntryAssembly"/>.
        /// </summary>
	    protected virtual void DiscoverWebEntryAssembly()
        {
            Type handlerType = EnumerableUtility.FirstOrDefault(CompilationUtility.GetReferencedHandlerTypes());
            Type moduleType = EnumerableUtility.FirstOrDefault(CompilationUtility.GetReferencedModuleTypes());
            WebEntryAssembly = (handlerType == null ? null : handlerType.Assembly) ?? (moduleType == null ? null : moduleType.Assembly);
        }

        /// <summary>
        /// Provides access to the ApplicationStart event that occurs when an AppPool is first started.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked only once as the first event in the HTTP pipeline chain of execution when ASP.NET responds to a request.</remarks>
        protected override void OnApplicationStart(HttpApplication context)
        {
            base.OnApplicationStart(context);
            DiscoverWebEntryAssembly();
        }

        /// <summary>
        /// Provides access to the BeginRequest event of the <see cref="HttpApplication"/>.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked as the first event in the HTTP pipeline chain of execution when ASP.NET responds to a request.</remarks>
        protected override void OnBeginRequest(HttpApplication context)
        {
            base.OnBeginRequest(context);
            context.Response.DisableKernelCache(); // strange side effect might occur if we don't do this
        }

        /// <summary>
        /// Provides access to the PostResolveRequestCache event of the <see cref="HttpApplication"/>.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked when ASP.NET bypasses execution of the current event handler and allows a caching module to serve a request from the cache.</remarks>
        protected override void OnPostResolveRequestCache(HttpApplication context)
        {
            base.OnPostResolveRequestCache(context);
            HandleUrlRouting(context);
            InitializeCompression(context, HttpRequestUtility.ParseAcceptEncoding(context.Request));
        }

        /// <summary>
        /// Provides access to the PreRequestHandlerExecute event of the <see cref="HttpApplication"/>.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked just before ASP.NET starts executing an event handler.</remarks>
        protected override void OnPreRequestHandlerExecute(HttpApplication context)
        {
            base.OnPreRequestHandlerExecute(context);
            if (context.Response.StatusCode == (int)HttpStatusCode.MovedPermanently ||
                context.Response.StatusCode == (int)HttpStatusCode.Redirect)
            {
                HttpResponseUtility.DisableClientSideResourceCache(context.Response);
                return;
            }

            if (EnableTokenParsingForClientCaching)
            {
                if (context.Response.BufferOutput) { context.Response.Filter = new HttpResponseContentFilter(context); }
            }
        }

        /// <summary>
        /// Provides access to the PreSendRequestHeaders event of the <see cref="HttpApplication"/>.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked just before ASP.NET sends HTTP headers to the client.</remarks>
		protected override void OnPreSendRequestHeaders(HttpApplication context)
        {
            base.OnPreSendRequestHeaders(context);
            if (EnableTokenParsingForClientCaching) { HandleHtmlRelatedContentParsing(context); }
            if (EnableDynamicClientCaching)
            {
                if (!HttpRequestUtility.IsStandaloneServerLocal(context.Request))
                {
                    HandleDynamicContentExpiresHeaders(context);
                }
            }
            if (EnableStaticClientCaching)
            {
                if (!HttpRequestUtility.IsStandaloneServerLocal(context.Request))
                {
                    HandleStaticContentExpiresHeaders(context);
                }
            }
            if (EnableExceptionToXmlInterception) { HandleExceptionInterception(context); }
            HandleCompressionHeaders(context);
        }

        internal static void CheckForHttpContextAvailability()
        {
            if (HttpContext.Current == null) { throw new InvalidOperationException("HttpContext is unavailable."); }
        }
        #endregion
    }
}