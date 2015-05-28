using System;
using System.Web;

namespace Cuemon.Web
{
    /// <summary>
    /// Indicates ways in which a operation can control cache-specific HTTP headers for the Web programming model.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public sealed class HttpCachingAttribute : Attribute
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpCachingAttribute"/> class. <see cref="Cacheability"/> is set to <see cref="HttpCacheability.NoCache"/>.
        /// </summary>
        public HttpCachingAttribute()
        {
            this.Cacheability = HttpCacheability.NoCache;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpCachingAttribute" /> class.
        /// </summary>
        /// <param name="duration">The cache duration value paired with <paramref name="durationUnit"/>.</param>
        /// <param name="durationUnit">The unit of time paired with <paramref name="duration"/>.</param>
        public HttpCachingAttribute(double duration, TimeUnit durationUnit)
        {
            this.Duration = duration;
            this.DurationUnit = durationUnit;
            this.Cacheability = HttpCacheability.Private;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether HTTP caching is enabled.
        /// </summary>
        /// <value><c>true</c> if HTTP caching is enabled; otherwise, <c>false</c>.</value>
        public bool EnableCaching
        {
            get { return this.Cacheability != HttpCacheability.NoCache; }
        }

        /// <summary>
        /// Gets or sets the cache duration value for <see cref="DurationUnit"/>.
        /// </summary>
        /// <value>The cache duration value for <see cref="DurationUnit"/>.</value>
        public double Duration { get; private set; }

        /// <summary>
        /// Gets the unit of time for <see cref="Duration"/>. Default is <see cref="TimeUnit.Seconds"/>.
        /// </summary>
        /// <value>The unit of time for <see cref="Duration"/>.</value>
        public TimeUnit DurationUnit { get; private set; }

        /// <summary>
        /// Gets the HTTP <b>Cache-Control</b> header to one of the values of <see cref="HttpCacheability"/>.
        /// </summary>
        /// <value>The HTTP <b>Cache-Control</b> header to one of the values of <see cref="HttpCacheability"/>.</value>
        public HttpCacheability Cacheability { get; set; }
        #endregion

        #region Methods
        #endregion
    }
}