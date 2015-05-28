using System;
using System.Configuration;
using Cuemon.Diagnostics;
namespace Cuemon.Configuration
{
    /// <summary>
    /// Represents a group of related sections within a configuration file (Data).
    /// </summary>
    public class SectionGroup : ConfigurationSectionGroup
    {
        /// <summary>
        /// Gets the data section group element.
        /// </summary>
        /// <value>The data section group element.</value>
        public DataSection Data
        {
            get
            {
                return (DataSection)this.Sections["Data"];
            }
        }
    }
}
