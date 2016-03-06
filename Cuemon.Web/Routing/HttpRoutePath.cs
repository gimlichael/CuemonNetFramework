using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Configuration;
using Cuemon.Collections.Generic;
using Cuemon.IO;
using Cuemon.Runtime.Caching;
using Cuemon.Web.Configuration;

namespace Cuemon.Web.Routing
{
    /// <summary>
    /// Provides supplemental information about the URL associated with a <see cref="HttpRoute"/>.
    /// </summary>
    public sealed class HttpRoutePath
    {
        private readonly int hashCode = 4794984;
        private static readonly IList<HttpHandlerAction> systemHandlers = new List<HttpHandlerAction>(WebConfigurationUtility.GetSystemHandlers());

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRoutePath"/> class.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        public HttpRoutePath(HttpContext context)
            : this(ValidateContext(context).Request.Url,
                   ValidateContext(context).Request.PhysicalPath,
                   ValidateContext(context).Request.FilePath)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRoutePath"/> class.
        /// </summary>
        /// <param name="url">The URL of the route.</param>
        /// <param name="physicalFilePath">The physical file path of the route.</param>
        /// <param name="virtualFilePath">The virtual file path of the route.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="url"/> is null -or- <paramref name="physicalFilePath"/> is null -or- <paramref name="virtualFilePath"/> is null.
        /// </exception>
        public HttpRoutePath(Uri url, string physicalFilePath, string virtualFilePath)
        {
            Validator.ThrowIfNull(url, nameof(url));
            Validator.ThrowIfNull(physicalFilePath, nameof(physicalFilePath));
            Validator.ThrowIfNull(virtualFilePath, nameof(virtualFilePath));

            this.Url = url;
            this.PhysicalFilePath = physicalFilePath.ToLowerInvariant();
            this.VirtualFilePath = virtualFilePath.ToLowerInvariant();
            this.hashCode = url.GetHashCode() ^ physicalFilePath.GetHashCode() ^ virtualFilePath.GetHashCode();
        }

        private static bool IsHandlerCore(string virtualFilePath)
        {
            string virtualFilePathExtension = Path.GetExtension(virtualFilePath);
            if (virtualFilePathExtension == null) { return false; }

            foreach (HttpHandlerAction handler in systemHandlers)
            {
                if (handler.Path.StartsWith("*.", StringComparison.OrdinalIgnoreCase))
                {
                    if (virtualFilePathExtension.Equals(Path.GetExtension(handler.Path), StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether the <see cref="PhysicalFilePath"/> contains the name of a file that exists.
        /// </summary>
        /// <value><c>true</c> if <see cref="PhysicalFilePath"/> contains the name of an existing file; otherwise, <c>false</c>.</value>
        public bool HasPhysicalFile
        {
            get
            {
                Doer<string, bool> fileExists = CachingManager.Cache.Memoize<string, bool>(File.Exists);
                return fileExists(this.PhysicalFilePath);
            }
        }

        /// <summary>
        /// Gets the MIME type of the physical file.
        /// </summary>
        /// <value>If <see cref="HasPhysicalFile"/> evaluates to <c>true</c>, then this property returns the MIME type of the physical file; otherwise, <c>null</c>.</value>
        public FileMapping PhysicalFileMimeType
        {
            get
            {
                if (this.HasPhysicalFile) { return EnumerableUtility.FirstOrDefault(MimeUtility.ParseFileExtensions(Path.GetExtension(this.PhysicalFilePath))); }
                return null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="VirtualFilePath"/> points to a handler.
        /// </summary>
        /// <value><c>true</c> if the <see cref="VirtualFilePath"/> points to a handler; otherwise, <c>false</c>.</value>
        public bool IsHandler
        {
            get
            {
                Doer<string, bool> isHandler = CachingManager.Cache.Memoize<string, bool>(IsHandlerCore);
                return isHandler(this.VirtualFilePath);
            }
        }

        /// <summary>
        /// Gets information about the URL of the route.
        /// </summary>
        /// <value>A <see cref="Uri"/> object containing information regarding the URL of the route.</value>
        public Uri Url { get; private set; }

        /// <summary>
        /// Gets the physical file system path corresponding to the routed URL.
        /// </summary>
        /// <value>The file system path of the routed URL.</value>
        public string PhysicalFilePath { get; private set; }

        /// <summary>
        /// Gets the virtual path of the route.
        /// </summary>
        /// <value>The virtual path of the route.</value>
        public string VirtualFilePath { get; private set; }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return hashCode;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            HttpRoutePath routePath = obj as HttpRoutePath;
            return routePath != null && this.Equals(routePath);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><c>true</c> if the current object is equal to the other parameter; otherwise, <c>false</c>. </returns>
        public bool Equals(HttpRoutePath other)
        {
            if (other == null) { return false; }
            return (this.GetHashCode() == other.GetHashCode());
        }

        /// <summary>
        /// Indicates whether two <see cref="HttpRoutePath"/> instances are equal.
        /// </summary>
        /// <param name="routePath1">The first <see cref="HttpRoutePath"/> to compare.</param>
        /// <param name="routePath2">The second <see cref="HttpRoutePath"/> to compare.</param>
        /// <returns><c>true</c> if the values of <paramref name="routePath1"/> and <paramref name="routePath2"/> are equal; otherwise, false. </returns>
        public static bool operator ==(HttpRoutePath routePath1, HttpRoutePath routePath2)
        {
            return routePath1 != null && routePath1.Equals(routePath2);
        }

        /// <summary>
        /// Indicates whether two <see cref="HttpRoutePath"/> instances are not equal.
        /// </summary>
        /// <param name="routePath1">The first <see cref="HttpRoutePath"/> to compare.</param>
        /// <param name="routePath2">The second <see cref="HttpRoutePath"/> to compare.</param>
        /// <returns><c>true</c> if the values of <paramref name="routePath1"/> and <paramref name="routePath2"/> are not equal; otherwise, false.</returns>
        public static bool operator !=(HttpRoutePath routePath1, HttpRoutePath routePath2)
        {
            return routePath1 != null && !routePath1.Equals(routePath2);
        }

        private static HttpContext ValidateContext(HttpContext context)
        {
            if (context == null) { throw new ArgumentNullException(nameof(context)); }
            return context;
        }
    }
}