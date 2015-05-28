using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using Cuemon.IO;
using Cuemon.Security.Cryptography;

namespace Cuemon.Web
{
	/// <summary>
	/// A filter which stores a snapshot of the content from the chained output filter.
	/// </summary>
	/// <remarks>Normal usage of this filter would be in conjunction with the <see cref="HttpApplication.PostRequestHandlerExecute"/> event.</remarks>
	public class HttpResponseContentFilter : HttpResponseFilter
	{
	    private readonly MemoryStream _snapshotOfContent;
	    private readonly bool _bufferOutput;
	    private static readonly IList<FileMapping> _mimeTypesFilter = DefaultMimeTypesFilter();

	    private static IList<FileMapping> DefaultMimeTypesFilter()
	    {
            List<FileMapping> mappings = new List<FileMapping>();
            mappings.Add(new FileMapping("text/plain", ".asm", ".bas", ".c", ".cnf", ".cpp", ".h", ".map", ".txt", ".vcs", ".xdr"));
            mappings.Add(new FileMapping("text/css", ".css", ".less"));
            mappings.Add(new FileMapping("text/xml", ".disco", ".dll.config", ".dtd", ".exe.config", ".mno", ".vml", ".wsdl", ".xml", ".xsd", ".xsf", ".xsl", ".xslt"));
            mappings.Add(new FileMapping("text/html", ".htm", ".html", ".hxt"));
            mappings.Add(new FileMapping("application/x-javascript", ".js"));
            mappings.Add(new FileMapping("application/xhtml+xml", ".xhtml", ".xhtm", ".xht"));
            mappings.Add(new FileMapping("text/jscript", ".jsx"));
            mappings.Add(new FileMapping("text/richtext", ".rtx"));
            mappings.Add(new FileMapping("text/scriptlet", ".sct"));
            mappings.Add(new FileMapping("text/sgml", ".sgml"));
	        return mappings;
	    }

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="HttpResponseContentFilter"/> class.
		/// </summary>
		/// <param name="context">The context of the ASP.NET application.</param>
		public HttpResponseContentFilter(HttpApplication context) : base(context)
		{
            _snapshotOfContent = new MemoryStream(512);
		    _bufferOutput = context.Response.BufferOutput;
		}
		#endregion

		#region Properties
	    private bool CheckUseChainedFilter()
	    {
            HttpContext context = HttpContext.Current;
            if (context == null) { return false; }
            if (context.Items.Contains("wcfHttpResponseContentFilter")) { return false; }
            FileMapping mimeType = MimeUtility.ParseContentType(MimeTypesFilter, context.Response.ContentType);
	        if (mimeType == null)
	        {
	            context.Response.Filter = this.ChainedOutputFilter;
	            return true;
	        }
	        return false;
	    }

        /// <summary>
        /// Gets the MIME types to process for <see cref="SnapshotOfContent"/>.
        /// </summary>
        /// <value>The MIME types to process for <see cref="SnapshotOfContent"/>.</value>
        /// <remarks>
        /// This is a positive list of MIME types to process.
        /// </remarks>
	    public static IList<FileMapping> MimeTypesFilter
	    {
            get { return _mimeTypesFilter; }
	    }

        /// <summary>
        /// Gets a snapshot of the value specified on <see cref="HttpResponse.BufferOutput"/>.
        /// </summary>
        /// <value><c>true</c> if the output to client is buffered; otherwise, <c>false</c>.</value>
        /// <remarks>Should this value have been set to <c>false</c>, the <see cref="SnapshotOfContent"/> is not being written to.</remarks>
        public bool BufferOutput
        {
            get { return _bufferOutput; }
        }

        /// <summary>
        /// Gets a snapshot from <see cref="HttpResponse.Filter"/> of the content currently ready to be written to the client.
        /// </summary>
        /// <value>
        /// A snapshot from <see cref="HttpResponse.Filter"/> of the content currently ready to be written to the client.
        /// </value>
	    public Stream SnapshotOfContent
	    {
            get { return _snapshotOfContent; }
	    }
		#endregion

		#region Methods
        /// <summary>
        /// When overridden in a derived class, clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        public override void Flush()
        {
            bool skipSnapshot = this.CheckUseChainedFilter();
            if (this.BufferOutput && !skipSnapshot) { this.SnapshotOfContent.Flush(); }
            base.Flush();
        }

		/// <summary>
		/// Writes a block of bytes to the current stream using data read from buffer.
		/// </summary>
		/// <param name="buffer">The buffer to write data from.</param>
		/// <param name="offset">The byte offset in <paramref name="buffer"/> at which to begin writing from.</param>
		/// <param name="count">The maximum number of bytes to write.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer"/> is null.
		///   </exception>
		///   
		/// <exception cref="T:System.NotSupportedException">
		/// The stream does not support writing. For additional information see <see cref="P:System.IO.Stream.CanWrite"/>.
		/// -or-
		/// The current position is closer than <paramref name="count"/> bytes to the end of the stream, and the capacity cannot be modified.
		///   </exception>
		///   
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="offset"/> subtracted from the buffer length is less than <paramref name="count"/>.
		///   </exception>
		///   
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset"/> or <paramref name="count"/> are negative.
		///   </exception>
		///   
		/// <exception cref="T:System.IO.IOException">
		/// An I/O error occurs.
		///   </exception>
		public override void Write(byte[] buffer, int offset, int count)
		{
            bool skipSnapshot = this.CheckUseChainedFilter();
            if (this.BufferOutput && !skipSnapshot) { this.SnapshotOfContent.Write(buffer, offset, count); }
            base.Write(buffer, offset, count);
		}
		#endregion
    }
}