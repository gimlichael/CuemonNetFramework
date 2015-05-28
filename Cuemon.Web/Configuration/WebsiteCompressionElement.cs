using System;
using System.Configuration;
namespace Cuemon.Web.Configuration
{
    /// <summary>
    /// Represents a Cuemon/Web/%ApplicationName%/WebSites/Website/RequestPipelineProcessing configuration element within a configuration file.
    /// </summary>
    public sealed class WebsiteCompressionElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets a comma delimited string of file extensions to include when determining what to dynamically compress.
        /// </summary>
        /// <value>A comma delimited string of file extensions to include when determining what to dynamically compress.</value>
        [ConfigurationProperty("fileExtensions", IsRequired = false, DefaultValue = "aspx,ashx,asmx,svc")]
        public string FileExtensions
        {
            get { return (string)base["fileExtensions"]; }
            set { base["fileExtensions"] = value; }
        }
    }
}