using System.Configuration;

namespace Cuemon.Web.Configuration
{
    /// <summary>
    /// Represents a configuration element containing a collection of child elements (<see cref="T:Cuemon.Web.Configuration.WebsiteThemeElement"></see>).
    /// </summary>
    public sealed class WebsiteThemeElementCollection : ConfigurationElementCollection
    {
        /// <summary>
        /// Creates a new <see cref="T:Cuemon.Web.Configuration.WebsiteThemeElement"></see>.
        /// </summary>
        /// <returns>
        /// A new <see cref="T:Cuemon.Web.Configuration.WebsiteThemeElement"></see>.
        /// </returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new WebsiteThemeElement();
        }

        /// <summary>
        /// Gets the element key for a specified configuration element when overridden in a derived class.
        /// </summary>
        /// <param name="element">The <see cref="T:Cuemon.Web.Configuration.WebsiteThemeElement"></see> to return the key for.</param>
        /// <returns>
        /// An <see cref="T:System.Object"></see> that acts as the key for the specified <see cref="T:Cuemon.Web.Configuration.WebsiteThemeElement"></see>.
        /// </returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((WebsiteThemeElement)(element)).Name;
        }

		  /// <summary>
		  /// Gets or sets the name of the default theme to use.
		  /// </summary>
		  /// <value>The name of the default theme to use.</value>
		  [ConfigurationProperty("defaultTheme", IsRequired = true)]
		  public string DefaultTheme
		  {
			  get { return (string)base["defaultTheme"]; }
			  set { base["defaultTheme"] = value; }
		  }

		  /// <summary>
		  /// Gets the default theme for this <see cref="Cuemon.Web.Website"/>.
		  /// </summary>
		  /// <value>The default theme for this <see cref="Cuemon.Web.Website"/>.</value>
		  public WebsiteThemeElement GetDefaultThemeFromCollection()
		  {
			  foreach (WebsiteThemeElement theme in this)
			  {
				  if (theme.Name == this.DefaultTheme)
				  {
					  return theme;
				  }
			  }
			  return null;
		  }
    }
}