using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.Web.Hosting;
using Cuemon.Caching;
using Cuemon.IO;
using Cuemon.IO.Compression;
using Cuemon.Threading;
using Cuemon.Web.Routing;

namespace Cuemon.Web
{
    public partial class GlobalModule
    {
        private const string CompressionCacheGroup = "ParseResourceCompression";

        /// <summary>
        /// Gets or sets a value indicating whether compression is enabled. Default is false.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if compression is enabled; otherwise, <c>false</c>.
        /// </value>
        public static bool EnableCompression
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="HandleCompressionHeaders(System.Web.HttpApplication)"/> should be invoked.
        /// </summary>
        /// <value><c>true</c> if <see cref="HandleCompressionHeaders(System.Web.HttpApplication)"/> should be invoked; otherwise, <c>false</c>.</value>
        public bool ParseCompressionHeaders { get; set; }

        /// <summary>
        /// Gets the client resolved compression method scheme.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <returns>The client resolved compression method scheme</returns>
        /// <remarks>
        /// Do note that <see cref="CompressionMethodScheme.Deflate"/> is avoided if possible. GZip is the de facto standard for HTTP compression.
        /// For more in-depth explanation of this choice, please refer to this link by Zoompf: http://zoompf.com/blog/2012/02/lose-the-wait-http-compression
        /// </remarks>
        protected CompressionMethodScheme GetClientCompressionMethod(HttpApplication context)
        {
            if (context == null) { throw new ArgumentNullException("context"); }
            return this.GetClientCompressionMethod(context.Request);
        }

        /// <summary>
        /// Gets the client resolved compression method scheme.
        /// </summary>
        /// <param name="request">An instance of the <see cref="HttpRequest"/> object.</param>
        /// <returns>The client resolved compression method scheme</returns>
        /// <remarks>
        /// Do note that <see cref="CompressionMethodScheme.Deflate"/> is avoided if possible. GZip is the de facto standard for HTTP compression.
        /// For more in-depth explanation of this choice, please refer to this link by Zoompf: http://zoompf.com/blog/2012/02/lose-the-wait-http-compression
        /// </remarks>
        protected CompressionMethodScheme GetClientCompressionMethod(HttpRequest request)
        {
            if (request == null) { throw new ArgumentNullException("request"); }
            return HttpRequestUtility.ParseAcceptEncoding(request);
        }

        /// <summary>
        /// Handles the compression headers of the <see cref="HttpResponse.OutputStream"/>.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is usually called from the <see cref="OnPreSendRequestHeaders"/> method and either sets or removes compression headers should <see cref="ParseCompressionHeaders"/> evaluate to <c>true</c>.</remarks>
        protected virtual void HandleCompressionHeaders(HttpApplication context)
        {
            if (!EnableCompression && !this.ParseCompressionHeaders) { return; }
            if (context == null) { throw new ArgumentNullException("context"); }
            if (HttpRequestUtility.IsStandaloneServerLocal(context.Request)) { return; }
            if (context.Response.SuppressContent) { return; }
            if (!this.IsValidForCompression(context)) {
                HttpResponseUtility.RemoveResponseHeader(context.Response, "Content-Encoding"); 
            }
            else
            {
                CompressionMethodScheme compressionMethod = this.GetClientCompressionMethod(context.Request);
                switch (compressionMethod)
                {
                    case CompressionMethodScheme.Deflate:
                    case CompressionMethodScheme.GZip:
                        HttpResponseUtility.SetClientCompressionMethod(context.Response, compressionMethod);
                        return;
                    case CompressionMethodScheme.Identity:
                    case CompressionMethodScheme.Compress:
                    case CompressionMethodScheme.None:
                        HttpResponseUtility.RemoveResponseHeader(context.Response, "Content-Encoding");
                        return;
                }
   
            }
        }

        /// <summary>
        /// Handles the initialization of compression for the <see cref="HttpResponse.OutputStream"/>.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <param name="compression">The compression method to apply to the HTTP Content-Encoding header.</param>
        protected virtual void InitializeCompression(HttpApplication context, CompressionMethodScheme compression)
        {
            if (!EnableCompression) { return; }
            if (context == null) { throw new ArgumentNullException("context"); }
            if (HttpRequestUtility.IsStandaloneServerLocal(context.Request)) { return; }
            if (!context.Response.BufferOutput) { return; }
            if (context.Response.SuppressContent) { return; }
            if (!this.IsValidForCompression(context)) { return; }

            HttpRoutePath path = new HttpRoutePath(context.Context);
            bool hasCachedResource = false;
            if (path.HasPhysicalFile && !path.IsHandler && path.PhysicalFileMimeType != null)
            {
                string resource = ParseResourceCompression(path, compression);
                string resourceExtension = GetCompressedFileExtension(path, compression);
                hasCachedResource = resource.EndsWith(resourceExtension, StringComparison.OrdinalIgnoreCase);
                if (hasCachedResource)
                {
                    context.Context.RewritePath(string.Format(CultureInfo.InvariantCulture, "{0}{1}", resource, path.Url.Query), false);
                }
                context.Response.ContentType = path.PhysicalFileMimeType.ContentType;
            }

            switch (compression)
            {
                case CompressionMethodScheme.Deflate:
                    if (!hasCachedResource) { context.Response.Filter = new DeflateStream(context.Response.Filter, CompressionMode.Compress); }
                    this.ParseCompressionHeaders = true;
                    break;
                case CompressionMethodScheme.GZip:
                    if (!hasCachedResource) { context.Response.Filter = new GZipStream(context.Response.Filter, CompressionMode.Compress); }
                    this.ParseCompressionHeaders = true;
                    break;
                case CompressionMethodScheme.Identity:
                case CompressionMethodScheme.Compress:
                case CompressionMethodScheme.None:
                    this.ParseCompressionHeaders = true;
                    break;
            }
        }


        /// <summary>
        /// Evaluates if the current request of the specified <paramref name="context"/> is valid for compression.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <returns><c>true</c> if the current request of the specified <paramref name="context"/> is valid for compression; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// The default implementation more or less follows the HTML5 Boilerplate Apache Server Config settings as seen on their project website: https://github.com/h5bp/html5-boilerplate/blob/master/dist/.htaccess.
        /// If the current request points to a handler, the content-type will be resolved from that (dynamic compression).
        /// </remarks>
        protected virtual bool IsValidForCompression(HttpApplication context)
        {
            if (context == null) { return false; }
            Doer<FileMapping, bool> compress = CachingManager.Cache.Memoize<FileMapping, bool>(IsValidForCompressionCore);
            HttpRoutePath path = new HttpRoutePath(context.Context);
            FileMapping mimeType = path.IsHandler ? MimeUtility.ParseContentType(context.Response.ContentType) : path.PhysicalFileMimeType;
            if (mimeType == null) { return false; }
            return compress(mimeType);
        }

        private bool IsValidForCompressionCore(FileMapping mimeType)
        {
            return ((mimeType.ContentType.StartsWith("text", StringComparison.OrdinalIgnoreCase) || mimeType.ContentType.StartsWith("font", StringComparison.OrdinalIgnoreCase)) ||
                (StringUtility.Contains(mimeType.ContentType, StringComparison.OrdinalIgnoreCase, "image/bmp", "image/svg+xml", "image/vnd.microsoft.icon", "image/x-icon")) ||
                (StringUtility.Contains(mimeType.ContentType, StringComparison.OrdinalIgnoreCase, "application/atom+xml", 
                "application/javascript", 
                "application/x-javascript",
                "application/json",
                "application/ld+json",
                "application/manifest+json",
                "application/rdf+xml",
                "application/rss+xml",
                "application/schema+json",
                "application/vnd.geo+json",
                "application/vnd.ms-fontobject",
                "application/x-font-ttf",
                "application/x-web-app-manifest+json",
                "application/xhtml+xml",
                "application/xml")));
        }

        private static string GetCompressedFileExtension(HttpRoutePath routePath, CompressionMethodScheme compression)
        {
            string extension = string.Concat(".", compression.ToString().ToLowerInvariant());
            string originalExtension = Path.GetExtension(routePath.PhysicalFilePath);
            return string.Concat(extension, originalExtension).ToLowerInvariant();
        }

        /// <summary>
        /// Returns a virtual file path of the specified <paramref name="pathToResource"/> while determining if a compressed equivalent can and should be written to the file system.
        /// </summary>
        /// <param name="pathToResource">The HTTP route path to a resource.</param>
        /// <param name="compression">The compression to apply to the <paramref name="pathToResource"/>.</param>
        /// <returns>A virtual file path of the specified <paramref name="pathToResource"/>.</returns>
        protected virtual string ParseResourceCompression(HttpRoutePath pathToResource, CompressionMethodScheme compression)
        {
            Validator.ThrowIfNull(pathToResource, "pathToResource");
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

            string pathToTempFile;
            string virtualPathToTempFile;
            if (!CachingManager.Cache.TryGetValue(pathToResource.VirtualFilePath, CompressionCacheGroup, out virtualPathToTempFile))
            {
                string newExtension = GetCompressedFileExtension(pathToResource, compression);
                pathToTempFile = Path.ChangeExtension(pathToResource.PhysicalFilePath, newExtension).ToLowerInvariant();
                virtualPathToTempFile = Path.ChangeExtension(pathToResource.VirtualFilePath, newExtension).ToLowerInvariant();
                ThreadPoolUtility.QueueWork(WriteFileQueued, pathToResource, pathToTempFile, compression, virtualPathToTempFile);
                return pathToResource.VirtualFilePath;
            }

            Doer<string, bool> fileExists = CachingManager.Cache.Memoize<string, bool>(File.Exists);
            Doer<string, string> mapPath = CachingManager.Cache.Memoize<string, string>(HostingEnvironment.MapPath);
            pathToTempFile = mapPath(virtualPathToTempFile);
            if (!fileExists(pathToTempFile))
            {
                CachingManager.Cache.Remove(pathToResource.VirtualFilePath, CompressionCacheGroup);
                return pathToResource.VirtualFilePath;
            }
            return virtualPathToTempFile; 
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

        private static void WriteFileQueued(HttpRoutePath path, string pathToTempFile, CompressionMethodScheme compression, string virtualPathToTempFile)
        {
            using (WindowsImpersonationContext wic = AppPoolIdentity.Impersonate())
            {
                string lockpath = string.Concat(path.PhysicalFilePath, ".locked");
                if (File.Exists(lockpath)) { return; }
                if (CachingManager.Cache.ContainsKey(path.VirtualFilePath, CompressionCacheGroup)) { return; }
                using (Mutex mutex = new Mutex(false, string.Concat(virtualPathToTempFile, "_mutex")))
                {
                    try
                    {
                        mutex.WaitOne();
                        using (FileStream temp = File.Create(lockpath)) { temp.Flush(); }
                        using (FileStream temp = File.OpenRead(path.PhysicalFilePath))
                        {
                            File.WriteAllBytes(pathToTempFile, CompressBytes(temp, compression));
                        }
                        string directory = Path.GetDirectoryName(path.PhysicalFilePath);
                        string filename = Path.GetFileName(path.PhysicalFilePath);
                        CachingManager.Cache.Add(path.VirtualFilePath, virtualPathToTempFile, CompressionCacheGroup, new FileDependency(directory, filename));
                    }
                    finally
                    {
                        File.Delete(lockpath);
                        mutex.ReleaseMutex();
                    }
                }
                wic.Undo();
            }
        }

        private static byte[] CompressBytes(Stream temp, CompressionMethodScheme compression)
        {
            switch (compression)
            {
                case CompressionMethodScheme.GZip:
                    using (Stream compressedTemp = CompressionUtility.CompressStream(temp, CompressionType.GZip))
                    {
                        return ConvertUtility.ToByteArray(compressedTemp);
                    }
                case CompressionMethodScheme.Deflate:
                    using (Stream compressedTemp = CompressionUtility.CompressStream(temp, CompressionType.Deflate))
                    {
                        return ConvertUtility.ToByteArray(compressedTemp);
                    }
                default:
                    throw new ArgumentOutOfRangeException("compression", "For now only GZip and DEFLATE is supported.");
            }
        }

        /// <summary>
        /// Parses and prepares the specified <paramref name="output"/> to be written to the <see cref="HttpResponse.BinaryWrite"/>.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <param name="output">The output to parse for optional compression.</param>
        /// <returns>A <b>byte</b> array that is either left untouched or a compressed equivalent of <paramref name="output"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="context"/> - or - <paramref name="output"/> is null.
        /// </exception>
        protected virtual byte[] ParseHttpOutputStream(HttpContext context, byte[] output)
        {
            if (context == null) { throw new ArgumentNullException("context"); }
            if (output == null) { throw new ArgumentNullException("output"); }
            return EnableCompression && this.IsValidForCompression(context.ApplicationInstance)
                ? this.ParseHttpOutputStream(output, this.GetClientCompressionMethod(context.Request)) 
                : output;
        }

        /// <summary>
        /// Parses and prepares the specified <paramref name="output"/> to be written to the <see cref="HttpResponse.BinaryWrite"/>.
        /// </summary>
        /// <param name="output">The output to parse for optional compression.</param>
        /// <param name="compressionMethod">The compression method to apply to the HTTP Content-Encoding header.</param>
        /// <returns>A <b>byte</b> array that is either left untouched or a compressed equivalent of <paramref name="output"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="output"/> is null.
        /// </exception>
        protected virtual byte[] ParseHttpOutputStream(byte[] output, CompressionMethodScheme compressionMethod)
        {
            if (output == null) { throw new ArgumentNullException("output"); }
            return EnableCompression 
                ? this.ParseHttpOutputStream(new MemoryStream(output), compressionMethod)
                : output;
        }

        /// <summary>
        /// Parses and prepares the specified <paramref name="output"/> to be written to the <see cref="HttpResponse.BinaryWrite"/>.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <param name="output">The output to parse for optional compression.</param>
        /// <returns>A <b>byte</b> array that is either left untouched or a compressed equivalent of <paramref name="output"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="context"/> - or - <paramref name="output"/> is null.
        /// </exception>
        protected virtual byte[] ParseHttpOutputStream(HttpContext context, Stream output)
        {
            if (context == null) { throw new ArgumentNullException("context"); }
            if (output == null) { throw new ArgumentNullException("output"); }
            return EnableCompression && this.IsValidForCompression(context.ApplicationInstance)
                ? this.ParseHttpOutputStream(output, this.GetClientCompressionMethod(context.Request)) 
                : ConvertUtility.ToByteArray(output);
        }

        /// <summary>
        /// Parses and prepares the specified <paramref name="output"/> to be written to the <see cref="HttpResponse.BinaryWrite"/>.
        /// </summary>
        /// <param name="output">The output to parse for optional compression.</param>
        /// <param name="compressionMethod">The compression method to apply to the HTTP Content-Encoding header.</param>
        /// <returns>A <b>byte</b> array that is either left untouched or a compressed equivalent of <paramref name="output"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="output"/> is null.
        /// </exception>
        protected virtual byte[] ParseHttpOutputStream(Stream output, CompressionMethodScheme compressionMethod)
        {
            if (output == null) { throw new ArgumentNullException("output"); }
            if (EnableCompression)
            {
                CompressionType? compressionType = null;
                switch (compressionMethod)
                {
                    case CompressionMethodScheme.GZip:
                        compressionType = CompressionType.GZip;
                        break;
                    case CompressionMethodScheme.Deflate:
                        compressionType = CompressionType.Deflate;
                        break;
                }

                if (compressionType.HasValue)
                {
                    Stream compressedOutput = CompressionUtility.CompressStream(output, compressionType.Value);
                    return ConvertUtility.ToByteArray(compressedOutput);
                }
            }
            return ConvertUtility.ToByteArray(output);
        }
    }
}