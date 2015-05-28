using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;
using Cuemon.Web.Configuration;
using Cuemon.Xml.Serialization;
namespace Cuemon.Web
{
    /// <summary>
    /// Represent a binding collection for a given Website and its affiliated ASP.NET application.
    /// </summary>
    [XmlRoot("Bindings")]
    [XmlSerialization(EnableAutomatedXmlSerialization = true)]
    public sealed class WebsiteBindingCollection : XmlSerialization, IEnumerable<WebsiteBinding>
    {
        private List<WebsiteBinding> _innerCollection;
        private readonly Website _website;
        private readonly object _instanceLock = new object();

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WebsiteBindingCollection"/> class.
        /// </summary>
        WebsiteBindingCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebsiteBindingCollection"/> class.
        /// </summary>
        /// <param name="website">An instance of the Website class.</param>
        internal WebsiteBindingCollection(Website website)
        {
            _website = website;
        }
        #endregion

        #region Properties
        private List<WebsiteBinding> InnerCollection
        {
            get
            {
                if (_innerCollection == null)
                {
                    lock (_instanceLock)
                    {
                        if (_innerCollection == null)
                        {
                            _innerCollection = new List<WebsiteBinding>();

                            foreach (WebsiteBindingElement element in this.Website.ConfigurationElement.Bindings)
                            {
                                _innerCollection.Add(new WebsiteBinding(element));
                            }
                        }
                    }
                }
                return _innerCollection;
            }
        }

        private Website Website
        {
            get
            {
                if (_website == null) { throw new InvalidOperationException("This object is not in a valid state - do not use parameterless constructor!"); }
                return _website;
            }
        }

        /// <summary>
        /// Gets the number of elements contained in this object.
        /// </summary>
        /// <value>The number of elements contained in this object.</value>
        [XmlIgnore]
        public int Count
        {
            get { return this.InnerCollection.Count; }
        }

        /// <summary>
        /// Gets the <see cref="Cuemon.Web.WebsiteBinding"/> at the specified index.
        /// </summary>
        /// <value></value>
         public WebsiteBinding this[int index]
        {
            get
            {
                return this.InnerCollection[index];
            }
        }

        /// <summary>
        /// Gets the <see cref="Cuemon.Web.WebsiteBinding"/> with the specified host header.
        /// </summary>
        /// <value></value>
        [XmlIgnore]
        public WebsiteBinding this[string hostHeader]
        {
            get
            {
                foreach (WebsiteBinding binding in this)
                {
                    if (hostHeader == binding.HostHeader) { return binding; }
                }
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "No Binding with the parameter value of '{0}' exists!", hostHeader), "hostHeader");
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<WebsiteBinding> GetEnumerator()
        {
            return this.InnerCollection.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        #endregion
    }
}