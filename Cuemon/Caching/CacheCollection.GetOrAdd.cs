using System;
using System.Collections.Generic;
using System.Text;

namespace Cuemon.Caching
{
    public sealed partial class CacheCollection
    {
        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<TResult>(string key, Doer<TResult> resolver)
        {
            return this.GetOrAdd(key, NoGroup, resolver);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<TResult>(string key, string group, Doer<TResult> resolver)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver();
                    this.Add(key, result, group);
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="absoluteExpiration">The time at which the return value of <paramref name="resolver"/> expires and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<TResult>(string key, Doer<TResult> resolver, DateTime absoluteExpiration)
        {
            return this.GetOrAdd(key, NoGroup, resolver, absoluteExpiration);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="absoluteExpiration">The time at which the return value of <paramref name="resolver"/> expires and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<TResult>(string key, string group, Doer<TResult> resolver, DateTime absoluteExpiration)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver();
                    this.Add(key, result, group, absoluteExpiration);
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="slidingExpiration">The interval between the time the return value of <paramref name="resolver"/> was last accessed and the time at which that object expires. If this value is the equivalent of 20 minutes, the object expires and is removed from the cache 20 minutes after it was last accessed.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<TResult>(string key, Doer<TResult> resolver, TimeSpan slidingExpiration)
        {
            return this.GetOrAdd(key, NoGroup, resolver, slidingExpiration);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="slidingExpiration">The interval between the time the return value of <paramref name="resolver"/> was last accessed and the time at which that object expires. If this value is the equivalent of 20 minutes, the object expires and is removed from the cache 20 minutes after it was last accessed.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<TResult>(string key, string group, Doer<TResult> resolver, TimeSpan slidingExpiration)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver();
                    this.Add(key, result, group, slidingExpiration);
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="dependencyResolver">The function delegate that is used to assign dependencies to the result of <paramref name="resolver"/> to the cache. When any dependency changes, the object becomes invalid and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<TResult>(string key, Doer<TResult> resolver, Doer<IEnumerable<IDependency>> dependencyResolver)
        {
            return this.GetOrAdd(key, NoGroup, resolver, dependencyResolver);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="dependencyResolver">The function delegate that is used to assign dependencies to the result of <paramref name="resolver"/> to the cache. When any dependency changes, the object becomes invalid and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<TResult>(string key, string group, Doer<TResult> resolver, Doer<IEnumerable<IDependency>> dependencyResolver)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            Validator.ThrowIfNull(dependencyResolver, "dependencyResolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver();
                    this.Add(key, result, group, dependencyResolver());
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="T">The type of the parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg">The parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<T, TResult>(string key, Doer<T, TResult> resolver, T arg)
        {
            return this.GetOrAdd(key, NoGroup, resolver, arg);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="T">The type of the parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg">The parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<T, TResult>(string key, string group, Doer<T, TResult> resolver, T arg)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver(arg);
                    this.Add(key, result, group);
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="T">The type of the parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg">The parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="absoluteExpiration">The time at which the return value of <paramref name="resolver"/> expires and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<T, TResult>(string key, Doer<T, TResult> resolver, T arg, DateTime absoluteExpiration)
        {
            return this.GetOrAdd(key, NoGroup, resolver, arg, absoluteExpiration);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="T">The type of the parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg">The parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="absoluteExpiration">The time at which the return value of <paramref name="resolver"/> expires and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<T, TResult>(string key, string group, Doer<T, TResult> resolver, T arg, DateTime absoluteExpiration)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver(arg);
                    this.Add(key, result, group, absoluteExpiration);
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="T">The type of the parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg">The parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="slidingExpiration">The interval between the time the return value of <paramref name="resolver"/> was last accessed and the time at which that object expires. If this value is the equivalent of 20 minutes, the object expires and is removed from the cache 20 minutes after it was last accessed.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<T, TResult>(string key, Doer<T, TResult> resolver, T arg, TimeSpan slidingExpiration)
        {
            return this.GetOrAdd(key, NoGroup, resolver, arg, slidingExpiration);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="T">The type of the parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg">The parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="slidingExpiration">The interval between the time the return value of <paramref name="resolver"/> was last accessed and the time at which that object expires. If this value is the equivalent of 20 minutes, the object expires and is removed from the cache 20 minutes after it was last accessed.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<T, TResult>(string key, string group, Doer<T, TResult> resolver, T arg, TimeSpan slidingExpiration)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver(arg);
                    this.Add(key, result, group, slidingExpiration);
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="T">The type of the parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg">The parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="dependencyResolver">The function delegate that is used to assign dependencies to the result of <paramref name="resolver"/> to the cache. When any dependency changes, the object becomes invalid and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<T, TResult>(string key, Doer<T, TResult> resolver, T arg, Doer<T, IEnumerable<IDependency>> dependencyResolver)
        {
            return this.GetOrAdd(key, NoGroup, resolver, arg, dependencyResolver);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="T">The type of the parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg">The parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="dependencyResolver">The function delegate that is used to assign dependencies to the result of <paramref name="resolver"/> to the cache. When any dependency changes, the object becomes invalid and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<T, TResult>(string key, string group, Doer<T, TResult> resolver, T arg, Doer<T, IEnumerable<IDependency>> dependencyResolver)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            Validator.ThrowIfNull(dependencyResolver, "dependencyResolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver(arg);
                    this.Add(key, result, group, dependencyResolver(arg));
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<T1, T2, TResult>(string key, Doer<T1, T2, TResult> resolver, T1 arg1, T2 arg2)
        {
            return this.GetOrAdd(key, NoGroup, resolver, arg1, arg2);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<T1, T2, TResult>(string key, string group, Doer<T1, T2, TResult> resolver, T1 arg1, T2 arg2)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver(arg1, arg2);
                    this.Add(key, result, group);
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="absoluteExpiration">The time at which the return value of <paramref name="resolver"/> expires and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<T1, T2, TResult>(string key, Doer<T1, T2, TResult> resolver, T1 arg1, T2 arg2, DateTime absoluteExpiration)
        {
            return this.GetOrAdd(key, NoGroup, resolver, arg1, arg2, absoluteExpiration);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="absoluteExpiration">The time at which the return value of <paramref name="resolver"/> expires and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<T1, T2, TResult>(string key, string group, Doer<T1, T2, TResult> resolver, T1 arg1, T2 arg2, DateTime absoluteExpiration)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver(arg1, arg2);
                    this.Add(key, result, group, absoluteExpiration);
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="slidingExpiration">The interval between the time the return value of <paramref name="resolver"/> was last accessed and the time at which that object expires. If this value is the equivalent of 20 minutes, the object expires and is removed from the cache 20 minutes after it was last accessed.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<T1, T2, TResult>(string key, Doer<T1, T2, TResult> resolver, T1 arg1, T2 arg2, TimeSpan slidingExpiration)
        {
            return this.GetOrAdd(key, NoGroup, resolver, arg1, arg2, slidingExpiration);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="slidingExpiration">The interval between the time the return value of <paramref name="resolver"/> was last accessed and the time at which that object expires. If this value is the equivalent of 20 minutes, the object expires and is removed from the cache 20 minutes after it was last accessed.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<T1, T2, TResult>(string key, string group, Doer<T1, T2, TResult> resolver, T1 arg1, T2 arg2, TimeSpan slidingExpiration)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver(arg1, arg2);
                    this.Add(key, result, group, slidingExpiration);
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="dependencyResolver">The function delegate that is used to assign dependencies to the result of <paramref name="resolver"/> to the cache. When any dependency changes, the object becomes invalid and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<T1, T2, TResult>(string key, Doer<T1, T2, TResult> resolver, T1 arg1, T2 arg2, Doer<T1, T2, IEnumerable<IDependency>> dependencyResolver)
        {
            return this.GetOrAdd(key, NoGroup, resolver, arg1, arg2, dependencyResolver);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="dependencyResolver">The function delegate that is used to assign dependencies to the result of <paramref name="resolver"/> to the cache. When any dependency changes, the object becomes invalid and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<T1, T2, TResult>(string key, string group, Doer<T1, T2, TResult> resolver, T1 arg1, T2 arg2, Doer<T1, T2, IEnumerable<IDependency>> dependencyResolver)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            Validator.ThrowIfNull(dependencyResolver, "dependencyResolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver(arg1, arg2);
                    this.Add(key, result, group, dependencyResolver(arg1, arg2));
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, TResult>(string key, Doer<T1, T2, T3, TResult> resolver, T1 arg1, T2 arg2, T3 arg3)
        {
            return this.GetOrAdd(key, NoGroup, resolver, arg1, arg2, arg3);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, TResult>(string key, string group, Doer<T1, T2, T3, TResult> resolver, T1 arg1, T2 arg2, T3 arg3)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver(arg1, arg2, arg3);
                    this.Add(key, result, group);
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="absoluteExpiration">The time at which the return value of <paramref name="resolver"/> expires and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, TResult>(string key, Doer<T1, T2, T3, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, DateTime absoluteExpiration)
        {
            return this.GetOrAdd(key, NoGroup, resolver, arg1, arg2, arg3, absoluteExpiration);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="absoluteExpiration">The time at which the return value of <paramref name="resolver"/> expires and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, TResult>(string key, string group, Doer<T1, T2, T3, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, DateTime absoluteExpiration)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver(arg1, arg2, arg3);
                    this.Add(key, result, group, absoluteExpiration);
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="slidingExpiration">The interval between the time the return value of <paramref name="resolver"/> was last accessed and the time at which that object expires. If this value is the equivalent of 20 minutes, the object expires and is removed from the cache 20 minutes after it was last accessed.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, TResult>(string key, Doer<T1, T2, T3, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, TimeSpan slidingExpiration)
        {
            return this.GetOrAdd(key, NoGroup, resolver, arg1, arg2, arg3, slidingExpiration);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="slidingExpiration">The interval between the time the return value of <paramref name="resolver"/> was last accessed and the time at which that object expires. If this value is the equivalent of 20 minutes, the object expires and is removed from the cache 20 minutes after it was last accessed.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, TResult>(string key, string group, Doer<T1, T2, T3, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, TimeSpan slidingExpiration)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver(arg1, arg2, arg3);
                    this.Add(key, result, group, slidingExpiration);
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="dependencyResolver">The function delegate that is used to assign dependencies to the result of <paramref name="resolver"/> to the cache. When any dependency changes, the object becomes invalid and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, TResult>(string key, Doer<T1, T2, T3, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, Doer<T1, T2, T3, IEnumerable<IDependency>> dependencyResolver)
        {
            return this.GetOrAdd(key, NoGroup, resolver, arg1, arg2, arg3, dependencyResolver);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="dependencyResolver">The function delegate that is used to assign dependencies to the result of <paramref name="resolver"/> to the cache. When any dependency changes, the object becomes invalid and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, TResult>(string key, string group, Doer<T1, T2, T3, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, Doer<T1, T2, T3, IEnumerable<IDependency>> dependencyResolver)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            Validator.ThrowIfNull(dependencyResolver, "dependencyResolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver(arg1, arg2, arg3);
                    this.Add(key, result, group, dependencyResolver(arg1, arg2, arg3));
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, TResult>(string key, Doer<T1, T2, T3, T4, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return this.GetOrAdd(key, NoGroup, resolver, arg1, arg2, arg3, arg4);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, TResult>(string key, string group, Doer<T1, T2, T3, T4, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver(arg1, arg2, arg3, arg4);
                    this.Add(key, result, group);
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="absoluteExpiration">The time at which the return value of <paramref name="resolver"/> expires and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, TResult>(string key, Doer<T1, T2, T3, T4, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, DateTime absoluteExpiration)
        {
            return this.GetOrAdd(key, NoGroup, resolver, arg1, arg2, arg3, arg4, absoluteExpiration);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="absoluteExpiration">The time at which the return value of <paramref name="resolver"/> expires and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, TResult>(string key, string group, Doer<T1, T2, T3, T4, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, DateTime absoluteExpiration)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver(arg1, arg2, arg3, arg4);
                    this.Add(key, result, group, absoluteExpiration);
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="slidingExpiration">The interval between the time the return value of <paramref name="resolver"/> was last accessed and the time at which that object expires. If this value is the equivalent of 20 minutes, the object expires and is removed from the cache 20 minutes after it was last accessed.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, TResult>(string key, Doer<T1, T2, T3, T4, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, TimeSpan slidingExpiration)
        {
            return this.GetOrAdd(key, NoGroup, resolver, arg1, arg2, arg3, arg4, slidingExpiration);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="slidingExpiration">The interval between the time the return value of <paramref name="resolver"/> was last accessed and the time at which that object expires. If this value is the equivalent of 20 minutes, the object expires and is removed from the cache 20 minutes after it was last accessed.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, TResult>(string key, string group, Doer<T1, T2, T3, T4, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, TimeSpan slidingExpiration)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver(arg1, arg2, arg3, arg4);
                    this.Add(key, result, group, slidingExpiration);
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="dependencyResolver">The function delegate that is used to assign dependencies to the result of <paramref name="resolver"/> to the cache. When any dependency changes, the object becomes invalid and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, TResult>(string key, Doer<T1, T2, T3, T4, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Doer<T1, T2, T3, T4, IEnumerable<IDependency>> dependencyResolver)
        {
            return this.GetOrAdd(key, NoGroup, resolver, arg1, arg2, arg3, arg4, dependencyResolver);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="dependencyResolver">The function delegate that is used to assign dependencies to the result of <paramref name="resolver"/> to the cache. When any dependency changes, the object becomes invalid and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, TResult>(string key, string group, Doer<T1, T2, T3, T4, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Doer<T1, T2, T3, T4, IEnumerable<IDependency>> dependencyResolver)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            Validator.ThrowIfNull(dependencyResolver, "dependencyResolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver(arg1, arg2, arg3, arg4);
                    this.Add(key, result, group, dependencyResolver(arg1, arg2, arg3, arg4));
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, TResult>(string key, Doer<T1, T2, T3, T4, T5, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return this.GetOrAdd(key, NoGroup, resolver, arg1, arg2, arg3, arg4, arg5);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, TResult>(string key, string group, Doer<T1, T2, T3, T4, T5, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver(arg1, arg2, arg3, arg4, arg5);
                    this.Add(key, result, group);
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="absoluteExpiration">The time at which the return value of <paramref name="resolver"/> expires and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, TResult>(string key, Doer<T1, T2, T3, T4, T5, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, DateTime absoluteExpiration)
        {
            return this.GetOrAdd(key, NoGroup, resolver, arg1, arg2, arg3, arg4, arg5, absoluteExpiration);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="absoluteExpiration">The time at which the return value of <paramref name="resolver"/> expires and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, TResult>(string key, string group, Doer<T1, T2, T3, T4, T5, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, DateTime absoluteExpiration)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver(arg1, arg2, arg3, arg4, arg5);
                    this.Add(key, result, group, absoluteExpiration);
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="slidingExpiration">The interval between the time the return value of <paramref name="resolver"/> was last accessed and the time at which that object expires. If this value is the equivalent of 20 minutes, the object expires and is removed from the cache 20 minutes after it was last accessed.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, TResult>(string key, Doer<T1, T2, T3, T4, T5, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, TimeSpan slidingExpiration)
        {
            return this.GetOrAdd(key, NoGroup, resolver, arg1, arg2, arg3, arg4, arg5, slidingExpiration);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="slidingExpiration">The interval between the time the return value of <paramref name="resolver"/> was last accessed and the time at which that object expires. If this value is the equivalent of 20 minutes, the object expires and is removed from the cache 20 minutes after it was last accessed.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, TResult>(string key, string group, Doer<T1, T2, T3, T4, T5, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, TimeSpan slidingExpiration)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver(arg1, arg2, arg3, arg4, arg5);
                    this.Add(key, result, group, slidingExpiration);
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="dependencyResolver">The function delegate that is used to assign dependencies to the result of <paramref name="resolver"/> to the cache. When any dependency changes, the object becomes invalid and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, TResult>(string key, Doer<T1, T2, T3, T4, T5, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Doer<T1, T2, T3, T4, T5, IEnumerable<IDependency>> dependencyResolver)
        {
            return this.GetOrAdd(key, NoGroup, resolver, arg1, arg2, arg3, arg4, arg5, dependencyResolver);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="dependencyResolver">The function delegate that is used to assign dependencies to the result of <paramref name="resolver"/> to the cache. When any dependency changes, the object becomes invalid and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, TResult>(string key, string group, Doer<T1, T2, T3, T4, T5, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Doer<T1, T2, T3, T4, T5, IEnumerable<IDependency>> dependencyResolver)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            Validator.ThrowIfNull(dependencyResolver, "dependencyResolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver(arg1, arg2, arg3, arg4, arg5);
                    this.Add(key, result, group, dependencyResolver(arg1, arg2, arg3, arg4, arg5));
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, T6, TResult>(string key, Doer<T1, T2, T3, T4, T5, T6, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            return this.GetOrAdd(key, NoGroup, resolver, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, T6, TResult>(string key, string group, Doer<T1, T2, T3, T4, T5, T6, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver(arg1, arg2, arg3, arg4, arg5, arg6);
                    this.Add(key, result, group);
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="absoluteExpiration">The time at which the return value of <paramref name="resolver"/> expires and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, T6, TResult>(string key, Doer<T1, T2, T3, T4, T5, T6, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, DateTime absoluteExpiration)
        {
            return this.GetOrAdd(key, NoGroup, resolver, arg1, arg2, arg3, arg4, arg5, arg6, absoluteExpiration);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="absoluteExpiration">The time at which the return value of <paramref name="resolver"/> expires and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, T6, TResult>(string key, string group, Doer<T1, T2, T3, T4, T5, T6, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, DateTime absoluteExpiration)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver(arg1, arg2, arg3, arg4, arg5, arg6);
                    this.Add(key, result, group, absoluteExpiration);
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="slidingExpiration">The interval between the time the return value of <paramref name="resolver"/> was last accessed and the time at which that object expires. If this value is the equivalent of 20 minutes, the object expires and is removed from the cache 20 minutes after it was last accessed.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, T6, TResult>(string key, Doer<T1, T2, T3, T4, T5, T6, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, TimeSpan slidingExpiration)
        {
            return this.GetOrAdd(key, NoGroup, resolver, arg1, arg2, arg3, arg4, arg5, arg6, slidingExpiration);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="slidingExpiration">The interval between the time the return value of <paramref name="resolver"/> was last accessed and the time at which that object expires. If this value is the equivalent of 20 minutes, the object expires and is removed from the cache 20 minutes after it was last accessed.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, T6, TResult>(string key, string group, Doer<T1, T2, T3, T4, T5, T6, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, TimeSpan slidingExpiration)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver(arg1, arg2, arg3, arg4, arg5, arg6);
                    this.Add(key, result, group, slidingExpiration);
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="dependencyResolver">The function delegate that is used to assign dependencies to the result of <paramref name="resolver"/> to the cache. When any dependency changes, the object becomes invalid and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, T6, TResult>(string key, Doer<T1, T2, T3, T4, T5, T6, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, Doer<T1, T2, T3, T4, T5, T6, IEnumerable<IDependency>> dependencyResolver)
        {
            return this.GetOrAdd(key, NoGroup, resolver, arg1, arg2, arg3, arg4, arg5, arg6, dependencyResolver);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="dependencyResolver">The function delegate that is used to assign dependencies to the result of <paramref name="resolver"/> to the cache. When any dependency changes, the object becomes invalid and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, T6, TResult>(string key, string group, Doer<T1, T2, T3, T4, T5, T6, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, Doer<T1, T2, T3, T4, T5, T6, IEnumerable<IDependency>> dependencyResolver)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            Validator.ThrowIfNull(dependencyResolver, "dependencyResolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver(arg1, arg2, arg3, arg4, arg5, arg6);
                    this.Add(key, result, group, dependencyResolver(arg1, arg2, arg3, arg4, arg5, arg6));
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, T6, T7, TResult>(string key, Doer<T1, T2, T3, T4, T5, T6, T7, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            return this.GetOrAdd(key, NoGroup, resolver, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, T6, T7, TResult>(string key, string group, Doer<T1, T2, T3, T4, T5, T6, T7, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
                    this.Add(key, result, group);
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="absoluteExpiration">The time at which the return value of <paramref name="resolver"/> expires and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, T6, T7, TResult>(string key, Doer<T1, T2, T3, T4, T5, T6, T7, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, DateTime absoluteExpiration)
        {
            return this.GetOrAdd(key, NoGroup, resolver, arg1, arg2, arg3, arg4, arg5, arg6, arg7, absoluteExpiration);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="absoluteExpiration">The time at which the return value of <paramref name="resolver"/> expires and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, T6, T7, TResult>(string key, string group, Doer<T1, T2, T3, T4, T5, T6, T7, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, DateTime absoluteExpiration)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
                    this.Add(key, result, group, absoluteExpiration);
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="slidingExpiration">The interval between the time the return value of <paramref name="resolver"/> was last accessed and the time at which that object expires. If this value is the equivalent of 20 minutes, the object expires and is removed from the cache 20 minutes after it was last accessed.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, T6, T7, TResult>(string key, Doer<T1, T2, T3, T4, T5, T6, T7, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, TimeSpan slidingExpiration)
        {
            return this.GetOrAdd(key, NoGroup, resolver, arg1, arg2, arg3, arg4, arg5, arg6, arg7, slidingExpiration);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="slidingExpiration">The interval between the time the return value of <paramref name="resolver"/> was last accessed and the time at which that object expires. If this value is the equivalent of 20 minutes, the object expires and is removed from the cache 20 minutes after it was last accessed.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, T6, T7, TResult>(string key, string group, Doer<T1, T2, T3, T4, T5, T6, T7, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, TimeSpan slidingExpiration)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
                    this.Add(key, result, group, slidingExpiration);
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="dependencyResolver">The function delegate that is used to assign dependencies to the result of <paramref name="resolver"/> to the cache. When any dependency changes, the object becomes invalid and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, T6, T7, TResult>(string key, Doer<T1, T2, T3, T4, T5, T6, T7, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, Doer<T1, T2, T3, T4, T5, T6, T7, IEnumerable<IDependency>> dependencyResolver)
        {
            return this.GetOrAdd(key, NoGroup, resolver, arg1, arg2, arg3, arg4, arg5, arg6, arg7, dependencyResolver);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="dependencyResolver">The function delegate that is used to assign dependencies to the result of <paramref name="resolver"/> to the cache. When any dependency changes, the object becomes invalid and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, T6, T7, TResult>(string key, string group, Doer<T1, T2, T3, T4, T5, T6, T7, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, Doer<T1, T2, T3, T4, T5, T6, T7, IEnumerable<IDependency>> dependencyResolver)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            Validator.ThrowIfNull(dependencyResolver, "dependencyResolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
                    this.Add(key, result, group, dependencyResolver(arg1, arg2, arg3, arg4, arg5, arg6, arg7));
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(string key, Doer<T1, T2, T3, T4, T5, T6, T7, T8, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            return this.GetOrAdd(key, NoGroup, resolver, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(string key, string group, Doer<T1, T2, T3, T4, T5, T6, T7, T8, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
                    this.Add(key, result, group);
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="absoluteExpiration">The time at which the return value of <paramref name="resolver"/> expires and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(string key, Doer<T1, T2, T3, T4, T5, T6, T7, T8, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, DateTime absoluteExpiration)
        {
            return this.GetOrAdd(key, NoGroup, resolver, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, absoluteExpiration);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="absoluteExpiration">The time at which the return value of <paramref name="resolver"/> expires and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(string key, string group, Doer<T1, T2, T3, T4, T5, T6, T7, T8, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, DateTime absoluteExpiration)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
                    this.Add(key, result, group, absoluteExpiration);
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="slidingExpiration">The interval between the time the return value of <paramref name="resolver"/> was last accessed and the time at which that object expires. If this value is the equivalent of 20 minutes, the object expires and is removed from the cache 20 minutes after it was last accessed.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(string key, Doer<T1, T2, T3, T4, T5, T6, T7, T8, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, TimeSpan slidingExpiration)
        {
            return this.GetOrAdd(key, NoGroup, resolver, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, slidingExpiration);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="slidingExpiration">The interval between the time the return value of <paramref name="resolver"/> was last accessed and the time at which that object expires. If this value is the equivalent of 20 minutes, the object expires and is removed from the cache 20 minutes after it was last accessed.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(string key, string group, Doer<T1, T2, T3, T4, T5, T6, T7, T8, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, TimeSpan slidingExpiration)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
                    this.Add(key, result, group, slidingExpiration);
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="dependencyResolver">The function delegate that is used to assign dependencies to the result of <paramref name="resolver"/> to the cache. When any dependency changes, the object becomes invalid and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(string key, Doer<T1, T2, T3, T4, T5, T6, T7, T8, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, Doer<T1, T2, T3, T4, T5, T6, T7, T8, IEnumerable<IDependency>> dependencyResolver)
        {
            return this.GetOrAdd(key, NoGroup, resolver, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, dependencyResolver);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="dependencyResolver">The function delegate that is used to assign dependencies to the result of <paramref name="resolver"/> to the cache. When any dependency changes, the object becomes invalid and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(string key, string group, Doer<T1, T2, T3, T4, T5, T6, T7, T8, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, Doer<T1, T2, T3, T4, T5, T6, T7, T8, IEnumerable<IDependency>> dependencyResolver)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            Validator.ThrowIfNull(dependencyResolver, "dependencyResolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
                    this.Add(key, result, group, dependencyResolver(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8));
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(string key, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            return this.GetOrAdd(key, NoGroup, resolver, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(string key, string group, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
                    this.Add(key, result, group);
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="absoluteExpiration">The time at which the return value of <paramref name="resolver"/> expires and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(string key, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, DateTime absoluteExpiration)
        {
            return this.GetOrAdd(key, NoGroup, resolver, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, absoluteExpiration);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="absoluteExpiration">The time at which the return value of <paramref name="resolver"/> expires and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(string key, string group, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, DateTime absoluteExpiration)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
                    this.Add(key, result, group, absoluteExpiration);
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="slidingExpiration">The interval between the time the return value of <paramref name="resolver"/> was last accessed and the time at which that object expires. If this value is the equivalent of 20 minutes, the object expires and is removed from the cache 20 minutes after it was last accessed.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(string key, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, TimeSpan slidingExpiration)
        {
            return this.GetOrAdd(key, NoGroup, resolver, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, slidingExpiration);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="slidingExpiration">The interval between the time the return value of <paramref name="resolver"/> was last accessed and the time at which that object expires. If this value is the equivalent of 20 minutes, the object expires and is removed from the cache 20 minutes after it was last accessed.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(string key, string group, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, TimeSpan slidingExpiration)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
                    this.Add(key, result, group, slidingExpiration);
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="dependencyResolver">The function delegate that is used to assign dependencies to the result of <paramref name="resolver"/> to the cache. When any dependency changes, the object becomes invalid and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(string key, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, IEnumerable<IDependency>> dependencyResolver)
        {
            return this.GetOrAdd(key, NoGroup, resolver, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, dependencyResolver);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="dependencyResolver">The function delegate that is used to assign dependencies to the result of <paramref name="resolver"/> to the cache. When any dependency changes, the object becomes invalid and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(string key, string group, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, IEnumerable<IDependency>> dependencyResolver)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            Validator.ThrowIfNull(dependencyResolver, "dependencyResolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
                    this.Add(key, result, group, dependencyResolver(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9));
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg10">The tenth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(string key, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            return this.GetOrAdd(key, NoGroup, resolver, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg10">The tenth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(string key, string group, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
                    this.Add(key, result, group);
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg10">The tenth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="absoluteExpiration">The time at which the return value of <paramref name="resolver"/> expires and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(string key, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, DateTime absoluteExpiration)
        {
            return this.GetOrAdd(key, NoGroup, resolver, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, absoluteExpiration);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg10">The tenth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="absoluteExpiration">The time at which the return value of <paramref name="resolver"/> expires and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(string key, string group, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, DateTime absoluteExpiration)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
                    this.Add(key, result, group, absoluteExpiration);
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg10">The tenth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="slidingExpiration">The interval between the time the return value of <paramref name="resolver"/> was last accessed and the time at which that object expires. If this value is the equivalent of 20 minutes, the object expires and is removed from the cache 20 minutes after it was last accessed.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(string key, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, TimeSpan slidingExpiration)
        {
            return this.GetOrAdd(key, NoGroup, resolver, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, slidingExpiration);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg10">The tenth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="slidingExpiration">The interval between the time the return value of <paramref name="resolver"/> was last accessed and the time at which that object expires. If this value is the equivalent of 20 minutes, the object expires and is removed from the cache 20 minutes after it was last accessed.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(string key, string group, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, TimeSpan slidingExpiration)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
                    this.Add(key, result, group, slidingExpiration);
                }
                return result;
            }
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the function delegate <paramref name="resolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="arg10">The tenth parameter of the function delegate <paramref name="resolver"/>.</param>
        /// <param name="dependencyResolver">The function delegate that is used to assign dependencies to the result of <paramref name="resolver"/> to the cache. When any dependency changes, the object becomes invalid and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/>. This will either be the existing value if the <paramref name="key"/> is already in the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(string key, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, IEnumerable<IDependency>> dependencyResolver)
        {
            return this.GetOrAdd(key, NoGroup, resolver, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, dependencyResolver);
        }

        /// <summary>
        /// Adds a value to the cache by using the specified function delegate <paramref name="resolver"/>, if the <paramref name="key"/> does not already exist in the virtual <paramref name="group"/> of the cache.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value in the cache.</typeparam>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="group">The virtual group to associate the <paramref name="key"/> with.</param>
        /// <param name="resolver">The function delegate that is used to resolve a value for the <paramref name="key"/>.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="arg10">The tenth parameter of the function delegate <paramref name="resolver"/> and the function delegate <paramref name="dependencyResolver"/>.</param>
        /// <param name="dependencyResolver">The function delegate that is used to assign dependencies to the result of <paramref name="resolver"/> to the cache. When any dependency changes, the object becomes invalid and is removed from the cache.</param>
        /// <returns>The value for the specified <paramref name="key"/> and <paramref name="group"/>. This will either be the existing value if the <paramref name="key"/> is already in the virtual <paramref name="group"/> of the cache, or the new value for the <paramref name="key"/> as returned by <paramref name="resolver"/> if the <paramref name="key"/> was not in virtual <paramref name="group"/> of the cache.</returns>
        public TResult GetOrAdd<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(string key, string group, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> resolver, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, IEnumerable<IDependency>> dependencyResolver)
        {
            Validator.ThrowIfNull(resolver, "resolver");
            Validator.ThrowIfNull(dependencyResolver, "dependencyResolver");
            lock (InnerCaches)
            {
                TResult result;
                if (!this.TryGetValue(key, group, out result))
                {
                    result = resolver(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
                    this.Add(key, result, group, dependencyResolver(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10));
                }
                return result;
            }
        }
    }
}