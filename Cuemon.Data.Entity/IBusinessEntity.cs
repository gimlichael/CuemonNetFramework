using System;
using System.ComponentModel;
using System.Xml.XPath;
using Cuemon.Xml.Serialization;
namespace Cuemon.Data.Entity
{
    /// <summary>
    /// The following tables list the members exposed by the IBusinessEntity type.
    /// </summary>
    public interface IBusinessEntity : IFormatProvider, IXPathNavigable, IXmlSerialization, IBusinessEntityDataMapped, INotifyPropertyChanged
    {
        /// <summary>
        /// Gets a value indicating whether this object has changed.
        /// </summary>
        /// <value><c>true</c> if the content has changed; otherwise, <c>false</c>.</value>
        bool IsDirty { get; }

    	/// <summary>
    	/// Gets a value indicating whether this instance is new.
    	/// </summary>
    	/// <value><c>true</c> if this instance is new; otherwise, <c>false</c>.</value>
    	bool IsNew { get; }
    }
}