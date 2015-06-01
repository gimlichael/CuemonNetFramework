using System.Configuration;

namespace Cuemon.Web.Configuration
{
    /// <summary>
    /// Represents a configuration element containing a collection of child elements (<see cref="T:Cuemon.Web.Configuration.WebsiteDataConnectionElement"></see>).
    /// </summary>
    public class WebsiteDataConnectionElementCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Creates a new <see cref="T:Cuemon.Web.Configuration.WebsiteDataConnectionElement"></see>.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:Cuemon.Web.Configuration.WebsiteDataConnectionElement"></see>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new WebsiteDataConnectionElement();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element">The <see cref="T:Cuemon.Web.Configuration.WebsiteDataConnectionElement"></see> to return the key for.</param>
        /// <returns>
        /// An <see cref="T:System.Object"></see> that acts as the key for the specified <see cref="T:Cuemon.Web.Configuration.WebsiteDataConnectionElement"></see>.
        /// </returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((WebsiteDataConnectionElement)(element)).Name;
        }
    }
}