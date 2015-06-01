using System;
using System.Xml.Serialization;
using Cuemon.Web.Configuration;
using Cuemon.Xml.Serialization;

namespace Cuemon.Web
{
	/// <summary>
	/// Represents the binding information for a <see cref="Cuemon.Web.Website"/>.
	/// </summary>
	[XmlRoot("Binding")]
	[XmlSerialization(DefaultSerializationMethod = SerializationMethod.XmlAttributeAttribute, EnableAutomatedXmlSerialization = true)]
	public sealed class WebsiteBinding : XmlSerialization
	{
		private readonly string _protocolType;
		private readonly string _hostHeader;

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="WebsiteBinding"/> class.
		/// </summary>
		WebsiteBinding()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WebsiteBinding"/> class.
		/// </summary>
		/// <param name="bindingElement">A valid Binding configuration element.</param>
		internal WebsiteBinding(WebsiteBindingElement bindingElement) : this(bindingElement.ProtocolType, bindingElement.HostHeader)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WebsiteBinding"/> class.
		/// </summary>
		/// <param name="protocolType">The protocol type (http/https) for this binding.</param>
		/// <param name="hostHeader">The host header information for this binding.</param>
		public WebsiteBinding(string protocolType, string hostHeader)
		{
			if (protocolType == null) { throw new ArgumentNullException("protocolType"); }
			if (hostHeader == null) { throw new ArgumentNullException("hostHeader"); }
			if (!protocolType.Equals("http", StringComparison.OrdinalIgnoreCase) &&
				!protocolType.Equals("https", StringComparison.OrdinalIgnoreCase)) { throw new ArgumentException("Binding is allowed only by the http or https protocol."); }
			_protocolType = protocolType;
			_hostHeader = hostHeader;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the protocol type of this binding.
		/// </summary>
		/// <value>The protocol type of this binding.</value>
		[XmlAttribute("type")]
		public string ProtocolType
		{
			get { return _protocolType; }
		}

		/// <summary>
		/// Gets the host header of this binding.
		/// </summary>
		/// <value>The host header of this binding.</value>
		[XmlAttribute("hostHeader")]
		public string HostHeader
		{
			get { return _hostHeader; }
		}
		#endregion
	}
}