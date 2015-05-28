using System;
using System.Configuration;
using System.Xml;
using Cuemon.Configuration;
namespace Cuemon.Web.Configuration
{
	/// <summary>
	/// Represents a Cuemon/Web/%ApplicationName%/Websites/add configuration element within a configuration file.
	/// </summary>
	public class WebsiteElement : ConfigurationElement
	{
		/// <summary>
		/// Gets or sets the id of the <see cref="Cuemon.Web.Website"/> object.
		/// </summary>
		/// <value>The id of the <see cref="Cuemon.Web.Website"/> object.</value>
		[ConfigurationProperty("id", IsKey = true, IsRequired = true)]
		public Guid Id
		{
			get { return (Guid)base["id"]; }
			set { base["id"] = value; }
		}

		/// <summary>
		/// Gets or sets the name of the <see cref="Cuemon.Web.Website"/>.
		/// </summary>
		/// <value>The name of the <see cref="Cuemon.Web.Website"/>.</value>
		[ConfigurationProperty("name", IsRequired = true)]
		public string Name
		{
			get { return (string)base["name"]; }
			set { base["name"] = value; }
		}

		/// <summary>
		/// Gets or sets the physical content application path of the <see cref="Cuemon.Web.Website"/>.
		/// </summary>
		/// <value>The physical content application path of the <see cref="Cuemon.Web.Website"/>.</value>
		[ConfigurationProperty("contentApplicationPath", IsRequired = true)]
		public string ContentApplicationPath
		{
			get { return (string)base["contentApplicationPath"]; }
			set { base["contentApplicationPath"] = value; }
		}

		/// <summary>
		/// Gets or sets the version of the <see cref="Cuemon.Web.Website"/>.
		/// </summary>
		/// <value>The version of the <see cref="Cuemon.Web.Website"/>.</value>
		[ConfigurationProperty("version", IsRequired = true)]
		public string Version
		{
			get { return (string)base["version"]; }
			set { base["version"] = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether to enable compression of delivered output content.
		/// </summary>
		/// <value><c>true</c> to enable compression of delivered output content; otherwise, <c>false</c>.</value>
		[ConfigurationProperty("enableCompression", IsRequired = true)]
		public bool EnableCompression
		{
			get { return (bool)base["enableCompression"]; }
			set { base["enableCompression"] = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether to enable clientside caching by the resolvement from when a ressource was last modified.
		/// </summary>
		/// <value><c>true</c> to enable clientside caching by the resolvement from when a ressource was last modified; otherwise, <c>false</c>.</value>
		[ConfigurationProperty("enableClientSideCaching", IsRequired = true)]
		public bool EnableClientSideCaching
		{
			get { return (bool)base["enableClientSideCaching"]; }
			set { base["enableClientSideCaching"] = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the content application path will be set dynamically by the executing application.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the content application path will be set dynamically by the executing application; otherwise, <c>false</c>.
		/// </value>
		[ConfigurationProperty("enableDynamicContentApplicationPath", IsRequired = true)]
		public bool EnableDynamicContentApplicationPath
		{
			get { return (bool)base["enableDynamicContentApplicationPath"]; }
			set { base["enableDynamicContentApplicationPath"] = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether maintenance is enabled.
		/// </summary>
		/// <value><c>true</c> if maintenance is enabled; otherwise, <c>false</c>.</value>
		[ConfigurationProperty("enableMaintenance", IsRequired = true)]
		public bool EnableMaintenance
		{
			get { return (bool)base["enableMaintenance"]; }
			set { base["enableMaintenance"] = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether security is enabled.
		/// </summary>
		/// <value><c>true</c> if security is enabled; otherwise, <c>false</c>.</value>
		[ConfigurationProperty("enableSecurity", IsRequired = true)]
		public bool EnableSecurity
		{
			get { return (bool)base["enableSecurity"]; }
			set { base["enableSecurity"] = value; }
		}

		/// <summary>
		/// Gets the available website bindings from your configuration file.
		/// </summary>
		/// <value>The website bindings as entered in your configuration file.</value>
		[ConfigurationProperty("Bindings", IsRequired = true)]
		public WebsiteBindingElementCollection Bindings
		{
			get { return (WebsiteBindingElementCollection)base["Bindings"]; }
		}

		/// <summary>
		/// Gets the caching element of the %ApplicationName% in the Website.
		/// </summary>
		/// <value>The caching element of the %ApplicationName% in the Website.</value>
		[ConfigurationProperty("Caching", IsRequired = true)]
		public WebsiteCachingElement Caching
		{
			get { return (WebsiteCachingElement)base["Caching"]; }
		}

		/// <summary>
		/// Gets the compression element.
		/// </summary>
		/// <value>The compression element.</value>
		[ConfigurationProperty("Compression", IsRequired = false)]
		public WebsiteCompressionElement Compression
		{
			get { return (WebsiteCompressionElement)base["Compression"]; }
		}

		/// <summary>
		/// Gets the maintenance element of the %ApplicationName% in the Website.
		/// </summary>
		/// <value>The maintenance element of the %ApplicationName% in the Website.</value>
		[ConfigurationProperty("Maintenance", IsRequired = true)]
		public WebsiteMaintenanceElement Maintenance
		{
			get { return (WebsiteMaintenanceElement)base["Maintenance"]; }
		}

		/// <summary>
		/// Gets the globalization element of the %ApplicationName% in the Website.
		/// </summary>
		/// <value>The globalization element of the community.</value>
		[ConfigurationProperty("Globalization", IsRequired = true)]
		public WebsiteGlobalizationElement Globalization
		{
			get { return (WebsiteGlobalizationElement)base["Globalization"]; }
		}

		/// <summary>
		/// Gets the request pipeline processing element.
		/// </summary>
		/// <value>The request pipeline processing element.</value>
		[ConfigurationProperty("RequestPipelineProcessing", IsRequired = false)]
		public WebsiteRequestPipelineProcessingElement RequestPipelineProcessing
		{
			get { return (WebsiteRequestPipelineProcessingElement) base["RequestPipelineProcessing"]; }
		}

		/// <summary>
		/// Gets the robots element of the <see cref="Cuemon.Web.Website"/>.
		/// </summary>
		/// <value>The robots element of the <see cref="Cuemon.Web.Website"/>.</value>
		[ConfigurationProperty("Robots", IsRequired = true)]
		public WebsiteRobotsElement Robots
		{
			get { return (WebsiteRobotsElement)base["Robots"]; }
		}

		/// <summary>
		/// Gets the security element of the %ApplicationName% in the Website.
		/// </summary>
		/// <value>The security element of the community.</value>
		[ConfigurationProperty("Security", IsRequired = true)]
		public WebsiteSecurityElement Security
		{
			get { return (WebsiteSecurityElement)base["Security"]; }
		}

		/// <summary>
		/// Gets the available Themes from your configuration file.
		/// </summary>
		/// <value>The available Themes as entered in your configuration file.</value>
		[ConfigurationProperty("Themes", IsRequired = true)]
		public WebsiteThemeElementCollection Themes
		{
			get
			{
				return (WebsiteThemeElementCollection)base["Themes"];
			}
		}

		/// <summary>
		/// Gets the data element of the %ApplicationName% in the Website.
		/// </summary>
		/// <value>The data element of the %ApplicationName% in the Website.</value>
		[ConfigurationProperty("Data")]
		public virtual WebsiteDataElement Data
		{
			get { return (WebsiteDataElement)base["Data"]; }
		}
	}
}