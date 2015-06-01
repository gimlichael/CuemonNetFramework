using System.Configuration;

namespace Cuemon.Web.Configuration
{
    /// <summary>
    /// Represents a Cuemon/Web/%ApplicationName%/WebSites/Website/RequestPipelineProcessing configuration element within a configuration file.
    /// </summary>
    public sealed class WebsiteRequestPipelineProcessingElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets a comma delimited string of file extensions to include when determining requests for pipeline processing.
        /// </summary>
        /// <value>A comma delimited string of file extensions to include when determining requests for pipeline processing.</value>
        [ConfigurationProperty("fileExtensions", IsRequired = false, DefaultValue = "aspx,ashx,asmx,svc")]
        public string FileExtensions
        {
            get { return (string)base["fileExtensions"]; }
            set { base["fileExtensions"] = value; }
        }
    }
}