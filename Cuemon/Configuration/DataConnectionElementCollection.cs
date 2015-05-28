using System;
using System.Configuration;
using Cuemon.Diagnostics;
namespace Cuemon.Configuration
{
    /// <summary>
    /// Represents a configuration element containing a collection of child elements (<see cref="T:Cuemon.Configuration.DataConnectionElement"></see>).
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface")]
    public class DataConnectionElementCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Creates a new <see cref="T:Cuemon.Configuration.DataConnectionElement"></see>.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:Cuemon.Configuration.DataConnectionElement"></see>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new DataConnectionElement();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element">The <see cref="T:Cuemon.Configuration.DataConnectionElement"></see> to return the key for.</param>
        /// <returns>
        /// An <see cref="T:System.Object"></see> that acts as the key for the specified <see cref="T:Cuemon.Configuration.DataConnectionElement"></see>.
        /// </returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DataConnectionElement)(element)).Name;
        }
    }
}