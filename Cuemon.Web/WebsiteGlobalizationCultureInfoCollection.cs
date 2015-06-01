using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Cuemon.Web.Configuration;
using Cuemon.Xml.Serialization;

namespace Cuemon.Web
{
    /// <summary>
    /// Represent a culture info collection for a given Website and its affiliated ASP.NET application.
    /// </summary>
    [XmlRoot("CultureInfos")]
    public sealed class WebsiteGlobalizationCultureInfoCollection : XmlSerialization, IEnumerable<WebsiteGlobalizationCultureInfo>
    {
        private static readonly List<WebsiteGlobalizationCultureInfo> InnerCollection = new List<WebsiteGlobalizationCultureInfo>();
        private readonly WebsiteGlobalization _globalization;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WebsiteGlobalizationCultureInfoCollection"/> class.
        /// </summary>
        WebsiteGlobalizationCultureInfoCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebsiteGlobalizationCultureInfoCollection"/> class.
        /// </summary>
        /// <param name="globalization">An instance of the <see cref="WebsiteGlobalization"/> class.</param>
        internal WebsiteGlobalizationCultureInfoCollection(WebsiteGlobalization globalization)
        {
            _globalization = globalization;
            if (InnerCollection.Count > 0) { return; }
            lock (InnerCollection)
            {
                if (InnerCollection.Count > 0) { return; }
                foreach (WebsiteGlobalizationCultureInfoElement element in this.Globalization.Website.ConfigurationElement.Globalization.CultureInfos)
                {
                    InnerCollection.Add(new WebsiteGlobalizationCultureInfo(this.Globalization, element));
                }
            }
        }
        #endregion

        #region Properties
        private WebsiteGlobalization Globalization
        {
            get 
            {
                if (_globalization == null) { throw new InvalidOperationException("This object is not in a valid state - do not use parameterless constructor!"); }
                return _globalization; 
            }
        }

        /// <summary>
        /// Gets the number of elements contained in this object.
        /// </summary>
        /// <value>The number of elements contained in this object.</value>
        public int Count
        {
            get { return InnerCollection.Count; }
        }

        /// <summary>
        /// Gets the <see cref="Cuemon.Web.WebsiteGlobalizationCultureInfo"/> at the specified index.
        /// </summary>
        /// <value></value>
        public WebsiteGlobalizationCultureInfo this[int index]
        {
            get 
            {
                return InnerCollection[index];
            }
        }

        /// <summary>
        /// Gets the <see cref="Cuemon.Web.WebsiteGlobalizationCultureInfo"/> from the specified LCID.
        /// </summary>
        /// <value></value>
        public WebsiteGlobalizationCultureInfo this[string lcid]
        {
            get
            {
                foreach (WebsiteGlobalizationCultureInfo cultureInfo in this)
                {
                    if (cultureInfo.LCID.ToString() == lcid)
                    {
                        return cultureInfo;
                    }
                }
                throw new ArgumentException("There is no culture info available from the specified lcid indexer.");
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
        public IEnumerator<WebsiteGlobalizationCultureInfo> GetEnumerator()
        {
            return InnerCollection.GetEnumerator();
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

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"></see> stream from which the object is deserialized.</param>
        public override void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"></see> stream to which the object is serialized.</param>
        public override void WriteXml(XmlWriter writer)
        {
            if (writer == null) throw new ArgumentNullException("writer");
            foreach (WebsiteGlobalizationCultureInfo cultureInfo in this)
            {
                writer.WriteRaw(cultureInfo.ToString(cultureInfo.ToXml(true), PreambleSequence.Remove));
            }
        }

        #endregion
    }
}