using System.Web;
using Cuemon.Caching;

namespace Cuemon.Web
{
    /// <summary>
    /// Extends the <see cref="IHttpHandler"/> with client-side caching.
    /// </summary>
    public interface ICacheableHttpHandler : IHttpHandler
    {
        /// <summary>
        /// Gets a <see cref="CacheValidator"/> object that represents the content of the resource.
        /// </summary>
        /// <returns>A <see cref="CacheValidator" /> object that represents the content of the resource.</returns>
        CacheValidator GetCacheValidator();

        /// <summary>
        /// Determines whether a cached version of the content of the resource is found client-side given the specified <paramref name="validator"/>.
        /// </summary>
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        /// <returns>
        /// 	<c>true</c> if a cached version of the content of the resource is found client-side given the specified <paramref name="validator"/>; otherwise, <c>false</c>.
        /// </returns>
        bool HasClientSideResource(CacheValidator validator);
    }
}