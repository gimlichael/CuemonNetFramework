using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Cuemon.Web
{
    /// <summary>
    /// Provides properties and methods for defining a response for a HTTP operation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class HttpResponseAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResponseAttribute"/> class.
        /// </summary>
        public HttpResponseAttribute()
        {
            this.SuccessStatusCode = HttpStatusCode.OK;
        }

        /// <summary>
        /// Gets or sets the status code of a successful HTTP response. Default is <see cref="HttpStatusCode.OK"/>.
        /// </summary>
        /// <value>The status code of a successful HTTP response.</value>
        public HttpStatusCode SuccessStatusCode { get; set; }

        /// <summary>
        /// Gets or sets the status description of a successful HTTP response.
        /// </summary>
        /// <value>The status description of a successful HTTP response.</value>
        public string SuccessStatusDescription { get; set; }
    }
}
