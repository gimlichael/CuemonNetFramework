using System;
using System.Globalization;
using System.IO;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using Cuemon.Integrity;
using Cuemon.IO;
using Cuemon.IO.Compression;
using Cuemon.Runtime.Caching;
using Cuemon.Web.Routing;

namespace Cuemon.Web
{
    /// <summary>
    /// A <see cref="GlobalModule"/> implementation that is tweaked for a Content Delivery Network (CDN) role with ASP.NET as the runtime platform.
    /// </summary>
    public class CdnGlobalModule : GlobalModule
    {
        private const string CompressionCacheGroup = "ParseResourceCompression";
        private static readonly Regex CompiledMd5PatternExpression = new Regex("([a-f0-9]{32})", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Provides access to the ApplicationStart event that occurs when an AppPool is first started.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked only once as the first event in the HTTP pipeline chain of execution when ASP.NET responds to a request.</remarks>
        protected override void OnApplicationStart(HttpApplication context)
        {
            base.OnApplicationStart(context);
            EnableTokenParsingForClientCaching = true;
        }

        /// <summary>
        /// Provides access to the AuthorizeRequest event of the <see cref="HttpApplication"/> control.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked when a security module has verified user authorization.</remarks>
        protected override void OnAuthorizeRequest(HttpApplication context)
        {
            if (EnableTokenParsingForClientCaching) { HandleTokenParsingUrlRouting(context); }
        }

        /// <summary>
        ///  Handles the URL routing of the result from the query-string token parsing.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        protected virtual void HandleTokenParsingUrlRouting(HttpApplication context)
        {
            if (context == null) { throw new ArgumentNullException(nameof(context)); }
            if (!IsHtmlRelatedContent(context)) { return; }
            string rawUrl = context.Request.RawUrl;
            Match match = CompiledMd5PatternExpression.Match(rawUrl);
            if (match.Success)
            {
                //HttpResponseUtility.RedirectPermanently(rawUrl.Replace(match.Value, "")); // consider redirect
                context.Context.RewritePath(rawUrl.Replace(match.Value, ""), false);
            }
        }

        /// <summary>
        /// Handles the expires headers of static content.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <param name="expiresInterval">The interval added to <see cref="DateTime.UtcNow"/> for a calculated HTTP Expires header.</param>
        /// <param name="cacheability">Sets the <b>Cache-Control</b> header to one of the values of <see cref="HttpCacheability"/>.</param>
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        protected override void HandleStaticContentExpiresHeaders(HttpApplication context, TimeSpan expiresInterval, HttpCacheability cacheability, CacheValidator validator)
        {
            base.HandleStaticContentExpiresHeaders(context, expiresInterval, HttpCacheability.Public, validator);
        }

        /// <summary>
        /// Handles the expires headers of dynamic content - as in where an associated <see cref="HttpContext.Handler"/> has been assigned (such as aspx, ashx, asmx and so forth).
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <param name="expiresInterval">The interval added to <see cref="DateTime.UtcNow"/> for a calculated HTTP Expires header.</param>
        /// <param name="cacheability">Sets the <b>Cache-Control</b> header to one of the values of <see cref="HttpCacheability"/>.</param>
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        protected override void HandleDynamicContentExpiresHeaders(HttpApplication context, TimeSpan expiresInterval, HttpCacheability cacheability, CacheValidator validator)
        {
            base.HandleDynamicContentExpiresHeaders(context, expiresInterval, HttpCacheability.Public, validator);
        }

        /// <summary>
        /// Provides access to the PreRequestHandlerExecute event of the <see cref="HttpApplication"/> control.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked just before ASP.NET starts executing an event handler.</remarks>
        protected override void OnPreRequestHandlerExecute(HttpApplication context)
        {
            if (context == null) { throw new ArgumentNullException(nameof(context)); }
        }

        /// <summary>
        /// Provides access to the PreSendRequestHeaders event of the <see cref="HttpApplication"/> control.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked just before ASP.NET sends HTTP headers to the client.</remarks>
        protected override void OnPreSendRequestHeaders(HttpApplication context)
        {
            if (context == null) { throw new ArgumentNullException(nameof(context)); }
            if (EnableDynamicClientCaching)
            {
                if (!HttpRequestUtility.IsStandaloneServerLocal(context.Request))
                {
                    HandleDynamicContentExpiresHeaders(context);
                }
            }
            if (EnableStaticClientCaching)
            {
                if (!HttpRequestUtility.IsStandaloneServerLocal(context.Request))
                {
                    HandleStaticContentExpiresHeaders(context);
                }
            }
            HandleCompressionHeaders(context);
            context.Response.AppendHeader("Access-Control-Allow-Origin", "*");
        }

        /// <summary>
        /// Returns a virtual file path of the specified <paramref name="pathToResource"/> while determining if a compressed equivalent can and should be written to the file system.
        /// </summary>
        /// <param name="pathToResource">The HTTP route path to a resource.</param>
        /// <param name="compression">The compression to apply to the <paramref name="pathToResource"/>.</param>
        /// <returns>A virtual file path of the specified <paramref name="pathToResource"/>.</returns>
        protected string ParseResourceCompression(HttpRoutePath pathToResource, CompressionMethodScheme compression)
        {
            return ParseResourceCompression(pathToResource, compression, DefaultVirtualPathResolver);
        }

        /// <summary>
        /// Returns a virtual file path of the specified <paramref name="pathToResource"/> while determining if a compressed equivalent can and should be written to the file system.
        /// </summary>
        /// <param name="pathToResource">The HTTP route path to a resource.</param>
        /// <param name="compression">The compression to apply to the <paramref name="pathToResource"/>.</param>
        /// <param name="virtualPathResolver">The function delegate that will resolve the virtual file path from the specified <paramref name="pathToResource"/>.</param>
        /// <returns>A virtual file path of the specified <paramref name="pathToResource"/>.</returns>
        protected virtual string ParseResourceCompression(HttpRoutePath pathToResource, CompressionMethodScheme compression, Doer<HttpRoutePath, CompressionMethodScheme, string> virtualPathResolver)
        {
            Validator.ThrowIfNull(pathToResource, nameof(pathToResource));
            Validator.ThrowIfNull(virtualPathResolver, nameof(virtualPathResolver));

            Doer<string, bool> canWriteFileToDisk = CachingManager.Cache.Memoize<string, bool>(CanWriteFileToDisk);
            bool enableResourceCompression = canWriteFileToDisk(Path.GetDirectoryName(pathToResource.PhysicalFilePath));

            switch (compression)
            {
                case CompressionMethodScheme.Deflate:
                case CompressionMethodScheme.GZip:
                    break;
                case CompressionMethodScheme.Identity:
                case CompressionMethodScheme.Compress:
                case CompressionMethodScheme.None:
                    enableResourceCompression = false; // we do not support these for compression
                    break;
            }

            if (!enableResourceCompression) { return pathToResource.VirtualFilePath; }
            return virtualPathResolver(pathToResource, compression);
        }

        /// <summary>
        /// Handles the initialization of compression for the <see cref="HttpResponse.OutputStream"/>.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <param name="compression">The compression method to apply to the HTTP Content-Encoding header.</param>
        protected override void InitializeCompression(HttpApplication context, CompressionMethodScheme compression)
        {
            if (!EnableCompression) { return; }
            if (context == null) { throw new ArgumentNullException(nameof(context)); }
            if (HttpRequestUtility.IsStandaloneServerLocal(context.Request)) { return; }
            if (!context.Response.BufferOutput) { return; }
            if (context.Response.SuppressContent) { return; }
            if (!IsValidForCompression(context)) { return; }

            HttpRoutePath path = new HttpRoutePath(context.Context);
            bool hasCachedResource = false;
            if (path.HasPhysicalFile && !path.IsHandler && path.PhysicalFileMimeType != null)
            {
                string resource = ParseResourceCompression(path, compression);
                string resourceExtension = Infrastructure.GetCompressedFileExtension(path, compression);
                hasCachedResource = resource.EndsWith(resourceExtension, StringComparison.OrdinalIgnoreCase);
                if (hasCachedResource)
                {
                    context.Context.RewritePath(string.Format(CultureInfo.InvariantCulture, "{0}{1}", resource, path.Url.Query), false);
                }
                context.Response.ContentType = path.PhysicalFileMimeType.ContentType;
            }
            Infrastructure.ApplyCompression(context, this, compression, () => !hasCachedResource);
        }

        private static string DefaultVirtualPathResolver(HttpRoutePath pathToResource, CompressionMethodScheme compression)
        {
            string virtualPathToCompressedFile;
            if (!CachingManager.Cache.TryGetValue(pathToResource.VirtualFilePath, CompressionCacheGroup, out virtualPathToCompressedFile))
            {
                using (Mutex mutex = new Mutex(false, string.Concat(virtualPathToCompressedFile, "_mutex")))
                {
                    try
                    {
                        mutex.WaitOne();
                        if (!CachingManager.Cache.TryGetValue(pathToResource.VirtualFilePath, CompressionCacheGroup, out virtualPathToCompressedFile))
                        {
                            string newExtension = Infrastructure.GetCompressedFileExtension(pathToResource, compression);
                            var pathToCompressedFile = Path.ChangeExtension(pathToResource.PhysicalFilePath, newExtension).ToLowerInvariant();
                            virtualPathToCompressedFile = Path.ChangeExtension(pathToResource.VirtualFilePath, newExtension).ToLowerInvariant();
                            WriteFileQueued(pathToResource, pathToCompressedFile, compression, virtualPathToCompressedFile);
                        }
                    }
                    finally
                    {
                        mutex.ReleaseMutex();
                    }
                }
            }
            return virtualPathToCompressedFile;
        }

        private static void WriteFileQueued(HttpRoutePath path, string pathToCompressedFile, CompressionMethodScheme compression, string virtualPathToTempFile)
        {
            if (EnableIdentityImpersonate)
            {
                Infrastructure.InvokeWitImpersonationContextOrDefault(WindowsIdentity.GetCurrent, WriteFileQueuedCore, TupleUtility.CreateFour(path, pathToCompressedFile, compression, virtualPathToTempFile));
                return;
            }
            WriteFileQueuedCore(TupleUtility.CreateFour(path, pathToCompressedFile, compression, virtualPathToTempFile));
        }

        private static void WriteFileQueuedCore(Template<HttpRoutePath, string, CompressionMethodScheme, string> parameters)
        {
            HttpRoutePath path = parameters.Arg1;
            string pathToCompressedFile = parameters.Arg2;
            CompressionMethodScheme compression = parameters.Arg3;
            string virtualPathToTempFile = parameters.Arg4;

            string lockpath = string.Concat(path.PhysicalFilePath, ".locked");
            if (File.Exists(lockpath)) { return; }
            if (CachingManager.Cache.ContainsKey(path.VirtualFilePath, CompressionCacheGroup)) { return; }
            try
            {
                using (FileStream temp = File.Create(lockpath)) { temp.Flush(); }
                using (FileStream temp = File.OpenRead(path.PhysicalFilePath))
                {
                    File.WriteAllBytes(pathToCompressedFile, CompressBytes(temp, compression));
                }
                string directory = Path.GetDirectoryName(path.PhysicalFilePath);
                string filename = Path.GetFileName(path.PhysicalFilePath);
                string compressedFilename = Path.GetFileName(pathToCompressedFile);
                CachingManager.Cache.Add(path.VirtualFilePath, virtualPathToTempFile, CompressionCacheGroup, new FileDependency(directory, filename), new FileDependency(directory, compressedFilename));
            }
            finally
            {
                File.Delete(lockpath);
            }
        }

        private static byte[] CompressBytes(Stream temp, CompressionMethodScheme compression)
        {
            CompressionType compressionType;
            switch (compression)
            {
                case CompressionMethodScheme.GZip:
                    compressionType = CompressionType.GZip;
                    break;
                case CompressionMethodScheme.Deflate:
                    compressionType = CompressionType.Deflate;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(compression), "For now only GZip and DEFLATE is supported.");
            }

            using (Stream compressedTemp = CompressionUtility.CompressStream(temp, compressionType))
            {
                return ByteConverter.FromStream(compressedTemp);
            }
        }

        /// <summary>
        /// Determines whether the specified <paramref name="physicalFilePath"/> can be written to the disk.
        /// </summary>
        /// <param name="physicalFilePath">The physical file path on the disk.</param>
        /// <returns><c>true</c> if the specified <paramref name="physicalFilePath"/> can be written to the disk; otherwise, <c>false</c>.</returns>
        protected bool CanWriteFileToDisk(string physicalFilePath)
        {
            bool hadException = false;
            string tempFile = null;
            try
            {
                tempFile = string.Concat(physicalFilePath, StringUtility.CreateRandomString(6) + ".tmp");
                using (StreamWriter writer = File.CreateText(tempFile))
                {
                    writer.Flush();
                }
                return true;
            }
            catch (Exception)
            {
                hadException = true;
                return false;
            }
            finally
            {
                if (!hadException && tempFile != null) { File.Delete(tempFile); }
            }
        }
    }
}