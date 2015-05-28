using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Xml.XPath;
using Cuemon.Caching;
using Cuemon.Collections.Generic;
using Cuemon.IO;
using Cuemon.Security.Cryptography;
using Cuemon.Xml.XPath;

namespace Cuemon.Web.Configuration
{
    /// <summary>
    /// This utility class is designed to make operations related to a Web.config file easier to work with.
    /// </summary>
    public static class WebConfigurationUtility
    {
        /// <summary>
        /// Opens the Web-application configuration file as an object that implements the <see cref="IXPathNavigable"/> interface using the specified virtual path to allow read or write operations.
        /// </summary>
        /// <param name="path">The virtual path to the configuration file.</param>
        /// <returns>An object that implements the <see cref="IXPathNavigable"/> interface.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="path"/> is null.
        /// </exception>
        /// <exception cref="Cuemon.ArgumentEmptyException">
        /// <paramref name="path"/> is empty.
        /// </exception>
        public static IXPathNavigable OpenWebConfiguration(string path)
        {
            return OpenWebConfiguration(path, GlobalModule.AppPoolIdentity ?? WindowsIdentity.GetCurrent());
        }

        /// <summary>
        /// Opens the Web-application configuration file as an object that implements the <see cref="IXPathNavigable"/> interface using the specified virtual path to allow read or write operations.
        /// </summary>
        /// <param name="path">The virtual path to the configuration file.</param>
        /// <param name="identity">The Windows identity that will open the specified <paramref name="path"/>.</param>
        /// <returns>An object that implements the <see cref="IXPathNavigable"/> interface.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="path"/> is null.
        /// </exception>
        /// <exception cref="Cuemon.ArgumentEmptyException">
        /// <paramref name="path"/> is empty.
        /// </exception>
        public static IXPathNavigable OpenWebConfiguration(string path, WindowsIdentity identity)
        {
            Validator.ThrowIfNullOrEmpty(path, "path");
            Validator.ThrowIfNull(identity, "identity");

            using (WindowsImpersonationContext wic = identity.Impersonate())
            {
                string webConfigLocation = GetWebConfigLocation(path);
                string cacheKey = HashUtility.ComputeHash(webConfigLocation);
                if (!CachingManager.Cache.ContainsKey(cacheKey))
                {
                    Uri fileUri = new Uri(webConfigLocation);
                    IXPathNavigable document = XPathUtility.CreateXPathNavigableDocument(fileUri);
                    string directory = Path.GetDirectoryName(webConfigLocation);
                    string filter = Path.GetFileName(webConfigLocation);
                    CachingManager.Cache.Add(cacheKey, document, new FileDependency(directory, filter));
                }
                wic.Undo();
                return CachingManager.Cache.Get<IXPathNavigable>(cacheKey);
            }
        }

        private static string GetWebConfigLocation(string path)
        {
            return HostingEnvironment.MapPath(path);
        }

        /// <summary>
        /// Returns all system handlers of the default (root) Web-application configuration file and derived.
        /// </summary>
        /// <returns>A sequence of <see cref="HttpHandlerAction"/> elements.</returns>
        public static IEnumerable<HttpHandlerAction> GetSystemHandlers()
        {
            return GetSystemHandlers("~/Web.config");
        }

        /// <summary>
        /// Returns all system handlers of the default (root) Web-application configuration file and derived.
        /// </summary>
        /// <param name="path">The virtual path to the configuration file.</param>
        /// <returns>A sequence of <see cref="HttpHandlerAction"/> elements.</returns>
        public static IEnumerable<HttpHandlerAction> GetSystemHandlers(string path)
        {
            return GetSystemHandlers(path, GlobalModule.AppPoolIdentity ?? WindowsIdentity.GetCurrent());
        }

        /// <summary>
        /// Returns all system handlers of the default (root) Web-application configuration file and derived.
        /// </summary>
        /// <param name="path">The virtual path to the configuration file.</param>
        /// <param name="identity">The Windows identity that will open the specified <paramref name="path"/>.</param>
        /// <returns>A sequence of <see cref="HttpHandlerAction"/> elements.</returns>
        public static IEnumerable<HttpHandlerAction> GetSystemHandlers(string path, WindowsIdentity identity)
        {
            Validator.ThrowIfNullOrEmpty(path, "path");
            Validator.ThrowIfNull(identity, "identity");

            using (WindowsImpersonationContext wic = identity.Impersonate())
            {
                System.Configuration.Configuration config = WebConfigurationManager.OpenWebConfiguration(path);
                HttpHandlersSection iis6HandlerSection = config.GetSection("system.web/httpHandlers") as HttpHandlersSection;
                HttpHandlersSection iis7HandlerSection = config.GetSection("system.webServer/handlers") as HttpHandlersSection;
                List<HttpHandlerAction> customHandlers = new List<HttpHandlerAction>(GetHandlers(OpenWebConfiguration(path)));

                if (iis6HandlerSection != null)
                {
                    foreach (HttpHandlerAction handler in iis6HandlerSection.Handlers)
                    {
                        bool identical = false;
                        foreach (HttpHandlerAction customHandler in customHandlers)
                        {
                            if (IsIdentical(handler, customHandler))
                            {
                                identical = true;
                                break;
                            }
                        }
                        if (!identical) { yield return handler; }
                    }
                }

                if (iis7HandlerSection != null)
                {
                    foreach (HttpHandlerAction handler in iis7HandlerSection.Handlers)
                    {
                        bool identical = false;
                        foreach (HttpHandlerAction customHandler in customHandlers)
                        {
                            if (IsIdentical(handler, customHandler))
                            {
                                identical = true;
                                break;
                            }
                        }
                        if (!identical) { yield return handler; }
                    }
                }
                wic.Undo();
            }
        }

        private static bool IsIdentical(HttpHandlerAction source, HttpHandlerAction compare)
        {
            return (source.Path.Equals(compare.Path, StringComparison.OrdinalIgnoreCase) &&
                source.Type.Equals(compare.Type, StringComparison.OrdinalIgnoreCase) &&
                source.Verb.Equals(compare.Verb, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Returns the handlers section of the default (root) Web-application configuration file.
        /// </summary>
        /// <returns>A sequence of <see cref="HttpHandlerAction"/> elements.</returns>
        public static IEnumerable<HttpHandlerAction> GetHandlers()
        {
            return GetHandlers(OpenWebConfiguration("~/Web.config"));
        }

        /// <summary>
        /// Returns the system.webServer/handlers and/or system.web/httpHandlers section of the specified <paramref name="webConfiguration"/> file.
        /// </summary>
        /// <param name="webConfiguration">An object that implements the <see cref="IXPathNavigable"/> interface and represent a Web-application configuration file.</param>
        /// <returns>A sequence of <see cref="HttpHandlerAction"/> elements.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="webConfiguration"/> is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// The content of the specified <paramref name="webConfiguration"/> does not appear to be from a web.config file.
        /// </exception>
        public static IEnumerable<HttpHandlerAction> GetHandlers(IXPathNavigable webConfiguration)
        {
            if (webConfiguration == null) { throw new ArgumentNullException("webConfiguration"); }
            
            XPathNavigator navigator = webConfiguration.CreateNavigator();
            XPathNavigator root = navigator.SelectSingleNode("/configuration");
            if (root == null) { throw new ArgumentException("The content of the provided IXPathNavigable does not appear to be from a web.config file.", "webConfiguration"); }
            XPathNodeIterator handlers = navigator.Select("configuration/system.webServer/handlers/add|configuration/system.web/httpHandlers/add"); // IIS7+ | IIS6
            List<int> hashes = new List<int>();
            while (handlers.MoveNext())
            {
                string handlerPath = handlers.Current.SelectSingleNode("@path") == null ? null : handlers.Current.SelectSingleNode("@path").Value;
                string handlerType = handlers.Current.SelectSingleNode("@type") == null ? null : handlers.Current.SelectSingleNode("@type").Value;
                string handlerVerb = handlers.Current.SelectSingleNode("@verb") == null ? null : handlers.Current.SelectSingleNode("@verb").Value;

                if (handlerPath != null && handlerType != null && handlerVerb != null)
                {
                    int hash = handlerPath.ToLowerInvariant().GetHashCode() ^ handlerType.ToLowerInvariant().GetHashCode() ^ handlerVerb.ToLowerInvariant().GetHashCode();
                    if (!hashes.Contains(hash))
                    {
                        hashes.Add(hash);
                        yield return new HttpHandlerAction(handlerPath, handlerType, handlerVerb);
                    }
                }
            }
        }

        /// <summary>
        /// Returns the handler of the handlers section of the default (root) Web-application configuration file who's @type matches the specified <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="HttpHandlerAction"/> to retrieve.</typeparam>
        /// <returns>A <see cref="HttpHandlerAction"/> object who's <see cref="HttpHandlerAction.Type"/> property matches the specified <typeparamref name="T"/>.</returns>
        public static HttpHandlerAction GetHandler<T>()
        {
            return GetHandler<T>(OpenWebConfiguration("~/Web.config"));
        }

        /// <summary>
        /// Returns the handler of the handlers section of the specified <paramref name="webConfiguration"/> file who's @type matches the specified <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="HttpHandlerAction"/> to retrieve.</typeparam>
        /// <param name="webConfiguration">An object that implements the <see cref="IXPathNavigable"/> interface and represent a Web-application configuration file.</param>
        /// <returns>A <see cref="HttpHandlerAction"/> object who's <see cref="HttpHandlerAction.Type"/> property matches the specified <typeparamref name="T"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="webConfiguration"/> is null.
        /// </exception>
        public static HttpHandlerAction GetHandler<T>(IXPathNavigable webConfiguration)
        {
            if (webConfiguration == null) { throw new ArgumentNullException("webConfiguration"); }
            Type handlerType = typeof(T);
            IEnumerable<HttpHandlerAction> handlers = GetHandlers(webConfiguration);
            foreach (HttpHandlerAction handler in handlers)
            {
                if (Type.GetType(handler.Type) == handlerType) { return handler; }
            }
            return null;
        }
    }
}