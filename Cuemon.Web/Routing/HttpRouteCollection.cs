using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Cuemon.Web.Routing
{
    /// <summary>
    /// Provides a collection of routes for ASP.NET routing.
    /// </summary>
    public sealed class HttpRouteCollection : Collection<HttpRoute>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRouteCollection"/> class.
        /// </summary>
        public HttpRouteCollection()
        {
        }
    }
}