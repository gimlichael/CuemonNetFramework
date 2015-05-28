using System;
using System.Configuration;
using Cuemon.Globalization;
namespace Cuemon.Web.Configuration
{
    /// <summary>
    /// Represents a Cuemon/Web/%ApplicationName%/WebSites/Website/Caching/ExpiresHeaders/add configuration element within a configuration file.
    /// </summary>
    public sealed class WebsiteCachingExpiresHeaderElement : ConfigurationElement
    {
        private static readonly string[] units = new string[] { "Seconds", "Minutes", "Hours", "Days", "Months", "Years" };

        /// <summary>
        /// Gets or sets the expires value to be used in conjuction with the <see cref="Unit"/> attribute.
        /// </summary>
        /// <value>The expires value to be used in conjuction with the <see cref="Unit"/> attribute.</value>
        [ConfigurationProperty("expires", IsRequired = true)]
        public ushort Expires
        {
            get { return (ushort)base["expires"]; }
            set { base["expires"] = value; }
        }

        /// <summary>
        /// Gets or sets the unit (Seconds, Minutes, Hours, Days, Months, Years) to use in conjuction with the <see cref="Expires"/> attribute.
        /// </summary>
        /// <value>The unit (Seconds, Minutes, Hours, Days, Months, Years) to use in conjuction with the <see cref="Expires"/> attribute.</value>
        [ConfigurationProperty("unit", IsRequired = true)]
        public string Unit
        {
            get
            {
                string unit = (string)base["unit"];
                this.ValidateUnit(unit);
                return unit;
            }
            set
            {
                this.ValidateUnit(value);
                base["unit"] = value;
            }
        }

        private void ValidateUnit(string value)
        {
            foreach (string unit in units)
            {
                if (unit.Equals(value)) { return; }
            }
            throw new ArgumentException("The value does not conform to the allowed unit values: Seconds, Minutes, Hours, Days, Months, Years.", "value");
        }

        /// <summary>
        /// Gets or sets a comma delimited string of file extensions to match when applying the expires header.
        /// </summary>
        /// <value>A comma delimited string of file extensions to match when applying the expires header.</value>
        [ConfigurationProperty("fileExtensions", IsRequired = true, IsKey = true)]
        public string FileExtensions
        {
            get { return (string)base["fileExtensions"]; }
            set { base["fileExtensions"] = value; }
        }
    }
}