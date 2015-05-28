using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web;
using Cuemon.Web.Routing;

namespace Cuemon.Web
{
    public partial class GlobalModule
    {
        /// <summary>
        /// Handles the URL routing of this module.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is called just after any custom implementation of <see cref="OnPostResolveRequestCache"/>.</remarks>
        protected virtual void HandleUrlRouting(HttpApplication context)
        {
            if (context == null) { throw new ArgumentNullException("context"); }
            HttpRoute route;
            if (HttpRouteTable.TryParse(context.Context, out route))
            {
                HttpRoutePath path = route.GetVirtualRoutePath(context.Context);
                if (path.HasPhysicalFile && path.IsHandler)
                {
                    HttpResponseUtility.RedirectPermanently(string.Format(CultureInfo.InvariantCulture, "{0}{1}", route.UriPattern, path.Url.Query));
                }
                else
                {
                    if (!path.HasPhysicalFile)
                    {
                        context.Context.RewritePath(string.Format(CultureInfo.InvariantCulture, "{0}{1}", route.VirtualFilePath, path.Url.Query), false);
                        context.Context.Handler = route.Handler;
                        context.Context.Items.Add("httpRoute", route);
                    }
                }
            }
        }
    }
}