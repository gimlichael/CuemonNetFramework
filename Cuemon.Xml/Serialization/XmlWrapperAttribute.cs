using System;
using System.Collections.Generic;
using System.Text;

namespace Cuemon.Xml.Serialization
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    internal sealed class XmlWrapperAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlWrapperAttribute"/> class.
        /// </summary>
        public XmlWrapperAttribute()
        {
        }
    }
}