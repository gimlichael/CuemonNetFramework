using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Security.Principal;
using System.Web;
using System.Web.UI;
using Cuemon.Reflection;

namespace Cuemon.Web
{
    public partial class GlobalModule : IHttpModule
    {
        private bool _isHeadersWrittenDoToIisAdaptiveErrorLifecycleViolation;
        private static bool _hasIisIntegratedPipelineMode = true;
        private EventHandler<EventArgs> _sendRequestContent = null;
        internal static WindowsIdentity AppPoolIdentity;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalModule"/> class.
        /// </summary>
        protected GlobalModule()
        {
            this.ApplicationLifecycle = Stopwatch.StartNew();
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs after the PreSendRequestContent event of the <see cref="HttpApplication"/> control.
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

        #region Properties
        private Stopwatch ApplicationLifecycle { get; set; }

        /// <summary>
        /// Gets the assembly that is part of this ASP.NET application.
        /// </summary>
        /// <value>The assembly that is part of this ASP.NET application.</value>
        /// <remarks>This property is set just before <see cref="OnApplicationStart"/> is invoked. Because of the nature in ASP.NET this property may contain null.</remarks>
        public Assembly WebEntryAssembly { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Initializes a module and prepares it to handle requests.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpApplication"></see> that provides access to the methods, properties, and events common to all application objects within an ASP.NET application</param>
        public virtual void Init(HttpApplication context)
        {
            if (context == null) throw new ArgumentNullException("context");

            context.BeginRequest += new EventHandler(HttpApplication_BeginRequest);
            context.AuthenticateRequest += new EventHandler(HttpApplication_AuthenticateRequest);
            context.PostAuthenticateRequest += new EventHandler(HttpApplication_PostAuthenticateRequest);
            context.AuthorizeRequest += new EventHandler(HttpApplication_AuthorizeRequest);
            context.PostAuthorizeRequest += new EventHandler(HttpApplication_PostAuthorizeRequest);
            context.ResolveRequestCache += new EventHandler(HttpApplication_ResolveRequestCache);
            context.PostResolveRequestCache += new EventHandler(HttpApplication_PostResolveRequestCache);
            context.PostMapRequestHandler += new EventHandler(HttpApplication_PostMapRequestHandler);
            context.AcquireRequestState += new EventHandler(HttpApplication_AcquireRequestState);
            context.PostAcquireRequestState += new EventHandler(HttpApplication_PostAcquireRequestState);
            context.PreRequestHandlerExecute += new EventHandler(HttpApplication_PreRequestHandlerExecute);
            context.PostRequestHandlerExecute += new EventHandler(HttpApplication_PostRequestHandlerExecute);
            context.ReleaseRequestState += new EventHandler(HttpApplication_ReleaseRequestState);
            context.PostReleaseRequestState += new EventHandler(HttpApplication_PostReleaseRequestState);
            context.UpdateRequestCache += new EventHandler(HttpApplication_UpdateRequestCache);
            context.PostUpdateRequestCache += new EventHandler(HttpApplication_PostUpdateRequestCache);
            context.EndRequest += new EventHandler(HttpApplication_EndRequest);
            context.PreSendRequestHeaders += new EventHandler(HttpApplication_PreSendRequestHeaders);
            context.PreSendRequestContent += new EventHandler(HttpApplication_PreSendRequestContent);
            context.Error += new EventHandler(HttpApplication_Error);
            context.Disposed += new EventHandler(HttpApplication_Disposed);
            AppDomain.CurrentDomain.DomainUnload += new EventHandler(AppPool_Stopped);
            this.SendRequestContent += new EventHandler<EventArgs>(GlobalModule_SendRequestContent);

            if (!_hasIisIntegratedPipelineMode) { return; }
            try
            {
                context.MapRequestHandler += new EventHandler(HttpApplication_MapRequestHandler);
                context.LogRequest += new EventHandler(HttpApplication_LogRequest);
                context.PostLogRequest += new EventHandler(HttpApplication_PostLogRequest);
            }
            catch (Exception)
            {
                _hasIisIntegratedPipelineMode = false;
            }
        }

        #region Private Eventhooks
        /// <summary>
        /// Handles the EndRequest event of the HttpApplication control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void HttpApplication_EndRequest(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            this.OnEndRequest(application);
            this.AddOrUpdateMeasuredApplicationLifecycle("OnEndRequest", this.ApplicationLifecycle.Elapsed);
        }

        /// <summary>
        /// Handles the PostLogRequest event of the HttpApplication control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void HttpApplication_PostLogRequest(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            this.OnPostLogRequest(application);
            if (HasIisIntegratedPipelineMode)
            {
                this.AddOrUpdateMeasuredApplicationLifecycle("OnPostLogRequest", this.ApplicationLifecycle.Elapsed);
            }
        }

        /// <summary>
        /// Handles the LogRequest event of the HttpApplication control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void HttpApplication_LogRequest(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            this.OnLogRequest(application);
            if (HasIisIntegratedPipelineMode)
            {
                this.AddOrUpdateMeasuredApplicationLifecycle("OnLogRequest", this.ApplicationLifecycle.Elapsed);
            }
        }

        /// <summary>
        /// Handles the PostUpdateRequestCache event of the HttpApplication control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void HttpApplication_PostUpdateRequestCache(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            this.OnPostUpdateRequestCache(application);
            this.AddOrUpdateMeasuredApplicationLifecycle("OnPostUpdateRequestCache", this.ApplicationLifecycle.Elapsed);
        }

        /// <summary>
        /// Handles the UpdateRequestCache event of the HttpApplication control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void HttpApplication_UpdateRequestCache(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            this.OnUpdateRequestCache(application);
            this.AddOrUpdateMeasuredApplicationLifecycle("OnUpdateRequestCache", this.ApplicationLifecycle.Elapsed);
        }

        /// <summary>
        /// Handles the ReleaseRequestState event of the HttpApplication control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void HttpApplication_ReleaseRequestState(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            this.OnReleaseRequestState(application);
            this.DetectIisAdaptiveErrorViolation(application);
            this.AddOrUpdateMeasuredApplicationLifecycle("OnReleaseRequestState", this.ApplicationLifecycle.Elapsed);
        }

        /// <summary>
        /// Handles the PostRequestHandlerExecute event of the HttpApplication control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void HttpApplication_PostRequestHandlerExecute(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            this.OnPostRequestHandlerExecute(application);
            this.AddOrUpdateMeasuredApplicationLifecycle("OnPostRequestHandlerExecute", this.ApplicationLifecycle.Elapsed);
        }

        /// <summary>
        /// Handles the AcquireRequestState event of the HttpApplication control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void HttpApplication_AcquireRequestState(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            this.OnAcquireRequestState(application);
            this.AddOrUpdateMeasuredApplicationLifecycle("OnAcquireRequestState", this.ApplicationLifecycle.Elapsed);
        }

        /// <summary>
        /// Handles the MapRequestHandler event of the HttpApplication control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void HttpApplication_MapRequestHandler(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            this.OnMapRequestHandler(application);
            if (HasIisIntegratedPipelineMode)
            {
                this.AddOrUpdateMeasuredApplicationLifecycle("OnMapRequestHandler", this.ApplicationLifecycle.Elapsed);
            }
        }

        /// <summary>
        /// Handles the ResolveRequestCache event of the HttpApplication control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void HttpApplication_ResolveRequestCache(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            this.OnResolveRequestCache(application);
            this.AddOrUpdateMeasuredApplicationLifecycle("OnResolveRequestCache", this.ApplicationLifecycle.Elapsed);
        }

        /// <summary>
        /// Handles the PostAuthorizeRequest event of the HttpApplication control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void HttpApplication_PostAuthorizeRequest(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            this.OnPostAuthorizeRequest(application);
            this.AddOrUpdateMeasuredApplicationLifecycle("OnPostAuthorizeRequest", this.ApplicationLifecycle.Elapsed);
        }

        /// <summary>
        /// Handles the PostAuthenticateRequest event of the HttpApplication control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void HttpApplication_PostAuthenticateRequest(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            this.OnPostAuthenticateRequest(application);
            this.AddOrUpdateMeasuredApplicationLifecycle("OnPostAuthenticateRequest", this.ApplicationLifecycle.Elapsed);
        }

        /// <summary>
        /// Handles the AuthenticateRequest event of the HttpApplication control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void HttpApplication_AuthenticateRequest(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            this.OnAuthenticateRequest(application);
            this.AddOrUpdateMeasuredApplicationLifecycle("OnAuthenticateRequest", this.ApplicationLifecycle.Elapsed);
        }

        /// <summary>
        /// Handles the BeginRequest event of the HttpApplication control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void HttpApplication_BeginRequest(object sender, EventArgs e)
        {
            _measuredApplicationLifecycle.Clear();
            this.ApplicationLifecycle.Reset();
            this.ApplicationLifecycle.Start();
            HttpApplication application = sender as HttpApplication;
            if (!_hasInitialized)
            {
                lock (Locker)
                {
                    if (!_hasInitialized)
                    {
                        try
                        {
                            AppPoolIdentity = WindowsIdentity.GetCurrent();
                            this.OnApplicationStart(application);
                        }
                        finally
                        {
                            _hasInitialized = true;
                        }
                    }
                }
            }
            this.IsHeadersWrittenDoToIisAdaptiveErrorLifecycleViolation = false; // we need to catch fails by the IIS team (such as sending headers BEFORE the PreSendRequestHeaders)
            this.OnBeginRequest(application);
            this.AddOrUpdateMeasuredApplicationLifecycle("OnBeginRequest", this.ApplicationLifecycle.Elapsed);
        }

        /// <summary>
        /// Handles the DomainUnload event of the AppDomain.CurrentDomain control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void AppPool_Stopped(object sender, EventArgs e)
        {
            AppDomain domain = sender as AppDomain;
            domain.DomainUnload -= new EventHandler(AppPool_Stopped);
            if (!_hasUnloaded)
            {
                lock (Locker)
                {
                    if (!_hasUnloaded)
                    {
                        _hasUnloaded = true;
                        this.OnApplicationEnd(domain);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Disposed event of the HttpApplication control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void HttpApplication_Disposed(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            this.OnDisposed(application);
        }

        /// <summary>
        /// Handles the PreSendRequestContent event of the HttpApplication control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void HttpApplication_PreSendRequestContent(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            this.OnPreSendRequestContent(application);
            this.AddOrUpdateMeasuredApplicationLifecycle("OnPreSendRequestContent", this.ApplicationLifecycle.Elapsed);
            EventUtility.Raise(_sendRequestContent, application, new EventArgs());
        }

        /// <summary>
        /// Handles the SendRequestContent event of the GlobalModule control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="eventArgs">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void GlobalModule_SendRequestContent(object sender, EventArgs eventArgs)
        {
            HttpApplication application = sender as HttpApplication;
            this.SendRequestContent -= new EventHandler<EventArgs>(GlobalModule_SendRequestContent);
            this.OnSendRequestContent(application);
        }

        /// <summary>
        /// Handles the PreRequestHandlerExecute event of the HttpApplication control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void HttpApplication_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            if (application != null &&
                application.Context != null &&
                application.Context.CurrentHandler != null)
            {
                IHttpHandler handler = application.Context.CurrentHandler;
                Page page = handler as Page;
                if (page != null)
                {
                    page.PreInit += Page_PreInit;
                    page.Init += Page_Init;
                    page.InitComplete += Page_InitComplete;
                    page.PreLoad += Page_PreLoad;
                    page.Load += Page_Load;
                    page.LoadComplete += Page_LoadComplete;
                    page.PreRender += Page_PreRender;
                    page.PreRenderComplete += Page_PreRenderComplete;
                    page.SaveStateComplete += Page_SaveStateComplete;
                    page.Unload += Page_Unload;
                }
            }
            this.OnPreRequestHandlerExecute(application);
            this.AddOrUpdateMeasuredApplicationLifecycle("OnPreRequestHandlerExecute", this.ApplicationLifecycle.Elapsed);
        }

        /// <summary>
        /// Handles the PreInit event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Page_PreInit(object sender, EventArgs e)
        {
            Page page = sender as Page;
            page.PreInit -= Page_PreInit;

            this.OnPreInit(page);
            this.AddOrUpdateMeasuredApplicationLifecycle("OnPreInit", this.ApplicationLifecycle.Elapsed);
        }

        /// <summary>
        /// Handles the Init event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Page_Init(object sender, EventArgs e)
        {
            Page page = sender as Page;
            page.Init -= Page_Init;

            this.OnInit(page);
            this.AddOrUpdateMeasuredApplicationLifecycle("OnInit", this.ApplicationLifecycle.Elapsed);
        }

        /// <summary>
        /// Handles the InitComplete event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Page_InitComplete(object sender, EventArgs e)
        {
            Page page = sender as Page;
            page.InitComplete -= Page_InitComplete;

            this.OnInitComplete(page);
            this.AddOrUpdateMeasuredApplicationLifecycle("OnInitComplete", this.ApplicationLifecycle.Elapsed);
        }

        /// <summary>
        /// Handles the PreLoad event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Page_PreLoad(object sender, EventArgs e)
        {
            Page page = sender as Page;
            page.PreLoad -= Page_PreLoad;

            this.OnPreLoad(page);
            this.AddOrUpdateMeasuredApplicationLifecycle("OnPreLoad", this.ApplicationLifecycle.Elapsed);
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Page_Load(object sender, EventArgs e)
        {
            Page page = sender as Page;
            page.Load -= Page_Load;

            this.OnLoad(page);
            this.AddOrUpdateMeasuredApplicationLifecycle("OnLoad", this.ApplicationLifecycle.Elapsed);
        }

        /// <summary>
        /// Handles the LoadComplete event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Page_LoadComplete(object sender, EventArgs e)
        {
            Page page = sender as Page;
            page.LoadComplete -= Page_LoadComplete;

            this.OnLoadComplete(page);
            this.AddOrUpdateMeasuredApplicationLifecycle("OnLoadComplete", this.ApplicationLifecycle.Elapsed);
        }

        /// <summary>
        /// Handles the PreRender event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Page_PreRender(object sender, EventArgs e)
        {
            Page page = sender as Page;
            page.PreRender -= Page_PreRender;

            this.OnPreRender(page);
            this.AddOrUpdateMeasuredApplicationLifecycle("OnPreRender", this.ApplicationLifecycle.Elapsed);
        }

        /// <summary>
        /// Handles the PreRenderComplete event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Page_PreRenderComplete(object sender, EventArgs e)
        {
            Page page = sender as Page;
            page.PreRenderComplete -= Page_PreRenderComplete;

            this.OnPreRenderComplete(page);
            this.AddOrUpdateMeasuredApplicationLifecycle("OnPreRenderComplete", this.ApplicationLifecycle.Elapsed);
        }

        /// <summary>
        /// Handles the SaveStateComplete event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Page_SaveStateComplete(object sender, EventArgs e)
        {
            Page page = sender as Page;
            page.SaveStateComplete -= Page_SaveStateComplete;

            this.OnSaveStateComplete(page);
            this.AddOrUpdateMeasuredApplicationLifecycle("OnSaveStateComplete", this.ApplicationLifecycle.Elapsed);
        }

        /// <summary>
        /// Handles the Unload event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Page_Unload(object sender, EventArgs e)
        {
            Page page = sender as Page;
            page.Unload -= Page_Unload;

            this.OnUnload(page);
            this.AddOrUpdateMeasuredApplicationLifecycle("OnUnload", this.ApplicationLifecycle.Elapsed);
        }

        /// <summary>
        /// Handles the PreSendRequestHeaders event of the HttpApplication control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void HttpApplication_PreSendRequestHeaders(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            this.OnPreSendRequestHeaders(application);
            this.AddOrUpdateMeasuredApplicationLifecycle("OnPreSendRequestHeaders", this.ApplicationLifecycle.Elapsed);
        }

        /// <summary>
        /// Handles the PostReleaseRequestState event of the HttpApplication control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void HttpApplication_PostReleaseRequestState(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            this.OnPostReleaseRequestState(application);
            this.AddOrUpdateMeasuredApplicationLifecycle("OnPostReleaseRequestState", this.ApplicationLifecycle.Elapsed);
        }

        private void DetectIisAdaptiveErrorViolation(HttpApplication context)
        {
            if (context == null) { return; }
            PropertyInfo headersWritten = ReflectionUtility.GetProperty(context.Response.GetType(), "HeadersWritten", typeof(bool), null, ReflectionUtility.BindingInstancePublicAndPrivateNoneInherited);
            if (headersWritten != null)
            {
                this.IsHeadersWrittenDoToIisAdaptiveErrorLifecycleViolation = (bool)headersWritten.GetValue(context.Response, null);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the ASP.NET lifecycle associated <see cref="HttpResponse"/> has its headers written prematurely do to IIS adaptive error lifecycle violation.
        /// </summary>
        /// <value><c>true</c> if the ASP.NET lifecycle associated <see cref="HttpResponse"/> has its headers written prematurely do to IIS adaptive error lifecycle violation; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// Unfortunately some engineers at the Microsoft .NET Team and IIS Team decided to implement an internal UseAdaptiveError feature.<br/>
        /// What this does is that if an error code server side is &gt;400 and &lt;600 the resulting status code to your code is translated to 200 and headers as well as content is transferred automatically by IIS bypassing the otherwise well established ASP.NET Life Cycle.<br/><br/>
        /// IMO this is as much a deal breaker as it is a design flaw as one should unconditionally be able to trust the ASP.NET Lifecycle.<br/><br/>
        /// Headers to the client should NEVER (and let me emphasis that) NEVER be send until after <see cref="OnPreSendRequestHeaders"/> (which is equivalent to <see cref="HttpApplication.PreSendRequestHeaders"/>) just as client content should be sent AFTER <see cref="OnPreSendRequestContent"/> (which is equivalent to <see cref="HttpApplication.PreSendRequestContent"/>).
        /// </remarks>
        public bool IsHeadersWrittenDoToIisAdaptiveErrorLifecycleViolation
        {
            get { return _isHeadersWrittenDoToIisAdaptiveErrorLifecycleViolation; }
            private set { _isHeadersWrittenDoToIisAdaptiveErrorLifecycleViolation = value; }
        }

        /// <summary>
        /// Handles the Error event of the HttpApplication control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void HttpApplication_Error(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            this.OnError(application);
        }

        private void OnError(HttpApplication context)
        {
            Exception lastException = context.Server.GetLastError();
            this.OnError(context, lastException);
            this.AddOrUpdateMeasuredApplicationLifecycle("OnError", this.ApplicationLifecycle.Elapsed);
        }

        /// <summary>
        /// Handles the AuthorizeRequest event of the HttpApplication control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void HttpApplication_AuthorizeRequest(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            this.OnAuthorizeRequest(application);
            this.AddOrUpdateMeasuredApplicationLifecycle("OnAuthorizeRequest", this.ApplicationLifecycle.Elapsed);
        }

        /// <summary>
        /// Handles the PostResolveRequestCache event of the HttpApplication control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void HttpApplication_PostResolveRequestCache(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            this.OnPostResolveRequestCache(application);
            this.HandleUrlRouting(application);
            this.AddOrUpdateMeasuredApplicationLifecycle("OnPostResolveRequestCache", this.ApplicationLifecycle.Elapsed);
        }

        /// <summary>
        /// Handles the PostMapRequestHandler event of the HttpApplication control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void HttpApplication_PostMapRequestHandler(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            this.OnPostMapRequestHandler(application);
            this.AddOrUpdateMeasuredApplicationLifecycle("OnPostMapRequestHandler", this.ApplicationLifecycle.Elapsed);
        }

        /// <summary>
        /// Handles the PostAcquireRequestState event of the HttpApplication control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void HttpApplication_PostAcquireRequestState(object sender, EventArgs e)
        {
            HttpApplication application = sender as HttpApplication;
            this.OnPostAcquireRequestState(application);
            this.AddOrUpdateMeasuredApplicationLifecycle("OnPostAcquireRequestState", this.ApplicationLifecycle.Elapsed);
        }

        private static void TraceWrite(string currentLifecycleEvent, TimeSpan elapsed)
        {
            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} milliseconds have elapsed up till {1} event.", elapsed.TotalMilliseconds, currentLifecycleEvent));
        }
        #endregion

        /// <summary>
        /// Disposes of the resources (other than memory) used by the module that implements <see cref="T:System.Web.IHttpModule"></see>.
        /// </summary>
        public virtual void Dispose()
        {
        }
        #endregion
    }
}