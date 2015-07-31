using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.UI;
using Cuemon.Collections.Generic;
using Cuemon.Reflection;
using Cuemon.Web.Routing;

namespace Cuemon.Web
{
    /// <summary>
    /// A <see cref="GlobalModule"/> implementation that is tweaked for a routing role with ASP.NET as the runtime platform.
    /// </summary>
    public class RouteModule : GlobalModule
    {
        private static IEnumerable<string> PhysicalFileNames = null;
        private static readonly IDictionary<Type, IEnumerable<HttpRouteAttribute>> HandlerRoutes = new Dictionary<Type, IEnumerable<HttpRouteAttribute>>();
        //internal static readonly IDictionary<Type, HttpHandlerAction> HandlerActions = new Dictionary<Type, HttpHandlerAction>();

        /// <summary>
        /// Provides access to the ApplicationStart event that occurs when an AppPool is first started.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked only once as the first event in the HTTP pipeline chain of execution when ASP.NET responds to a request.</remarks>
        protected override void OnApplicationStart(HttpApplication context)
        {
            Validator.ThrowIfNull(context, "context");

            string path = context.Context.Request.PhysicalApplicationPath;
            string[] extensions = this.RouteExtensions.Split('|');
            if (path != null)
            {
                PhysicalFileNames = EnumerableUtility.FindAll(Directory.GetFiles(path, "*.*", SearchOption.AllDirectories), Match, extensions);
            }

            this.DiscoverHandlerRoutes(typeof(IHttpHandler));
            base.OnApplicationStart(context);
            //this.DiscoverHandlerActions();
        }

        /// <summary>
        /// Provides access to the Load event of the currently executing <see cref="Page"/> control.
        /// </summary>
        /// <param name="page">The page handler of the current request.</param>
        /// <remarks>This method is invoked when the <paramref name="page"/> loads.</remarks>
        protected override void OnLoad(Page page)
        {
            Validator.ThrowIfNull(page, "page");
            PropertyInfo reflectedContext = page.GetType().GetProperty("Context", ReflectionUtility.BindingInstancePublicAndPrivate);
            if (reflectedContext != null)
            {
                HttpContext context = reflectedContext.GetValue(page, null) as HttpContext;
                if (context != null)
                {
                    HttpRoute route = context.Items["httpRoute"] as HttpRoute;
                    if (route != null)
                    {
                        object[] parameters;
                        MethodInfo method = route.ParseMethod(context.Request, out parameters);
                        if (method != null)
                        {
                            method.Invoke(page, parameters);
                        }
                    }
                }
            }

            base.OnLoad(page);
        }

        private bool Match(string file, string[] extensions)
        {
            foreach (string extension in extensions)
            {
                if (file.EndsWith(extension, StringComparison.OrdinalIgnoreCase)) { return true; }
            }
            return false;
        }

        /// <summary>
        /// Gets the handler extensions supported by this routing module.
        /// </summary>
        /// <value>The handler extensions supported by this routing module.</value>
        /// <remarks>Default supported handler extensions are: <b>.ascx|.ashx|.asmx|.aspx|.axd</b>.</remarks>
        public virtual string RouteExtensions
        {
            get { return ".ascx|.ashx|.asmx|.aspx|.axd"; }
        }


        private static string ToVirtualPath(Type handler)
        {
            string partialHandlerFileName = handler.FullName.Replace(".", @"\").Replace("_", ".");
            foreach (string physicalFileName in PhysicalFileNames)
            {
                string extension = Path.GetExtension(physicalFileName);
                if (extension != null)
                {
                    string physicalFileNameWithoutExtension = physicalFileName.Replace(extension, "");
                    string reducedPartialHandlerFileName = partialHandlerFileName;
                    while (reducedPartialHandlerFileName.Length > 0)
                    {
                        if (physicalFileNameWithoutExtension.EndsWith(reducedPartialHandlerFileName, StringComparison.OrdinalIgnoreCase))
                        {
                            return reducedPartialHandlerFileName.Replace(@"\", "/") + extension;
                        }
                        int indexOfFileSlash = reducedPartialHandlerFileName.IndexOf('\\') + 1;
                        reducedPartialHandlerFileName = indexOfFileSlash > 0 ? reducedPartialHandlerFileName.Remove(0, indexOfFileSlash) : "";
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Discovers the routes associated with the specified <paramref name="routeHandler"/> implementing the <see cref="IHttpHandler"/> interface.
        /// </summary>
        /// <param name="routeHandler">The <see cref="Type"/> to discover routes from.</param>
        protected virtual void DiscoverHandlerRoutes(Type routeHandler)
        {
            Validator.ThrowIfNull(routeHandler, "routeHandler");
            Validator.ThrowIfNot(routeHandler, HasHttpHandlerInterface, ExceptionUtility.CreateArgumentException, "routeHandler", "The specified type does not implement the IHttpHandler interface.");
            IReadOnlyCollection<Type> routeHandlerTypes = GetReferencedTypes(routeHandler);
            foreach (Type routeHandlerType in routeHandlerTypes)
            {
                List<HttpRouteAttribute> httpAttributes = new List<HttpRouteAttribute>();
                IEnumerable<MethodInfo> methods = ReflectionUtility.GetMethods(routeHandlerType);
                foreach (MethodInfo method in methods)
                {
                    IEnumerable<HttpRouteAttribute> attributes = ReflectionUtility.GetAttributes<HttpRouteAttribute>(method, true);
                    if (attributes == null) { continue; }
                    foreach (HttpRouteAttribute attribute in attributes)
                    {
                        string handlerVirtualPath = attribute.VirtualFilePath ?? ToVirtualPath(routeHandlerType);
                        IHttpHandler handler = Activator.CreateInstance(routeHandlerType) as IHttpHandler;
                        if (handlerVirtualPath == null) { throw new HttpRouteException(handler, attribute); }
                        HttpRouteTable.Routes.Add(new HttpRoute(attribute.UriPattern, handlerVirtualPath, handler, attribute.Methods));
                    }
                }
                
                if (httpAttributes.Count > 0) { HandlerRoutes.Add(routeHandlerType, httpAttributes); }
            }
        }

        //protected virtual void DiscoverHandlerActions()
        //{
        //    IXPathNavigable webConfig = WebConfigurationUtility.OpenWebConfiguration("~/Web.config");
        //    IEnumerable<HttpHandlerAction> handlerActions = WebConfigurationUtility.GetHandlers(webConfig);
        //    foreach (Type routeHandlerType in HandlerRoutes.Keys)
        //    {
        //        foreach (HttpHandlerAction handlerAction in handlerActions)
        //        {
        //            if (routeHandlerType == routeHandlerType.Assembly.GetType(handlerAction.Type))
        //            {
        //                HandlerActions.Add(routeHandlerType, handlerAction);
        //            }
        //        }
        //    }
        //}

        private bool HasHttpHandlerInterface(Type routeHandler)
        {
            Type match = typeof(IHttpHandler);
            if (routeHandler == match) { return true; }
            return TypeUtility.ContainsInterface(routeHandler, match);
        }
    }
}
