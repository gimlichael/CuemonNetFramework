using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;
namespace Cuemon.Web
{
    /// <summary>
    /// Provides a common base class for an implementation of the Platform for Privacy Preferences Project (P3P), which enables Websites to express their 
    /// privacy practices in a standard format that can be retrieved automatically and interpreted easily by user agents.
    /// </summary>
    public abstract class WebsitePrivacyProvider
    {
        public virtual string GetCompactPolicy()
        {
            return null;
        }

        public virtual IXPathNavigable GetPolicyReference()
        {
            return null;
        }
    }
}