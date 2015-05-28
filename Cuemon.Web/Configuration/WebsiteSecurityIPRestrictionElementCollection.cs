using System;
using System.Configuration;
namespace Cuemon.Web.Configuration
{
    /// <summary>
    /// Represents a configuration element containing a collection of child elements (<see cref="T:Cuemon.Web.Configuration.WebsiteSecurityIPRestrictionElement"></see>).
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface")]
    public sealed class WebsiteSecurityIPRestrictionElementCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Creates a new <see cref="T:Cuemon.Web.Configuration.WebsiteSecurityIPRestrictionElement"></see>.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:Cuemon.Web.Configuration.WebsiteSecurityIPRestrictionElement"></see>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new WebsiteSecurityIPRestrictionElement();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element">The <see cref="T:Cuemon.Web.Configuration.WebsiteSecurityIPRestrictionElement"></see> to return the key for.</param>
        /// <returns>
        /// An <see cref="T:System.Object"></see> that acts as the key for the specified <see cref="T:Cuemon.Web.Configuration.WebsiteSecurityIPRestrictionElement"></see>.
        /// </returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((WebsiteSecurityIPRestrictionElement)(element)).RemoteAddress;
        }
    }
}