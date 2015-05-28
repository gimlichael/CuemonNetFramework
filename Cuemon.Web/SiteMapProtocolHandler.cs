using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.SessionState;
using System.Xml;
using System.Xml.XPath;
using Cuemon.Caching;
using Cuemon.IO;
using Cuemon.Net;
using Cuemon.Net.Http;
using Cuemon.Xml;

namespace Cuemon.Web
{
    /// <summary>
    /// Initializes the sitemaps.org protocol for a Cuemon enabled ASP.NET website.
    /// For more information in regards to the sitemaps.org protocol, please visit: http://sitemaps.org/protocol.php.
    /// </summary>
    public class SiteMapProtocolHandler : IHttpHandler, IRequiresSessionState // IHttpAsyncHandler : had to change to IHttpHandler, as the Async does not use the same user credentials as the website
    {
        private static readonly object locker = new object();
        internal const string SitemapXmlFilename = "sitemap.xml";
        internal const string CacheGroupName = "Cuemon.Web.SiteMapProtocolHandler";

        private static void WriteSeoData(XmlWriter writer, string fullyQualifiedUrl)
        {
            writer.WriteElementString("loc", fullyQualifiedUrl); // always write the location, as it is mandatory
            using (HttpWebResponse response = NetHttpUtility.HttpHead(new Uri(fullyQualifiedUrl)))
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string lastModified = response.GetResponseHeader("Last-Modified");
                    if (!string.IsNullOrEmpty(lastModified))
                    {
                        DateTime utcLastModified;
                        if (DateTime.TryParse(lastModified, out utcLastModified))
                        {
                            utcLastModified = utcLastModified.ToUniversalTime();
                            writer.WriteElementString("lastmod", utcLastModified.ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture));
                        }
                    }

                    string changeFrequency = response.GetResponseHeader("X-Change-Frequency");
                    if (!string.IsNullOrEmpty(changeFrequency))
                    {
                        writer.WriteElementString("changefreq", changeFrequency.ToLowerInvariant());
                    }

                    string priority = response.GetResponseHeader("X-Crawler-Priority");
                    if (!string.IsNullOrEmpty(priority))
                    {
                        writer.WriteElementString("priority", priority.ToString(CultureInfo.InvariantCulture));
                    }
                }

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    writer.WriteElementString("changefreq", "never");
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler"/> instance.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Web.IHttpHandler"/> instance is reusable; otherwise, false.
        /// </returns>
        public bool IsReusable
        {
            get { return false; }
        }

        private void BuildSitemapProtocol(HttpContext context)
        {
            if (!CachingManager.Cache.ContainsKey(SitemapXmlFilename, CacheGroupName))
            {
                lock (locker)
                {
                    if (!CachingManager.Cache.ContainsKey(SitemapXmlFilename, CacheGroupName))
                    {
                        MemoryStream output = null;
                        MemoryStream tempOutput = null;
                        try
                        {
                            tempOutput = new MemoryStream();
                            using (XmlWriter writer = XmlWriter.Create(tempOutput, XmlWriterUtility.CreateSettings(Encoding.UTF8, false)))
                            {
                                writer.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");
                                Website website = Website.Create(context);
                                // todo: exception handling
                                foreach (WebsiteGlobalizationCultureInfo cultureInfo in website.Globalization.CultureInfos)
                                {
                                    XPathNavigator navigator = Website.SiteMapFiles[cultureInfo.LCID].CreateNavigator();
                                    using (XmlReader reader = navigator.ReadSubtree())
                                    {
                                        while (reader.Read())
                                        {
                                            switch (reader.NodeType)
                                            {
                                                case XmlNodeType.Element:
                                                    switch (reader.LocalName)
                                                    {
                                                        case "Page":
                                                            string url = reader.GetAttribute("friendlyName") ?? reader.GetAttribute("name");
                                                            bool isUrlOriginallyFullyQualified = UriUtility.IsUri(url, UriKind.Absolute, ConvertUtility.ToArray(UriScheme.Http, UriScheme.Https));
                                                            string fullyQualifiedUrl = string.Format(CultureInfo.InvariantCulture, context.Request.Url.IsDefaultPort ? "{0}{1}{2}{4}" : "{0}{1}{2}:{3}{4}", 
                                                                context.Request.Url.Scheme,
                                                                Uri.SchemeDelimiter,
                                                                context.Request.Url.Host,
                                                                context.Request.Url.Port,
                                                                XmlUtility.Escape(url));
                                                            writer.WriteStartElement("url");
                                                            if (!string.IsNullOrEmpty(url))
                                                            {
                                                                WriteSeoData(writer, isUrlOriginallyFullyQualified ? url : fullyQualifiedUrl);
                                                            }
                                                            writer.WriteEndElement();
                                                            break;
                                                    }
                                                    break;
                                            }
                                        }
                                    }
                                }
                                writer.WriteEndElement();
                                writer.Flush();
                                tempOutput.Position = 0;
                                output = tempOutput;
                                tempOutput = null;
                                CachingManager.Cache.Add(SitemapXmlFilename, output.ToArray(), CacheGroupName, TimeSpan.FromHours(4)); // cache, but expire it after 4 hours with no explicit call to it (since we the sitemap is rather static in nature, we use this over dependencies)
                            }
                        }
                        finally
                        {
                            if (tempOutput != null) { tempOutput.Dispose(); }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"/> interface.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpContext"/> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
        public void ProcessRequest(HttpContext context)
        {
            if (context == null) { throw new ArgumentNullException("context"); }
            this.BuildSitemapProtocol(context);
            WriteSiteMapProtocol(context.ApplicationInstance, CachingManager.Cache.Get<byte[]>(SiteMapProtocolHandler.SitemapXmlFilename, SiteMapProtocolHandler.CacheGroupName));
        }

        private static void WriteSiteMapProtocol(HttpApplication application, byte[] bytes)
        {
            application.Response.Clear();
            application.Response.ContentType = "application/xml";
            application.Response.BinaryWrite(bytes);
            application.CompleteRequest();
        }
    }
}