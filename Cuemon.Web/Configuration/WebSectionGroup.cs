using System;
using System.Configuration;
using Cuemon.Configuration;
namespace Cuemon.Web.Configuration
{
    /// <summary>
    /// Represents a group of related sections within a configuration file (Web).
    /// </summary>
    public class WebSectionGroup : SectionGroup
    {
        /// <summary>
        /// Gets the web section group element.
        /// </summary>
        /// <value>The web section group element.</value>
        public WebSection Web
        {
            get
            {
                return (WebSection)this.Sections["Web"];
            }
        }
    }
}
