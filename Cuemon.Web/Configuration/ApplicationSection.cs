using System;
using System.Configuration;
using Cuemon.Web.Configuration;
namespace Cuemon.Web.Configuration
{
    /// <summary>
    /// Represents a Cuemon/Web/%APPLICATION_NAME% configuration element within a configuration file.
    /// </summary>
    public class ApplicationSection : ConfigurationSection
    {
        /// <summary>
        /// Gets the available websites from your configuration file.
        /// </summary>
        /// <value>The websites as entered in your configuration file.</value>
        [ConfigurationProperty("Websites")]
        public virtual WebsiteElementCollection Websites
        {
            get
            {
                return (WebsiteElementCollection)base["Websites"];
            }
        }

        /// <summary>
        /// Gets the website from collection that matches the given host header.
        /// </summary>
        /// <param name="hostHeader">The host header to retreive a website from.</param>
        /// <returns>An <see cref="Cuemon.Web.Configuration.WebsiteElement"/> object.</returns>
        public WebsiteElement GetWebsiteFromCollection(string hostHeader)
        {
            foreach (WebsiteElement website in this.Websites)
            {
                if (website.Bindings.HasBinding(hostHeader))
                {
                    return website;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the website from collection that matches the given identifier.
        /// </summary>
        /// <param name="id">The website identifier.</param>
        /// <returns>An <see cref="Cuemon.Web.Configuration.WebsiteElement"/> object.</returns>
        public WebsiteElement GetWebsiteFromCollection(Guid id)
        {
            foreach (WebsiteElement website in this.Websites)
            {
                if (website.Id == id)
                {
                    return website;
                }
            }
            return null;
        }
    }
}