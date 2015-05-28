using System;
using System.Net.Mime;

namespace Cuemon.ServiceModel
{
    /// <summary>
    /// Specifies that the type defines or implements a HTTP entity body contract and can be serialized.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public class HttpEntityBodyAttribute : Attribute
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpEntityBodyAttribute"/> class.
        /// </summary>
        public HttpEntityBodyAttribute()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the name of the HTTP entity body contract for the type.
        /// </summary>
        /// <value>The local name of a HTTP entity body contract. The default is the name of the class that the attribute is applied to.</value>
        public string Name { get; set; }
        #endregion

        #region Methods
        #endregion
    }
}