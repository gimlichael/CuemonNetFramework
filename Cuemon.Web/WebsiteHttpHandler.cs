using System;
using System.Web;
using System.Web.SessionState;
using System.Globalization;
using Cuemon.Globalization;
using Cuemon.Net;
using Cuemon.Web.UI;
namespace Cuemon.Web
{
    /// <summary>
    /// A helper HttpHandler for the support of SessionState object doing requests from a HttpModule.
    /// </summary>
    public class WebsiteHttpHandler : IHttpHandler, IRequiresSessionState
    {
        private readonly IHttpHandler _originalHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebsiteHttpHandler"/> class.
        /// </summary>
        /// <param name="originalHandler">The original handler.</param>
        public WebsiteHttpHandler(IHttpHandler originalHandler)
        {
            _originalHandler = originalHandler;
        }

        /// <summary>
        /// Gets the original handler.
        /// </summary>
        /// <value>The original handler.</value>
        internal IHttpHandler OriginalHandler
        {
            get { return _originalHandler; }
        }

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"/> interface.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpContext"/> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
        public void ProcessRequest(HttpContext context)
        {
            // do not worry, ProcessRequest() will not be called, but let's be safe
            throw new InvalidOperationException("WebsiteHttpHandler cannot process requests.");
        }

        /// <summary>
        /// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler"/> instance.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Web.IHttpHandler"/> instance is reusable; otherwise, false.</returns>
        public bool IsReusable
        {
            // IsReusable must be set to false since class has a member!
            get { return false; }
        }
    }
}