using System;
using Cuemon.Diagnostics;
namespace Cuemon.Caching
{
    /// <summary>
    /// Provides data for cache related operations. This class cannot be inherited.
    /// </summary>
    public sealed class CacheEventArgs : EventArgs
    {
        private CacheEventArgs()
        {
        }

        internal CacheEventArgs(Cache cache)
        {
            this.Cache = cache;
        }

        internal Cache Cache { get; private set; }
    }
}