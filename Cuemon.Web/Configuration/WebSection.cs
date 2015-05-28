using System;
using System.Configuration;
namespace Cuemon.Web.Configuration
{
    /// <summary>
    /// Represents a Web section within a configuration file.
    /// </summary>
    public class WebSection : ConfigurationSection
    {
        /// <summary>
        /// Gets the base application element from your configuration file.
        /// </summary>
        /// <param name="name">The name of the application you wish to retreive.</param>
        /// <returns></returns>
        public virtual ApplicationSection GetApplication(string name)
        {
            return (ApplicationSection)base[name];
        }

        /// <summary>
        /// Gets the available UI from your configuration file.
        /// </summary>
        /// <value>The UI meta data as entered in your configuration file.</value>
        [ConfigurationProperty("UI")]
        public UISection UI
        {
            get { return (UISection)base["UI"]; }
        }
    }
}
