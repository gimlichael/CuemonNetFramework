using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security;
using System.Web;
using System.Web.Caching;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using Cuemon.Xml;
using Cuemon.Xml.XPath;
using Cuemon.Web.Configuration;
using System.Web.Configuration;
using System.Configuration;
namespace Cuemon.Web
{
    /// <summary>
    /// Represents a <see cref="Cuemon.Web.Website"/> with the appropriate application automatically resolved from the Cuemon XML section of your Web.config file.
    /// </summary>
    [XmlRoot(ElementName = "Website")]
    public sealed class DefaultWebsite : Website
    {
        private DefaultWebsite()
        {
        }

        private DefaultWebsite(HttpContext context) : base(context)
        {
        }

        /// <summary>
        /// Gets the website instance.
        /// </summary>
        /// <returns></returns>
        internal static Website GetInstance()
        {
            return new DefaultWebsite();
        }

        /// <summary>
        /// Gets the website instance.
        /// </summary>
        /// <param name="context">A reference to a <see cref="HttpContext"/> object.</param>
        /// <returns></returns>
        internal static Website GetInstance(HttpContext context)
        {
            return new DefaultWebsite(context);
        }
    }
}