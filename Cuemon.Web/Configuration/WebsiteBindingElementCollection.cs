using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Cuemon.Web.Configuration
{
    /// <summary>
    /// Represents a configuration element containing a collection of child elements (<see cref="T:Cuemon.Web.Configuration.WebsiteBindingElement"></see>).
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface")]
    public sealed class WebsiteBindingElementCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Creates a new <see cref="T:Cuemon.Web.Configuration.WebsiteBindingElement"></see>.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:Cuemon.Web.Configuration.WebsiteBindingElement"></see>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new WebsiteBindingElement();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element">The <see cref="T:Cuemon.Web.Configuration.WebsiteBindingElement"></see> to return the key for.</param>
        /// <returns>
        /// An <see cref="T:System.Object"></see> that acts as the key for the specified <see cref="T:Cuemon.Web.Configuration.WebsiteBindingElement"></see>.
        /// </returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
			WebsiteBindingElement websiteBindingElement = element as WebsiteBindingElement;
            return string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", websiteBindingElement.ProtocolType, Uri.SchemeDelimiter, websiteBindingElement.HostHeader);
        }


        /// <summary>
        /// Determines whether the specified host header has the necessary binding.
        /// </summary>
        /// <param name="hostHeader">The host header to search for.</param>
        /// <returns>
        /// 	<c>true</c> if the specified host header has the necessary binding; otherwise, <c>false</c>.
        /// </returns>
        public bool HasBinding(string hostHeader)
        {
            if (hostHeader == null) throw new ArgumentNullException("hostHeader");
            foreach (WebsiteBindingElement bindingElement in this)
            {
                if (bindingElement.HostHeader.ToUpperInvariant() == hostHeader.ToUpperInvariant())
                {
                    return true;
                }
            }
            return false;
        }
    }
}