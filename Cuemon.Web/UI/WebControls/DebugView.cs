using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Web.UI.WebControls;
using System.Xml;
using Cuemon.Xml;

namespace Cuemon.Web.UI.WebControls
{
    /// <summary>
    /// Displays the debug view of a Cuemon enabled website.
    /// </summary>
    [Description("Displays the debug view of a Cuemon enabled website.")]
    public class DebugView : WebControl
    {
        private string _pageName = null;
        private string _debugXml = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="DebugView"/> class.
        /// </summary>
        public DebugView()
        {
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            _pageName = this.Page.Server.UrlDecode(this.Page.Request.QueryString["page"]);
            this.Initialize();
            base.OnLoad(e);
        }

        private void Initialize()
        {
            if (WebsiteUtility.FastSession != null)
            {
                if (this.PageName == null) { return; }
                string debugKey = string.Format(CultureInfo.InvariantCulture, "{0}{1}", WebsiteUtility.CuemonDebugViewKey, this.PageName.ToLowerInvariant());
                if (WebsiteUtility.FastSession[debugKey] != null)
                {
                    XmlDocument document = new XmlDocument();
                    document.LoadXml((string)WebsiteUtility.FastSession[debugKey]);
                    using (MemoryStream stream = new MemoryStream())
                    {
                        document.Save(stream);
                        stream.Position = 0;
                        using (Stream output = XmlUtility.ConvertEncoding(stream, this.Page.Response.ContentEncoding, PreambleSequence.Remove))
                        {
                            this.DebugXml = StringConverter.FromStream(output, options =>
                            {
                                options.Encoding = Page.Response.ContentEncoding;
                                options.Preamble = PreambleSequence.Remove;
                            });
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the associated debug XML for the requested <see cref="PageName"/>.
        /// </summary>
        /// <value>The associated debug XML for the requested <see cref="PageName"/>.</value>
        public string DebugXml
        {
            get { return _debugXml; }
            private set { _debugXml = value; }
        }

        /// <summary>
        /// Gets the name of the page to fetch debug information for.
        /// </summary>
        /// <value>The name of the page to fetch debug information for.</value>
        public string PageName
        {
            get { return _pageName; }
        }
    }
}
