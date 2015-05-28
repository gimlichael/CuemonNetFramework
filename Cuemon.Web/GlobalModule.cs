using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using System.Web.UI;
using Cuemon.Caching;
using Cuemon.Collections.Generic;
using Cuemon.Reflection;

namespace Cuemon.Web
{
	/// <summary>
	/// An <see cref="IHttpModule"/> substitute that shares and exceeds the old-school Global.asax implementation, while providing diagnostics capabilities.
	/// </summary>
	/// <remarks>
	/// For more information and a quick overview of the ASP.NET Application and Page Lifecycle, have a look here: http://files.meetup.com/1060099/asp.net_life_cycle_cheatsheet.pdf
	/// </remarks>
	public partial class GlobalModule
	{
		/// <summary>
		/// Returns an object that can be used to synchronize access to the <see cref="GlobalModule"/>.
		/// </summary>
		protected static readonly object Locker = new object();

		private static bool _hasInitialized;
        private static bool _hasUnloaded;
		private static readonly DateTime ModuleCreated = DateTime.UtcNow;
		private readonly IDictionary<string, TimeSpan> _measuredApplicationLifecycle = new Dictionary<string, TimeSpan>();
	    internal const string CacheGroup = "Cuemon.GlobalModule";

		#region Properties
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

		/// <summary>
		/// Gets a value indicating whether this instance benefits from the IIS integrated pipeline mode.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance can benefit from the IIS integrated pipeline mode; otherwise, <c>false</c>.
		/// </value>
		public static bool HasIisIntegratedPipelineMode
		{
			get { return _hasIisIntegratedPipelineMode; }
		}

		/// <summary>
		/// Gets the elapsed time expressed as a <see cref="TimeSpan"/> since the application pool, where this <see cref="GlobalModule"/> is residing, was started.
		/// </summary>
		public TimeSpan AppPoolUptime
		{
			get { return (DateTime.UtcNow - ModuleCreated); }
		}

		/// <summary>
		/// Gets the measured application life cycle.
		/// </summary>
		protected IReadOnlyDictionary<string, TimeSpan> MeasuredApplicationLifecycle
		{
			get { return new ReadOnlyDictionary<string, TimeSpan>(_measuredApplicationLifecycle); }
		}

		private void AddOrUpdateMeasuredApplicationLifecycle(string key, TimeSpan value)
		{
            TraceWrite(key.Remove(0, 2), value);
            if (_measuredApplicationLifecycle.ContainsKey(key))
			{
                _measuredApplicationLifecycle[key] = value;
				return;
			}
            _measuredApplicationLifecycle.Add(key, value);
		}
		#endregion

		#region Methods
		/// <summary>
        /// Provides access to the ApplicationStart event that occurs when an AppPool is first started.
		/// </summary>
		/// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked only once as the first event in the HTTP pipeline chain of execution when ASP.NET responds to a request.</remarks>
		protected virtual void OnApplicationStart(HttpApplication context)
		{
			if (context == null) { throw new ArgumentNullException("context"); }
		}

        /// <summary>
        /// Discovers the assembly that is part of this ASP.NET application and assign the value to <see cref="WebEntryAssembly"/>.
        /// </summary>
	    protected virtual void DiscoverWebEntryAssembly()
	    {
            Type handlerType = EnumerableUtility.FirstOrDefault(GetReferencedHandlerTypes());
            Type moduleType = EnumerableUtility.FirstOrDefault(GetReferencedModuleTypes());
            this.WebEntryAssembly = (handlerType == null ? null : handlerType.Assembly) ?? (moduleType == null ? null : moduleType.Assembly);	        
	    }

		/// <summary>
		/// Provides access to the ApplicationEnd event that occurs when an AppPool is either forced stopped or simply crashed.
		/// </summary>
        /// <param name="domain">The application domain of the ASP.NET application.</param>
        /// <remarks>This method is invoked when the <paramref name="domain"/> is about to be unloaded.</remarks>
		protected virtual void OnApplicationEnd(AppDomain domain)
		{
            if (domain == null) { throw new ArgumentNullException("domain"); }
		}

        /// <summary>
        /// Provides access to the Disposed event of the <see cref="HttpApplication"/> control.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked when the application is disposed.</remarks>
        protected virtual void OnDisposed(HttpApplication context)
        {
            if (context == null) { throw new ArgumentNullException("context"); }
        }

        /// <summary>
        /// Provides access to the BeginRequest event of the <see cref="HttpApplication"/> control.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked as the first event in the HTTP pipeline chain of execution when ASP.NET responds to a request.</remarks>
		protected virtual void OnBeginRequest(HttpApplication context)
		{
			if (context == null) { throw new ArgumentNullException("context"); }
			context.Response.DisableKernelCache(); // strange side effect might occur if we don't do this
		}

        /// <summary>
        /// Provides access to the AuthenticateRequest event of the <see cref="HttpApplication"/> control.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked when a security module is establishing the identity of the user.</remarks>
		protected virtual void OnAuthenticateRequest(HttpApplication context)
		{
			if (context == null) { throw new ArgumentNullException("context"); }
		}

        /// <summary>
        /// Provides access to the PostAuthenticateRequest event of the <see cref="HttpApplication"/> control.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked when a security module has established the identity of the user.</remarks>
		protected virtual void OnPostAuthenticateRequest(HttpApplication context)
		{
			if (context == null) { throw new ArgumentNullException("context"); }
		}

        /// <summary>
        /// Provides access to the AuthorizeRequest event of the <see cref="HttpApplication"/> control.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked when a security module has verified user authorization.</remarks>
		protected virtual void OnAuthorizeRequest(HttpApplication context)
		{
			if (context == null) { throw new ArgumentNullException("context"); }
		}

        /// <summary>
        /// Provides access to the PostAuthorizeRequest event of the <see cref="HttpApplication"/> control.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked when the user for the current request has been authorized.</remarks>
		protected virtual void OnPostAuthorizeRequest(HttpApplication context)
		{
			if (context == null) { throw new ArgumentNullException("context"); }
		}

        /// <summary>
        /// Provides access to the ResolveRequestCache event of the <see cref="HttpApplication"/> control.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked when ASP.NET finishes an authorization event to let the caching modules serve requests from the cache, bypassing execution of the event handler.</remarks>
		protected virtual void OnResolveRequestCache(HttpApplication context)
		{
			if (context == null) { throw new ArgumentNullException("context"); }
		}

        /// <summary>
        /// Provides access to the PostResolveRequestCache event of the <see cref="HttpApplication"/> control.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked when ASP.NET bypasses execution of the current event handler and allows a caching module to serve a request from the cache.</remarks>
		protected virtual void OnPostResolveRequestCache(HttpApplication context)
		{
            if (context == null) { throw new ArgumentNullException("context"); }
            this.InitializeCompression(context, HttpRequestUtility.ParseAcceptEncoding(context.Request));
		}

        /// <summary>
        /// Provides access to the MapRequestHandler event of the <see cref="HttpApplication"/> control.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked when the handler is selected to respond to the request.</remarks>
		protected virtual void OnMapRequestHandler(HttpApplication context)
		{
			if (context == null) { throw new ArgumentNullException("context"); }
		}

		/// <summary>
        /// Provides access to the PostMapRequestHandler event of the <see cref="HttpApplication"/> control.
		/// </summary>
		/// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked when ASP.NET has mapped the current request to the appropriate event handler.</remarks>
        protected virtual void OnPostMapRequestHandler(HttpApplication context)
		{
			if (context == null) { throw new ArgumentNullException("context"); }
		}

		/// <summary>
        /// Provides access to the AcquireRequestState event of the <see cref="HttpApplication"/> control.
		/// </summary>
		/// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked when when ASP.NET acquires the current state hat is associated with the current request.</remarks>
        protected virtual void OnAcquireRequestState(HttpApplication context)
		{
			if (context == null) { throw new ArgumentNullException("context"); }
		}

		/// <summary>
        /// Provides access to the PostAcquireRequestState event of the <see cref="HttpApplication"/> control.
		/// </summary>
		/// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked when the request state that is associated with the current request has been obtained.</remarks>
		protected virtual void OnPostAcquireRequestState(HttpApplication context)
		{
			if (context == null) { throw new ArgumentNullException("context"); }
		}

		/// <summary>
		/// Provides access to the PreRequestHandlerExecute event of the <see cref="HttpApplication"/> control.
		/// </summary>
		/// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked just before ASP.NET starts executing an event handler.</remarks>
		protected virtual void OnPreRequestHandlerExecute(HttpApplication context)
		{
			if (context == null) { throw new ArgumentNullException("context"); }
            if (context.Response.StatusCode == (int)HttpStatusCode.MovedPermanently ||
                context.Response.StatusCode == (int)HttpStatusCode.Redirect)
            {
                HttpResponseUtility.DisableClientSideResourceCache(context.Response);
                return;
            }

            if (GlobalModule.EnableTokenParsingForClientCaching)
            {
                if (context.Response.BufferOutput) { context.Response.Filter = new HttpResponseContentFilter(context); }
            }
		}

        /// <summary>
        /// Provides access to the PreInit event of the currently executing <see cref="Page"/> control.
        /// </summary>
        /// <param name="page">The page handler of the current request.</param>
        /// <remarks>This method is invoked at the beginning of <paramref name="page"/> initialization.</remarks>
        protected virtual void OnPreInit(Page page)
        {
            if (page == null) { throw new ArgumentNullException("page"); }
        }

        /// <summary>
        /// Provides access to the Init event of the currently executing <see cref="Page"/> control.
        /// </summary>
        /// <param name="page">The page handler of the current request.</param>
        /// <remarks>This method is invoked when the <paramref name="page"/> is initialized, which is the first step in its lifecycle.</remarks>
        protected virtual void OnInit(Page page)
        {
            if (page == null) { throw new ArgumentNullException("page"); }
        }

        /// <summary>
        /// Provides access to the InitComplete event of the currently executing <see cref="Page"/> control.
        /// </summary>
        /// <param name="page">The page handler of the current request.</param>
        /// <remarks>This method is invoked when <paramref name="page"/> initialization is complete.</remarks>
        protected virtual void OnInitComplete(Page page)
        {
            if (page == null) { throw new ArgumentNullException("page"); }
        }

        /// <summary>
        /// Provides access to the PreLoad event of the currently executing <see cref="Page"/> control.
        /// </summary>
        /// <param name="page">The page handler of the current request.</param>
        /// <remarks>This method is invoked just before the <paramref name="page"/> load event.</remarks>
        protected virtual void OnPreLoad(Page page)
        {
            if (page == null) { throw new ArgumentNullException("page"); }
        }

        /// <summary>
        /// Provides access to the Load event of the currently executing <see cref="Page"/> control.
        /// </summary>
        /// <param name="page">The page handler of the current request.</param>
        /// <remarks>This method is invoked when the <paramref name="page"/> loads.</remarks>
        protected virtual void OnLoad(Page page)
        {
            if (page == null) { throw new ArgumentNullException("page"); }
        }

        /// <summary>
        /// Provides access to the LoadComplete event of the currently executing <see cref="Page"/> control.
        /// </summary>
        /// <param name="page">The page handler of the current request.</param>
        /// <remarks>This method is invoked at the end of the load stage of the <paramref name="page"/> life cycle.</remarks>
        protected virtual void OnLoadComplete(Page page)
        {
            if (page == null) { throw new ArgumentNullException("page"); }
        }

        /// <summary>
        /// Provides access to the PreRender event of the currently executing <see cref="Page"/> control.
        /// </summary>
        /// <param name="page">The page handler of the current request.</param>
        /// <remarks>This method is invoked after the <paramref name="page"/> is loaded but prior to rendering.</remarks>
        protected virtual void OnPreRender(Page page)
        {
            if (page == null) { throw new ArgumentNullException("page"); }
        }

        /// <summary>
        /// Provides access to the PreRenderComplete event of the currently executing <see cref="Page"/> control.
        /// </summary>
        /// <param name="page">The page handler of the current request.</param>
        /// <remarks>This method is invoked before the <paramref name="page"/> content is rendered.</remarks>
        protected virtual void OnPreRenderComplete(Page page)
        {
            if (page == null) { throw new ArgumentNullException("page"); }
        }

        /// <summary>
        /// Provides access to the SaveStateComplete event of the currently executing <see cref="Page"/> control.
        /// </summary>
        /// <param name="page">The page handler of the current request.</param>
        /// <remarks>This method is invoked after the <paramref name="page"/> has completed saving all view state and control state information for the <paramref name="page"/> and controls on the <paramref name="page"/>.</remarks>
        protected virtual void OnSaveStateComplete(Page page)
        {
            if (page == null) { throw new ArgumentNullException("page"); }
        }

        /// <summary>
        /// Provides access to the SaveStateComplete event of the currently executing <see cref="Page"/> control.
        /// </summary>
        /// <param name="page">The page handler of the current request.</param>
        /// <remarks>This method is invoked when the <paramref name="page"/> is unloaded from memory.</remarks>
        protected virtual void OnUnload(Page page)
        {
            if (page == null) { throw new ArgumentNullException("page"); }
        }

        /// <summary>
        /// Provides access to the PostRequestHandlerExecute event of the <see cref="HttpApplication"/> control.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked just before ASP.NET starts executing an event handler.</remarks>
		protected virtual void OnPostRequestHandlerExecute(HttpApplication context)
		{
			if (context == null) { throw new ArgumentNullException("context"); }
		}

        /// <summary>
        /// Provides access to the ReleaseRequestState event of the <see cref="HttpApplication"/> control.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked after ASP.NET finishes executing all request event handlers. This event causes state modules to save the current state data.</remarks>
		protected virtual void OnReleaseRequestState(HttpApplication context)
		{
			if (context == null) { throw new ArgumentNullException("context"); }
		}

        /// <summary>
        /// Provides access to the PostReleaseRequestState event of the <see cref="HttpApplication"/> control.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked when ASP.NET has completed executing all request event handlers and the request state data has been stored.</remarks>
		protected virtual void OnPostReleaseRequestState(HttpApplication context)
		{
			if (context == null) { throw new ArgumentNullException("context"); }
		}

        /// <summary>
        /// Provides access to the UpdateRequestCache event of the <see cref="HttpApplication"/> control.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked when ASP.NET finishes executing an event handler in order to let caching modules store responses that will be used to serve subsequent requests from the cache.</remarks>
		protected virtual void OnUpdateRequestCache(HttpApplication context)
		{
			if (context == null) { throw new ArgumentNullException("context"); }
		}

        /// <summary>
        /// Provides access to the PostUpdateRequestCache event of the <see cref="HttpApplication"/> control.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked when ASP.NET finishes updating caching modules and storing responses that are used to serve subsequent requests from the cache.</remarks>
		protected virtual void OnPostUpdateRequestCache(HttpApplication context)
		{
			if (context == null) { throw new ArgumentNullException("context"); }
		}

        /// <summary>
        /// Provides access to the LogRequest event of the <see cref="HttpApplication"/> control.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked just before ASP.NET performs any logging for the current request.</remarks>
		protected virtual void OnLogRequest(HttpApplication context)
		{
			if (context == null) { throw new ArgumentNullException("context"); }
		}

        /// <summary>
        /// Provides access to the PostLogRequest event of the <see cref="HttpApplication"/> control.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked when ASP.NET has completed processing all the event handlers for the LogRequest event.</remarks>
		protected virtual void OnPostLogRequest(HttpApplication context)
		{
			if (context == null) { throw new ArgumentNullException("context"); }
		}

        /// <summary>
        /// Provides access to the EndRequest event of the <see cref="HttpApplication"/> control.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked as the last event in the HTTP pipeline chain of execution when ASP.NET responds to a request.</remarks>
		protected virtual void OnEndRequest(HttpApplication context)
		{
			if (context == null) { throw new ArgumentNullException("context"); }
		}

        /// <summary>
        /// Provides access to the PreSendRequestHeaders event of the <see cref="HttpApplication"/> control.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked just before ASP.NET sends HTTP headers to the client.</remarks>
		protected virtual void OnPreSendRequestHeaders(HttpApplication context)
		{
			if (context == null) { throw new ArgumentNullException("context"); }
            if (GlobalModule.EnableTokenParsingForClientCaching) { this.HandleHtmlRelatedContentParsing(context); }
            if (GlobalModule.EnableDynamicClientCaching)
            {
                if (!HttpRequestUtility.IsStandaloneServerLocal(context.Request))
                {
                    this.HandleDynamicContentExpiresHeaders(context);
                }
            }
            if (GlobalModule.EnableStaticClientCaching)
            {
                if (!HttpRequestUtility.IsStandaloneServerLocal(context.Request))
                {
                    this.HandleStaticContentExpiresHeaders(context);
                }
            }
            if (GlobalModule.EnableExceptionToXmlInterception) { this.HandleExceptionInterception(context); }
            this.HandleCompressionHeaders(context);
		}

        /// <summary>
        /// Provides access to the PreSendRequestContent event of the <see cref="HttpApplication"/> control.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked just before ASP.NET sends content to the client.</remarks>
		protected virtual void OnPreSendRequestContent(HttpApplication context)
		{
			if (context == null) { throw new ArgumentNullException("context"); }
		}

        /// <summary>
        /// Provides access to the SendRequestContent event of the <see cref="GlobalModule"/> class.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked just before ASP.NET sends content to the client but after the processing of <see cref="OnPreSendRequestContent"/>.</remarks>
        protected virtual void OnSendRequestContent(HttpApplication context)
        {
            if (context == null) { throw new ArgumentNullException("context"); }
        }

		/// <summary>
        /// Provides access to the Error event of the <see cref="HttpApplication"/> control.
		/// </summary>
		/// <param name="context">The context of the ASP.NET application.</param>
		/// <param name="lastException">The last captured exception.</param>
        /// <remarks>This method is invoked when an unhandled exception is thrown.</remarks>
		protected virtual void OnError(HttpApplication context, Exception lastException)
		{
			if (context == null) { throw new ArgumentNullException("context"); }
		}



	    internal static void CheckForHttpContextAvailability()
	    {
	        if (HttpContext.Current == null) { throw new InvalidOperationException("HttpContext is unavailable."); }
	    }

        /// <summary>
        /// Gets all referenced types matching the specified <paramref name="filter"/>.
        /// </summary>
        /// <param name="filter">The <see cref="Type"/> that must be present in the inheritance chain.</param>
        /// <returns>An <see cref="IReadOnlyCollection{Type}"/> containing all <paramref name="filter"/> implemented types of this ASP.NET application.</returns>
        /// <remarks><see cref="Type"/> from assemblies starting with <b>Cuemon</b>, <b>System</b> or <b>Microsoft</b> is excluded from the result.</remarks>
	    public static IReadOnlyCollection<Type> GetReferencedTypes(Type filter)
	    {
	        return GetReferencedTypes(filter, "Cuemon", "System", "Microsoft");
	    }

        /// <summary>
        /// Gets all referenced types matching the specified <paramref name="filter"/>.
        /// </summary>
        /// <param name="filter">The <see cref="Type"/> that must be present in the inheritance chain.</param>
        /// <param name="excludeAssembliesStartingWith">A sequence of assemblies to exclude from the result by matching the beginning of each string in the sequence.</param>
        /// <returns>An <see cref="IReadOnlyCollection{Type}"/> containing all <paramref name="filter"/> implemented types of this ASP.NET application.</returns>
        public static IReadOnlyCollection<Type> GetReferencedTypes(Type filter, params string[] excludeAssembliesStartingWith)
        {
            List<Type> handlers = new List<Type>();
            try
            {
                ICollection assemblies = BuildManager.GetReferencedAssemblies();
                foreach (Assembly assembly in assemblies)
                {
                    if (AddReferenceType(assembly, excludeAssembliesStartingWith))
                    {
                        handlers.AddRange(ReflectionUtility.GetAssemblyTypes(assembly, null, filter));
                    }
                }

                IEnumerable<Assembly> temporaryAssemblies = AppDomain.CurrentDomain.GetAssemblies(); // we need this for Temporary ASP.NET files
                foreach (Assembly assembly in temporaryAssemblies)
                {
                    if (AddReferenceType(assembly, excludeAssembliesStartingWith))
                    {
                        handlers.AddRange(ReflectionUtility.GetAssemblyTypes(assembly, null, filter));
                    }
                }

                handlers = new List<Type>(EnumerableUtility.Distinct(handlers));
            }
            catch (Exception)
            {
            }
            return new ReadOnlyCollection<Type>(handlers);
        }

	    private static bool AddReferenceType(Assembly assembly, string[] excludeAssembliesStartingWith)
	    {
	        if (excludeAssembliesStartingWith == null) { return true; }
            for (int i = 0; i < excludeAssembliesStartingWith.Length; i++)
            {
                if (assembly.FullName.StartsWith(excludeAssembliesStartingWith[i], StringComparison.Ordinal))
                {
                    return false;
                }
            }
	        return true;
	    }

        /// <summary>
        /// Gets all referenced <see cref="IHttpHandler"/> types of this ASP.NET application.
        /// </summary>
        /// <returns>An <see cref="IReadOnlyCollection{Type}"/> containing all <see cref="IHttpHandler"/> implemented types of this ASP.NET application.</returns>
        /// <remarks><see cref="IHttpHandler"/> implementations from assemblies starting with <b>Cuemon</b>, <b>System</b> or <b>Microsoft</b> is excluded from the result.</remarks>
	    public static IReadOnlyCollection<Type> GetReferencedHandlerTypes()
	    {
            Doer<Type, IReadOnlyCollection<Type>> getReferenceTypes = CachingManager.Cache.Memoize<Type, IReadOnlyCollection<Type>>(GetReferencedTypes);
            return getReferenceTypes(typeof(IHttpHandler));
	    }

        /// <summary>
        /// Gets all referenced <see cref="IHttpModule"/> types of this ASP.NET application.
        /// </summary>
        /// <returns>An <see cref="IReadOnlyCollection{Type}"/> containing all <see cref="IHttpModule"/> implemented types of this ASP.NET application.</returns>
        /// <remarks><see cref="IHttpHandler"/> implementations from assemblies starting with <b>Cuemon</b>, <b>System</b> or <b>Microsoft</b> is excluded from the result.</remarks>
        public static IReadOnlyCollection<Type> GetReferencedModuleTypes()
        {
            Doer<Type, IReadOnlyCollection<Type>> getReferenceTypes = CachingManager.Cache.Memoize<Type, IReadOnlyCollection<Type>>(GetReferencedTypes);
            return getReferenceTypes(typeof(IHttpModule));
        }
	    #endregion
    }
}