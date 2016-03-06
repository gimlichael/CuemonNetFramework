using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.XPath;
using Cuemon.Xml;

namespace Cuemon.Web
{
    public partial class GlobalModule
    {
        /// <summary>
        /// Gets or sets a value indicating whether exceptions should be intercepted and converted to XML. Default is false.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if exceptions should be intercepted and converted to XML; otherwise, <c>false</c>.
        /// </value>
        public static bool EnableExceptionToXmlInterception
        {
            get; set;
        }

        /// <summary>
        /// Handles the exception interception. Especially useful for XML web services.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>
        /// This method controls the exception interception for both tradition ASP.NET runtime and the Windows Communication Foundation runtime.<br/>
        /// If <see cref="EnableExceptionToXmlInterception"/> is set to <c>true</c>, all content from the output buffer will be cleared in case of an unhandled exception and the exception itself will be written to the buffer stream as XML.
        /// </remarks>
        protected void HandleExceptionInterception(HttpApplication context)
        {
            this.HandleExceptionInterception(context, false);
        }

        /// <summary>
        /// Handles the interception of an unhandled exception.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <param name="includeStackTrace">if set to <c>true</c> the stack trace of the exception is included in the rendered result.</param>
        /// <remarks>
        /// If <see cref="EnableExceptionToXmlInterception"/> is set to <c>true</c>, all content from the output buffer will be cleared in case of an unhandled exception and the exception itself will be written to the buffer stream as XML.
        /// </remarks>
        protected virtual void HandleExceptionInterception(HttpApplication context, bool includeStackTrace)
        {
            if (context == null) { throw new ArgumentNullException(nameof(context)); }
            Exception lastException = context.Context.Error;
            if (lastException != null)
            {
                XPathNavigator navigator = XmlConvertUtility.ToXmlElement(lastException, context.Response.ContentEncoding, includeStackTrace).CreateNavigator();
                using (Stream exceptionAsXml = XmlWriterUtility.CreateXml(ExceptionWriter, navigator, context.Response.ContentEncoding))
                {
                    byte[] outputInBytes = this.ParseHttpOutputStream(context.Context, exceptionAsXml);
                    context.Response.Clear();
                    context.Response.ContentType = "application/xml";
                    context.Response.BinaryWrite(outputInBytes);
                }
            }
        }

        /// <summary>
        /// Provides a way to write and refine exceptions thrown from the <see cref="HandleExceptionInterception(System.Web.HttpApplication)"/>.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/> stream to which the <paramref name="navigable"/> is serialized.</param>
        /// <param name="navigable">The XML holding the <see cref="Exception"/> details.</param>
        /// <param name="encoding">The encoding the <paramref name="writer"/> is configured to use.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="writer"/> - or - <paramref name="navigable"/> - or - <paramref name="encoding"/> is null.
        /// </exception>
	    protected virtual void ExceptionWriter(XmlWriter writer, IXPathNavigable navigable, Encoding encoding)
        {
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
            if (navigable == null) { throw new ArgumentNullException(nameof(navigable)); }
            if (encoding == null) { throw new ArgumentNullException(nameof(encoding)); }
            writer.WriteProcessingInstruction("xml", string.Format(CultureInfo.InvariantCulture, "version=\"1.0\" encoding=\"{0}\"", encoding.WebName));
            writer.WriteNode(navigable.CreateNavigator(), true);
            writer.Flush();
        }
    }
}