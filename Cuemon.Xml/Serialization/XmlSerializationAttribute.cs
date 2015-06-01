using System;
using System.Xml.Serialization;

namespace Cuemon.Xml.Serialization
{
	/// <summary>
	/// Specifies the serialization attribute to be used by the <see cref="XmlSerialization"/> class in default serialization scenarios.
	/// </summary>
	public enum SerializationMethod
	{
		/// <summary>
		/// Specifies the <see cref="XmlAttributeAttribute"/> attribute.
		/// </summary>
		XmlAttributeAttribute,
		/// <summary>
		/// Specifies the <see cref="XmlElementAttribute"/> attribute.
		/// </summary>
		XmlElementAttribute
	}

	/// <summary>
	/// Specifies that the <see cref="XmlSerialization"/> must take special action when serializing the affected class member.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class XmlSerializationAttribute : Attribute
	{
		private bool _enableAutomatedXmlSerialization;
		private SerializationMethod _defaultSerializationMethod = SerializationMethod.XmlElementAttribute;
		private bool _enableSignatureCaching;
		private bool _omitXmlDeclaration = true;

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="XmlSerializationAttribute"/> class.
		/// </summary>
		public XmlSerializationAttribute()
		{
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets a value indicating whether to write an XML declaration.
		/// </summary>
		/// <value>
		///   <c>true</c> omit the XML declaration; otherwise <c>false</c>. The default is true.
		/// </value>
		public bool OmitXmlDeclaration
		{
			get { return _omitXmlDeclaration; }
			set { _omitXmlDeclaration = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the object for serializing will have its signature computed and cached for faster retrieval in some scenarios.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the object for serializing will have its signature computed and cached for faster retrieval in some scenarios; otherwise, <c>false</c>.
		/// </value>
		public bool EnableSignatureCaching
		{
			get { return _enableSignatureCaching; }
			set { _enableSignatureCaching = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether to use automated XML serialization for your class.
		/// If not enabled, you must override the <see cref="IXmlSerializable"/> defined methods to apply your own custom XML serialization.
		/// Default is not enabled.
		/// </summary>
		/// <value>
        /// 	<c>true</c> to use automated XML serialization for your class; otherwise, <c>false</c>. The default is true.
		/// </value>
		public bool EnableAutomatedXmlSerialization
		{
			get { return _enableAutomatedXmlSerialization; }
			set { _enableAutomatedXmlSerialization = value; }
		}

		/// <summary>
		/// Gets or sets the serialization attribute to be used in default serialization scenarios.
		/// </summary>
		/// <value>The default serialization attribute to be used in default serialization scenarios.</value>
		public SerializationMethod DefaultSerializationMethod
		{
			get { return _defaultSerializationMethod; }
			set { _defaultSerializationMethod = value; }
		}

        /// <summary>
        /// Gets the default controlling XML serialization attribute as specified by <see cref="DefaultSerializationMethod"/>.
        /// </summary>
        /// <value>The default controlling XML serialization attribute as specified by <see cref="DefaultSerializationMethod"/>.</value>
	    public Type DefaultSerializationMethodAsAttribute
	    {
            get { return this.DefaultSerializationMethod == SerializationMethod.XmlAttributeAttribute ? typeof(XmlAttributeAttribute) : typeof(XmlElementAttribute); }
	    }
		#endregion
	}
}