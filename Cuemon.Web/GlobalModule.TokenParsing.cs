using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Cuemon.Caching;
using Cuemon.IO;
using Cuemon.IO.Compression;
using Cuemon.Security.Cryptography;

namespace Cuemon.Web
{
	public partial class GlobalModule
	{
        internal static CacheValidator MostSignificantValidator = CacheValidator.Default;

		/// <summary>
		/// Gets or sets a value indicating whether token parsing of HTML-related content is enabled for guaranteed client-caching refresh. Default is false.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if token parsing of HTML-related content is enabled for guaranteed client-caching refresh; otherwise, <c>false</c>.
		/// </value>
		public static bool EnableTokenParsingForClientCaching
		{
		    get; set;
		}

		/// <summary>
		/// Handles the parsing of HTML related content. Argument {0} is replaced with a computed checksum.
		/// </summary>
		/// <param name="context">The context of the ASP.NET application.</param>
		/// <remarks>
		/// This method will resolve a <see cref="CacheValidator"/> from these prioritized implementations:
        /// <list type="table">
        ///     <listheader>
        ///         <term>Priority</term>
        ///         <description>Implementation</description>
        ///     </listheader>
        ///     <item>
        ///         <term>1</term>
        ///         <description><c><see cref="ICacheableHttpHandler"/></c></description>
        ///     </item>
        ///     <item>
        ///         <term>2</term>
        ///         <description><see cref="ISearchEngineOptimizer"/></description>
        ///     </item>
        ///     <item>
        ///         <term>3</term>
        ///         <description>Lookup the physical file (if possible) for the current request.</description>
        ///     </item>
        /// </list>
        /// Should none of the above be successful an instance of <see cref="CacheValidator.Default"/> is used.
		/// </remarks>
		protected void HandleHtmlRelatedContentParsing(HttpApplication context)
		{
			if (context == null) { throw new ArgumentNullException("context"); }
		    CacheValidator validator = GetCacheValidator(context.Request, context.Context.Handler ?? context.Context.CurrentHandler);
            MostSignificantValidator = CacheValidator.GetMostSignificant(MostSignificantValidator, validator);
            this.HandleHtmlRelatedContentParsing(context, MostSignificantValidator, DefaultHtmlOutputParser);
		}

		/// <summary>
        /// Handles the parsing of the HTML related content and uses <see cref="CacheValidator.Checksum"/> from the specified <paramref name="validator"/> to associate with the content being processed. Argument {0} is replaced with the <see cref="CacheValidator.Checksum"/> by the default <paramref name="htmlOutputParser"/>.
		/// </summary>
		/// <param name="context">The context of the ASP.NET application.</param>
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        /// <param name="htmlOutputParser">The function delegate that will apply the checksum of <paramref name="validator"/> to the output of a HTML related HTTP response.</param>
        protected virtual void HandleHtmlRelatedContentParsing(HttpApplication context, CacheValidator validator, Doer<Stream, Encoding, CacheValidator, Stream> htmlOutputParser)
		{
			if (context == null) { throw new ArgumentNullException("context"); }
            if (validator == null) { throw new ArgumentNullException("validator"); }
            if (htmlOutputParser == null) { throw new ArgumentNullException("htmlOutputParser"); }
            if (context.Response.StatusCode == (int)HttpStatusCode.MovedPermanently ||
                context.Response.StatusCode == (int)HttpStatusCode.Redirect)
            {
                HttpResponseUtility.DisableClientSideResourceCache(context.Response);
                return;
            }
            if (!this.IsHtmlRelatedContent(context)) { return; }
			HttpResponseContentFilter filter = context.Response.Filter as HttpResponseContentFilter;
			if (filter == null) { return; }
            Stream content = htmlOutputParser(filter.SnapshotOfContent, context.Response.ContentEncoding, validator);
            if (content == null) { return; }
            if (content.Length == 0) { return; }

            CompressionMethodScheme compression = CompressionMethodScheme.None;
            if (EnableCompression && this.IsValidForCompression(context))
            {
                string contentEncoding = HasIisIntegratedPipelineMode
                                                ? context.Response.Headers["Content-Encoding"] ?? this.GetClientCompressionMethod(context).ToString()
                                                : this.GetClientCompressionMethod(context).ToString();

                if (!String.IsNullOrEmpty(contentEncoding))
                {
                    switch (contentEncoding.ToLowerInvariant())
                    {
                        case "deflate":
                            compression = CompressionMethodScheme.Deflate;
                            break;
                        case "gzip":
                            compression = CompressionMethodScheme.GZip;
                            break;
                    }
                }
            }
            this.ParseWriteHtmlRelatedContent(context, content, compression);
		}

	    private static Stream DefaultHtmlOutputParser(Stream content, Encoding encoding, CacheValidator validator)
	    {
            if (content == null) { return content; }
            if (content.Length == 0) { return content; }
            string contentAsString = ConvertUtility.ToString(content, PreambleSequence.Remove, encoding, true);
            int arguments = 0;
            if (StringUtility.ParseFormat(contentAsString, out arguments))
            {
                contentAsString = contentAsString.Replace("{0}", validator.Checksum);
                return new MemoryStream(encoding.GetBytes(contentAsString));
            }
            return content;
	    }

		private void ParseWriteHtmlRelatedContent(HttpApplication context, Stream content, CompressionMethodScheme compression)
		{
            byte[] output = this.ParseHttpOutputStream(content, compression);
            context.Response.ClearContent();
            context.Response.BinaryWrite(output);
		}

		/// <summary>
		/// Determines whether the <see cref="HttpResponse.ContentType"/> of the specified <paramref name="context"/> is suitable for token parsing of HTML-related content for guaranteed client-caching refresh.
		/// </summary>
		/// <param name="context">The context of the ASP.NET application.</param>
		/// <returns>
		///   <c>true</c> if the <see cref="HttpResponse.ContentType"/> of the specified <paramref name="context"/> is suitable for token parsing of HTML-related content for guaranteed client-caching refresh; otherwise, <c>false</c>.
		/// </returns>
		protected virtual bool IsHtmlRelatedContent(HttpApplication context)
		{
			if (context == null) { throw new ArgumentNullException("context"); }
            if (context.Response.ContentType  == null) { return false; }
            FileMapping mimeType = MimeUtility.ParseContentType(HttpResponseContentFilter.MimeTypesFilter, context.Response.ContentType);
		    return (mimeType != null);
		}
	}
}