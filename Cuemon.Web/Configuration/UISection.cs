using System;
using System.Configuration;
using Cuemon.Web.Configuration.UI;
namespace Cuemon.Web.Configuration
{
    /// <summary>
    /// Represents a Cuemon/Web/UI configuration element within a configuration file.
    /// </summary>
    public sealed class UISection : ConfigurationSection
    {
        /// <summary>
        /// Gets the Page element from your configuration file.
        /// </summary>
        /// <value>The Page element from your configuration file.</value>
        [ConfigurationProperty("Page")]
        public PageElement Page
        {
            get
            {
                return (PageElement)base["Page"];
            }
        }
    }
}