using System;
using System.Collections.Generic;
using System.Web;
using Cuemon.Collections.Generic;
using Cuemon.Web.Compilation;

namespace Cuemon.Web.Routing
{
    /// <summary>
    /// Stores the URL routes for an application.
    /// </summary>
    public static class HttpRouteTable
    {
        private static readonly HttpRouteCollection Singleton = new HttpRouteCollection();

        /// <summary>
        /// Gets a collection of objects that derive from the <see cref="HttpRoute"/> class.
        /// </summary>
        /// <value>An object that contains all the routes in the collection.</value>
        public static HttpRouteCollection Routes
        {
            get { return Singleton; }
        }

        /// <summary>
        /// Determines whether a <see cref="HttpRoute"/> can be resolved from the specified <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application to resolve a <see cref="HttpRoute"/> from.</param>
        /// <param name="route">The route that was resolved from the specified <paramref name="context"/>, if the parsing succeeded, or a null reference if the conversion failed.</param>
        /// <returns><c>true</c> if the <paramref name="route"/> parameter was resolved successfully; otherwise, <c>false</c>.</returns>
        public static bool TryParse(HttpContext context, out HttpRoute route)
        {
            IDictionary<string, object> data = new Dictionary<string, object>();
            return TryParse(context, data, out route);
        }

        /// <summary>
        /// Determines whether a <see cref="HttpRoute"/> can be resolved from the specified <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application to resolve a <see cref="HttpRoute"/> from.</param>
        /// <param name="data">A collection of key/value pairs that provide additional user-defined information that must match the route being parsed.</param>
        /// <param name="route">The route that was resolved from the specified <paramref name="context"/>, if the parsing succeeded, or a null reference if the conversion failed.</param>
        /// <returns><c>true</c> if the <paramref name="route"/> parameter was resolved successfully; otherwise, <c>false</c>.</returns>
        public static bool TryParse(HttpContext context, IDictionary<string, object> data, out HttpRoute route)
        {
            if (context == null || Routes.Count == 0) // need two methods; one called from OnPostMapRequestHandler - other from OnAuthorizeRequest
            {
                route = null;
                return false;
            }


            IHttpHandler handler = context.CurrentHandler ?? context.Handler;
            route = handler == null ? ParseCore(context, data) : ParseCore(handler, data);
            return route != null;
        }

        /// <summary>
        /// Gets or sets the function delegate that will retrieve the ASP.NET <see cref="IHttpHandler"/> types.
        /// </summary>
        /// <value>The function delegate for retrieving ASP.NET <see cref="IHttpHandler"/> types.</value>
        /// <remarks>If no function delegate is assigned, a default implementation of <see cref="CompilationUtility.GetReferencedHandlerTypes"/> is used.</remarks>
        public static Doer<IReadOnlyCollection<Type>> ReferencedHandlerTypes { get; set; }

        private static IReadOnlyCollection<Type> GetDefaultReferencedHandlerTypes()
        {
            if (ReferencedHandlerTypes == null) { ReferencedHandlerTypes = CompilationUtility.GetReferencedHandlerTypes; }
            return ReferencedHandlerTypes();
        }

        private static HttpRoute ParseCore(HttpContext context, IDictionary<string, object> data)
        {
            HttpRoutePath currentPath = new HttpRoutePath(context);
            foreach (HttpRoute route in Routes)
            {
                if (!IsDataEqual(route.Data, data)) { continue; }
                IReadOnlyCollection<Type> handlers = GetDefaultReferencedHandlerTypes();
                if (!handlers.Contains(route.HandlerType)) { continue; }
                if (currentPath.HasPhysicalFile && currentPath.IsHandler)
                {
                    if (currentPath.VirtualFilePath.StartsWith(route.VirtualFilePath ?? "", StringComparison.OrdinalIgnoreCase))
                    {
                        return route;
                    }
                }
                else if (!currentPath.HasPhysicalFile)
                {
                    Uri endpointUri = new Uri(HttpRequestUtility.GetHostAuthority(currentPath.Url), route.UriPattern);
                    if (currentPath.Url.AbsolutePath.Equals(endpointUri.AbsolutePath, StringComparison.OrdinalIgnoreCase))
                    {
                        return route;
                    }
                }
            }
            return null;
        }

        private static HttpRoute ParseCore(IHttpHandler handler, IDictionary<string, object> data)
        {
            foreach (HttpRoute route in Routes)
            {
                if (!IsDataEqual(route.Data, data)) { continue; }
                if (TypeUtility.ContainsType(handler.GetType(), route.HandlerType)) { return route; }
            }
            return null;
        }

        private static bool IsDataEqual(IDictionary<string, object> source, IDictionary<string, object> comparer)
        {
            if (source.Count != comparer.Count) { return false; }
            bool result = true;
            foreach (KeyValuePair<string, object> sourceValuePair in source)
            {
                object o;
                if (comparer.TryGetValue(sourceValuePair.Key, out o))
                {
                    result &= (sourceValuePair.Value.Equals(o));
                }
            }
            return result;
        }
    }
}