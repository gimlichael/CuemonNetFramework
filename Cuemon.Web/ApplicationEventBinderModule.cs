using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Web;
using Cuemon.Diagnostics;
using Cuemon.Reflection;

namespace Cuemon.Web
{
    /// <summary>
    /// Provides access to <see cref="HttpApplication"/> specific events and beyond.
    /// </summary>
    public abstract class ApplicationEventBinderModule<T> : IHttpModule, ITimeMeasuring where T : ApplicationEventBinderModule<T>
    {
        private static readonly object PadLock = new object();
        private static volatile bool _supportsIisIntegratedPipelineMode = true;
        private EventHandler<EventArgs> _sendRequestContent;

        #region Properties
        private static bool HasUnloaded { get; set; }

        private static bool HasInitialized { get; set; }

        /// <summary>
        /// Gets a value indicating whether the ASP.NET lifecycle associated <see cref="HttpResponse"/> has its headers written prematurely because of IIS adaptive error lifecycle violation.
        /// </summary>
        /// <value><c>true</c> if the ASP.NET lifecycle associated <see cref="HttpResponse"/> has its headers written prematurely because of IIS adaptive error lifecycle violation; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// Unfortunately some engineers at the Microsoft .NET Team and IIS Team decided to implement an internal UseAdaptiveError feature.<br/>
        /// What this does is that if an error code server side is &gt;400 and &lt;600 the resulting status code to your code is translated to 200 and headers as well as content is transferred automatically by IIS bypassing the otherwise well established ASP.NET Life Cycle.<br/><br/>
        /// IMO this is as much a deal breaker as it is a design flaw as one should unconditionally be able to trust the ASP.NET Lifecycle.<br/><br/>
        /// Headers to the client should NEVER (and let me emphasis that) NEVER be send until after <see cref="OnPreSendRequestHeaders"/> (which is equivalent to <see cref="HttpApplication.PreSendRequestHeaders"/>) just as client content should be sent AFTER <see cref="OnPreSendRequestContent"/> (which is equivalent to <see cref="HttpApplication.PreSendRequestContent"/>).
        /// </remarks>
        public bool IsHeadersWrittenBecauseOfIisAdaptiveErrorLifecycleViolation { get; private set; }

        /// <summary>
        /// Gets the current handler (if any) of <see cref="OnPreRequestHandlerExecute"/>.
        /// </summary>
        /// <value>The current handler (if any) of <see cref="OnPreRequestHandlerExecute"/>.</value>
        public IHttpHandler CurrentHandler { get; private set; }

        /// <summary>
        /// Gets the timer that is used to accurately measure application lifecycle elapsed time.
        /// </summary>
        /// <value>The timer that is used to accurately measure application lifecycle elapsed time.</value>
        public Stopwatch Timer { get; private set; }

        /// <summary>
        /// Gets or sets the callback delegate for the application lifecycle measured elapsed time.
        /// </summary>
        /// <value>The callback delegate for the application lifecycle measured elapsed time.</value>
        public Act<string, TimeSpan> TimeMeasuringCallback { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether application lifecycle time measuring is enabled.
        /// </summary>
        /// <value><c>true</c> if application lifecycle time measuring is enabled; otherwise, <c>false</c>.</value>
        public bool EnableTimeMeasuring { get; set; }
        #endregion

        #region Methods
        void IHttpModule.Init(HttpApplication context)
        {
            Validator.ThrowIfNull(context, nameof(context));
            context.BeginRequest += OnBeginRequestWrapper;
            context.AuthenticateRequest += OnAuthenticateRequestWrapper;
            context.PostAuthenticateRequest += OnPostAuthenticateRequestWrapper;
            context.AuthorizeRequest += OnAuthorizeRequestWrapper;
            context.PostAuthorizeRequest += OnPostAuthorizeRequestWrapper;
            context.ResolveRequestCache += OnResolveRequestCacheWrapper;
            context.PostResolveRequestCache += OnPostResolveRequestCacheWrapper;
            context.PostMapRequestHandler += OnPostMapRequestHandlerWrapper;
            context.AcquireRequestState += OnAcquireRequestStateWrapper;
            context.PostAcquireRequestState += OnPostAcquireRequestStateWrapper;
            context.PreRequestHandlerExecute += OnPreRequestHandlerExecuteWrapper;
            context.PostRequestHandlerExecute += OnPostRequestHandlerExecuteWrapper;
            context.ReleaseRequestState += OnReleaseRequestStateWrapper;
            context.PostReleaseRequestState += OnPostReleaseRequestStateWrapper;
            context.UpdateRequestCache += OnUpdateRequestCacheWrapper;
            context.PostUpdateRequestCache += OnPostUpdateRequestCacheWrapper;
            context.EndRequest += OnEndRequestWrapper;
            context.PreSendRequestHeaders += OnPreSendRequestHeadersWrapper;
            context.PreSendRequestContent += OnPreSendRequestContentWrapper;
            context.Error += OnErrorWrapper;
            context.Disposed += OnDisposedWrapper;
            AppDomain.CurrentDomain.DomainUnload += OnDomainUnloadWrapper;
            SendRequestContent += OnSendRequestContentWrapper;
            if (_supportsIisIntegratedPipelineMode)
            {
                try
                {
                    context.MapRequestHandler += OnMapRequestHandlerWrapper;
                    context.LogRequest += OnLogRequestWrapper;
                    context.PostLogRequest += OnPostLogRequestWrapper;
                }
                catch (Exception)
                {
                    _supportsIisIntegratedPipelineMode = false;
                    HttpRuntimeUtility.SupportsIisIntegratedPipelineMode = _supportsIisIntegratedPipelineMode;
                }
            }
        }

        private void OnAuthenticateRequestWrapper(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            OnAuthenticateRequest(application);
            ApplicationLifecycleTracer(application, "OnAuthenticateRequest");
        }

        private void OnPostAuthenticateRequestWrapper(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            OnPostAuthenticateRequest(application);
            ApplicationLifecycleTracer(application, "OnPostAuthenticateRequest");
        }

        private void OnAuthorizeRequestWrapper(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            OnAuthorizeRequest(application);
            ApplicationLifecycleTracer(application, "OnAuthorizeRequest");
        }

        private void OnPostAuthorizeRequestWrapper(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            OnPostAuthorizeRequest(application);
            ApplicationLifecycleTracer(application, "OnPostAuthorizeRequest");
        }

        private void OnResolveRequestCacheWrapper(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            OnResolveRequestCache(application);
            ApplicationLifecycleTracer(application, "OnResolveRequestCache");
        }

        private void OnPostMapRequestHandlerWrapper(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            OnPostMapRequestHandler(application);
            ApplicationLifecycleTracer(application, "OnPostMapRequestHandler");
        }

        private void OnAcquireRequestStateWrapper(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            OnAcquireRequestState(application);
            ApplicationLifecycleTracer(application, "OnAcquireRequestState");
        }

        private void OnPostAcquireRequestStateWrapper(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            OnPostAcquireRequestState(application);
            ApplicationLifecycleTracer(application, "OnPostAcquireRequestState");
        }

        private void OnPostRequestHandlerExecuteWrapper(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            OnPostRequestHandlerExecute(application);
            ApplicationLifecycleTracer(application, "OnPostRequestHandlerExecute");
        }

        private void OnPostReleaseRequestStateWrapper(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            OnPostReleaseRequestState(application);
            ApplicationLifecycleTracer(application, "OnPostReleaseRequestState");
        }

        private void OnUpdateRequestCacheWrapper(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            OnUpdateRequestCache(application);
            ApplicationLifecycleTracer(application, "OnUpdateRequestCache");
        }

        private void OnPreSendRequestHeadersWrapper(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            OnPreSendRequestHeaders(application);
            ApplicationLifecycleTracer(application, "OnPreSendRequestHeaders");
        }

        private void OnPreSendRequestContentWrapper(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            OnPreSendRequestContent(application);
            ApplicationLifecycleTracer(application, "OnPreSendRequestContent");
            EventUtility.Raise(_sendRequestContent, application, new EventArgs());
        }

        private void OnErrorWrapper(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            if (application != null) { OnError(application, application.Server.GetLastError()); }
            ApplicationLifecycleTracer(application, "OnError");
        }

        private void OnDisposedWrapper(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            OnDisposed(application);
            ApplicationLifecycleTracer(application, "OnDisposed");
        }

        private void OnMapRequestHandlerWrapper(object sender, EventArgs e)
        {
            if (!HttpRuntimeUtility.SupportsIisIntegratedPipelineMode) { return; }
            HttpApplication application = sender as HttpApplication;
            OnMapRequestHandler(application);
            ApplicationLifecycleTracer(application, "OnMapRequestHandler");
        }

        private void OnLogRequestWrapper(object sender, EventArgs e)
        {
            if (!HttpRuntimeUtility.SupportsIisIntegratedPipelineMode) { return; }
            HttpApplication application = sender as HttpApplication;
            OnLogRequest(application);
            ApplicationLifecycleTracer(application, "OnLogRequest");
        }

        private void OnPostLogRequestWrapper(object sender, EventArgs e)
        {
            if (!HttpRuntimeUtility.SupportsIisIntegratedPipelineMode) { return; }
            HttpApplication application = sender as HttpApplication;
            OnPostLogRequest(application);
            ApplicationLifecycleTracer(application, "OnPostLogRequest");
        }

        private void OnPostUpdateRequestCacheWrapper(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            OnPostUpdateRequestCache(application);
            ApplicationLifecycleTracer(application, "OnPostUpdateRequestCache");
        }

        private void OnEndRequestWrapper(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            OnEndRequest(application);
            ApplicationLifecycleTracer(application, "OnEndRequest");
        }

        private void OnPostResolveRequestCacheWrapper(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            OnPostResolveRequestCache(application);
            ApplicationLifecycleTracer(application, "OnPostResolveRequestCache");
        }

        private void OnReleaseRequestStateWrapper(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            OnReleaseRequestState(application);
            DetectIisAdaptiveErrorViolation(application);
            ApplicationLifecycleTracer(application, "OnReleaseRequestState");
        }

        private void OnPreRequestHandlerExecuteWrapper(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            if (application != null &&
                application.Context != null &&
                application.Context.CurrentHandler != null) { CurrentHandler = application.Context.CurrentHandler; }
            OnPreRequestHandlerExecute(application);
            ApplicationLifecycleTracer(application, "OnPreRequestHandlerExecute");
        }

        private void OnBeginRequestWrapper(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            Timer = Stopwatch.StartNew();
            if (!HasInitialized)
            {
                lock (PadLock)
                {
                    if (!HasInitialized)
                    {
                        try
                        {
                            OnApplicationStart(application);
                        }
                        finally
                        {
                            HasInitialized = true;
                        }
                    }
                }
            }
            OnBeginRequest(application);
            ApplicationLifecycleTracer(application, "OnBeginRequest");
        }

        private void OnDomainUnloadWrapper(object sender, EventArgs eventArgs)
        {
            AppDomain domain = sender as AppDomain;
            if (domain != null) { domain.DomainUnload -= OnDomainUnloadWrapper; }
            if (!HasUnloaded)
            {
                lock (PadLock)
                {
                    if (!HasUnloaded)
                    {
                        try
                        {
                            OnApplicationEnd(domain);
                            ApplicationLifecycleTracer(null, "OnApplicationEnd");
                        }
                        finally
                        {
                            HasUnloaded = true;
                        }
                    }
                }
            }
        }

        private void OnSendRequestContentWrapper(object sender, EventArgs eventArgs)
        {
            HttpApplication application = sender as HttpApplication;
            SendRequestContent -= OnSendRequestContentWrapper;
            OnSendRequestContent(application);

            ApplicationLifecycleTracer(application, "OnSendRequestContent");
        }

        private void ApplicationLifecycleTracer(HttpApplication application, string lifecycleEvent)
        {
            if (!EnableTimeMeasuring) { return; }
            bool applicationIsUnavailable = (application == null || application.Context == null || application.Request == null);
            IHttpHandler httpHandler = applicationIsUnavailable ? null : application.Context.CurrentHandler ?? application.Context.Handler;
            string handler = string.Format(CultureInfo.InvariantCulture, "{0}", httpHandler == null ? "" : StringConverter.FromType(httpHandler.GetType(), true));
            string location = applicationIsUnavailable ? "" : string.Format(CultureInfo.InvariantCulture, "{0} [{1}]", application.Request.CurrentExecutionFilePath, HttpRequestUtility.RawUrl(application.Request).AbsolutePath);
            string optionalHandlerWithLocation = applicationIsUnavailable ? "" : string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", handler, handler.Length == 0 ? "" : " -> ", location);
            Condition.Initialize(TupleUtility.CreateTwo(string.Format(CultureInfo.InvariantCulture, "Application:{0}{1}{2}", lifecycleEvent, applicationIsUnavailable ? "" : " -> ", optionalHandlerWithLocation), Timer.Elapsed), TimeMeasuringCallback, Condition.IsNull)
                .Invoke(tuple => Infrastructure.TraceWriteLifecycleEvent(tuple.Arg1, tuple.Arg2), tuple => TimeMeasuringCallback(tuple.Arg1, tuple.Arg2));
        }

        private void DetectIisAdaptiveErrorViolation(HttpApplication context)
        {
            if (context == null) { return; }
            PropertyInfo headersWritten = ReflectionUtility.GetProperty(context.Response.GetType(), "HeadersWritten", typeof(bool), null, ReflectionUtility.BindingInstancePublicAndPrivateNoneInherited);
            if (headersWritten != null)
            {
                IsHeadersWrittenBecauseOfIisAdaptiveErrorLifecycleViolation = (bool)headersWritten.GetValue(context.Response, null);
            }
        }

        /// <summary>
        /// Provides access to the BeginRequest event of the <see cref="HttpApplication"/>.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked as the first event in the HTTP pipeline chain of execution when ASP.NET responds to a request.</remarks>
        protected virtual void OnBeginRequest(HttpApplication context)
        {
            Validator.ThrowIfNull(context, nameof(context));
        }

        /// <summary>
        /// Provides access to the AuthenticateRequest event of the <see cref="HttpApplication"/>.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked when a security module is establishing the identity of the user.</remarks>
        protected virtual void OnAuthenticateRequest(HttpApplication context)
        {
            Validator.ThrowIfNull(context, nameof(context));
        }

        /// <summary>
        /// Provides access to the PostAuthenticateRequest event of the <see cref="HttpApplication"/>.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked when a security module has established the identity of the user.</remarks>
        protected virtual void OnPostAuthenticateRequest(HttpApplication context)
        {
            Validator.ThrowIfNull(context, nameof(context));
        }

        /// <summary>
        /// Provides access to the AuthorizeRequest event of the <see cref="HttpApplication"/>.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked when a security module has verified user authorization.</remarks>
        protected virtual void OnAuthorizeRequest(HttpApplication context)
        {
            Validator.ThrowIfNull(context, nameof(context));
        }

        /// <summary>
        /// Provides access to the PostAuthorizeRequest event of the <see cref="HttpApplication"/>.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked when the user for the current request has been authorized.</remarks>
        protected virtual void OnPostAuthorizeRequest(HttpApplication context)
        {
            Validator.ThrowIfNull(context, nameof(context));
        }

        /// <summary>
        /// Provides access to the ResolveRequestCache event of the <see cref="HttpApplication"/>.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked when ASP.NET finishes an authorization event to let the caching modules serve requests from the cache, bypassing execution of the event handler.</remarks>
        protected virtual void OnResolveRequestCache(HttpApplication context)
        {
            Validator.ThrowIfNull(context, nameof(context));
        }


        /// <summary>
        /// Provides access to the PostResolveRequestCache event of the <see cref="HttpApplication"/>.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked when ASP.NET bypasses execution of the current event handler and allows a caching module to serve a request from the cache.</remarks>
        protected virtual void OnPostResolveRequestCache(HttpApplication context)
        {
            Validator.ThrowIfNull(context, nameof(context));
        }

        /// <summary>
        /// Provides access to the MapRequestHandler event of the <see cref="HttpApplication"/>.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked when the handler is selected to respond to the request.</remarks>
        protected virtual void OnMapRequestHandler(HttpApplication context)
        {
            Validator.ThrowIfNull(context, nameof(context));
        }

        /// <summary>
        /// Provides access to the PostMapRequestHandler event of the <see cref="HttpApplication"/>.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked when ASP.NET has mapped the current request to the appropriate event handler.</remarks>
        protected virtual void OnPostMapRequestHandler(HttpApplication context)
        {
            Validator.ThrowIfNull(context, nameof(context));
        }

        /// <summary>
        /// Provides access to the AcquireRequestState event of the <see cref="HttpApplication"/>.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked when when ASP.NET acquires the current state hat is associated with the current request.</remarks>
        protected virtual void OnAcquireRequestState(HttpApplication context)
        {
            Validator.ThrowIfNull(context, nameof(context));
        }

        /// <summary>
        /// Provides access to the PostAcquireRequestState event of the <see cref="HttpApplication"/>.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked when the request state that is associated with the current request has been obtained.</remarks>
        protected virtual void OnPostAcquireRequestState(HttpApplication context)
        {
            Validator.ThrowIfNull(context, nameof(context));
        }

        /// <summary>
        /// Provides access to the PreRequestHandlerExecute event of the <see cref="HttpApplication"/>.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked just before ASP.NET starts executing an event handler.</remarks>
        protected virtual void OnPreRequestHandlerExecute(HttpApplication context)
        {
            Validator.ThrowIfNull(context, nameof(context));
        }

        /// <summary>
        /// Provides access to the PostRequestHandlerExecute event of the <see cref="HttpApplication"/>.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked just before ASP.NET starts executing an event handler.</remarks>
        protected virtual void OnPostRequestHandlerExecute(HttpApplication context)
        {
            Validator.ThrowIfNull(context, nameof(context));
        }

        /// <summary>
        /// Provides access to the ReleaseRequestState event of the <see cref="HttpApplication"/>.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked after ASP.NET finishes executing all request event handlers. This event causes state modules to save the current state data.</remarks>
        protected virtual void OnReleaseRequestState(HttpApplication context)
        {
            Validator.ThrowIfNull(context, nameof(context));
        }

        /// <summary>
        /// Provides access to the PostReleaseRequestState event of the <see cref="HttpApplication"/>.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked when ASP.NET has completed executing all request event handlers and the request state data has been stored.</remarks>
		protected virtual void OnPostReleaseRequestState(HttpApplication context)
        {
            Validator.ThrowIfNull(context, nameof(context));
        }

        /// <summary>
        /// Provides access to the UpdateRequestCache event of the <see cref="HttpApplication"/>.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked when ASP.NET finishes executing an event handler in order to let caching modules store responses that will be used to serve subsequent requests from the cache.</remarks>
		protected virtual void OnUpdateRequestCache(HttpApplication context)
        {
            Validator.ThrowIfNull(context, nameof(context));
        }

        /// <summary>
        /// Provides access to the PostUpdateRequestCache event of the <see cref="HttpApplication"/>.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked when ASP.NET finishes updating caching modules and storing responses that are used to serve subsequent requests from the cache.</remarks>
		protected virtual void OnPostUpdateRequestCache(HttpApplication context)
        {
            Validator.ThrowIfNull(context, nameof(context));
        }

        /// <summary>
        /// Provides access to the LogRequest event of the <see cref="HttpApplication"/>.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked just before ASP.NET performs any logging for the current request.</remarks>
		protected virtual void OnLogRequest(HttpApplication context)
        {
            Validator.ThrowIfNull(context, nameof(context));
        }

        /// <summary>
        /// Provides access to the PostLogRequest event of the <see cref="HttpApplication"/>.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked when ASP.NET has completed processing all the event handlers for the LogRequest event.</remarks>
		protected virtual void OnPostLogRequest(HttpApplication context)
        {
            Validator.ThrowIfNull(context, nameof(context));
        }

        /// <summary>
        /// Provides access to the EndRequest event of the <see cref="HttpApplication"/>.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked as the last event in the HTTP pipeline chain of execution when ASP.NET responds to a request.</remarks>
		protected virtual void OnEndRequest(HttpApplication context)
        {
            Validator.ThrowIfNull(context, nameof(context));
        }

        /// <summary>
        /// Provides access to the PreSendRequestHeaders event of the <see cref="HttpApplication"/>.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked just before ASP.NET sends HTTP headers to the client.</remarks>
		protected virtual void OnPreSendRequestHeaders(HttpApplication context)
        {
            Validator.ThrowIfNull(context, nameof(context));
        }

        /// <summary>
        /// Provides access to the PreSendRequestContent event of the <see cref="HttpApplication"/>.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked just before ASP.NET sends content to the client.</remarks>
		protected virtual void OnPreSendRequestContent(HttpApplication context)
        {
            Validator.ThrowIfNull(context, nameof(context));
        }

        /// <summary>
        /// Provides access to the SendRequestContent event of the <see cref="GlobalModule"/>.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked just before ASP.NET sends content to the client but after the processing of <see cref="OnPreSendRequestContent"/>.</remarks>
        protected virtual void OnSendRequestContent(HttpApplication context)
        {
            Validator.ThrowIfNull(context, nameof(context));
        }

        /// <summary>
        /// Provides access to the Error event of the <see cref="HttpApplication"/>.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <param name="lastException">The last captured exception.</param>
        /// <remarks>This method is invoked when an unhandled exception is thrown.</remarks>
        protected virtual void OnError(HttpApplication context, Exception lastException)
        {
            Validator.ThrowIfNull(context, nameof(context));
        }

        /// <summary>
        /// Provides access to the ApplicationStart event that occurs when an AppPool is first started.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked only once as the first event in the HTTP pipeline chain of execution when ASP.NET responds to a request.</remarks>
        protected virtual void OnApplicationStart(HttpApplication context)
        {
            Validator.ThrowIfNull(context, nameof(context));
        }

        /// <summary>
        /// Provides access to the ApplicationEnd event that occurs when an AppPool is either forced stopped or simply crashed.
        /// </summary>
        /// <param name="domain">The application domain of the ASP.NET application.</param>
        /// <remarks>This method is invoked when the <paramref name="domain"/> is about to be unloaded.</remarks>
        protected virtual void OnApplicationEnd(AppDomain domain)
        {
            Validator.ThrowIfNull(domain, nameof(domain));
        }

        /// <summary>
        /// Provides access to the Disposed event of the <see cref="HttpApplication"/>.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked when the application is disposed.</remarks>
        protected virtual void OnDisposed(HttpApplication context)
        {
            Validator.ThrowIfNull(context, nameof(context));
        }

        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"/>.
        /// </summary>
        public virtual void Dispose()
        {
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs after the PreSendRequestContent event of the <see cref="HttpApplication"/>.
        /// </summary>
        public event EventHandler<EventArgs> SendRequestContent
        {
            add
            {
                EventUtility.AddEvent(value, ref _sendRequestContent);
            }
            remove
            {
                EventUtility.RemoveEvent(value, ref _sendRequestContent);
            }
        }
        #endregion
    }
}