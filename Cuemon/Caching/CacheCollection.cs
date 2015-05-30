using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Cuemon.Collections.Generic;
using Cuemon.Threading;

namespace Cuemon.Caching
{
    /// <summary>
    /// Implements a cache for an application. This class cannot be inherited.
    /// </summary>
    public sealed partial class CacheCollection : IEnumerable<KeyValuePair<long, object>>
    {
        private static readonly CacheCollection Singleton = new CacheCollection();
        private readonly Dictionary<long, Cache> InnerCaches = new Dictionary<long, Cache>();
        private const string NoGroup = null;

        internal static CacheCollection Cache
        {
            get { return Singleton; }
        }

        #region Constructors
        private CacheCollection()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the cached item with the specified key.
        /// </summary>
        /// <value></value>
        public object this[string key]
        {
            get
            {
                return this[key, NoGroup];
            }
            set
            {
                this[key, NoGroup] = value;
            }
        }

        /// <summary>
        /// Gets the cached item from the specified group with the specified key.
        /// </summary>
        /// <value></value>
        public object this[string key, string group]
        {
            get
            {
                return this.Get<object>(key, group);
            }
            set
            {
                this.Add(key, value, group);
            }
        }

        private Timer ExpirationTimer { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Retrieves the specified item from the <see cref="CacheCollection"/>.
        /// </summary>
        /// <typeparam name="T">The type of the item in the <see cref="CacheCollection"/>.</typeparam>
        /// <param name="key">The identifier of the cache item to retrieve.</param>
        /// <returns>The retrieved cache item, or the default value of the type parameter T if the key is not found.</returns>
        public T Get<T>(string key)
        {
            return this.Get<T>(key, NoGroup);
        }

        /// <summary>
        /// Retrieves the specified item from the associated group of the <see cref="CacheCollection"/>.
        /// </summary>
        /// <typeparam name="T">The type of the item in the <see cref="CacheCollection"/>.</typeparam>
        /// <param name="key">The identifier of the cache item to retrieve.</param>
        /// <param name="group">The associated group of the cache item to retrieve.</param>
        /// <returns>The retrieved cache item, or the default value of the type parameter T if the key is not found.</returns>
        public T Get<T>(string key, string group)
        {
            if (key == null) { throw new ArgumentNullException("key"); }
            T result;
            this.TryGetValue(key, group, out result);
            return result;
        }

        /// <summary>
        /// Gets the UTC date time value from when this item was added to the <see cref="CacheCollection"/>.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">When this method returns, contains the UTC date time value from when this item, with the specified key, was added; otherwise, if no item could be resolved or the item has expired, <see cref="DateTime.MinValue"/>. This parameter is passed uninitialized.</param>
        /// <returns>
        /// <c>true</c> if the <paramref name="value"/> parameter contains an element with the specified key, and the element has not expired; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="key"/> is null.
        /// </exception>
        public bool TryGetAdded(string key, out DateTime value)
        {
            return this.TryGetAdded(key, NoGroup, out value);
        }

        /// <summary>
        /// Gets the UTC date time value from when this item was added to the <see cref="CacheCollection"/>.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="group">The group of the value to get.</param>
        /// <param name="value">When this method returns, contains the UTC date time value from when this item, with the specified key, was added; otherwise, if no item could be resolved or the item has expired, <see cref="DateTime.MinValue"/>. This parameter is passed uninitialized.</param>
        /// <returns>
        /// <c>true</c> if the <paramref name="value"/> parameter contains an element with the specified key, and the element has not expired; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="key"/> is null.
        /// </exception>
        public bool TryGetAdded(string key, string group, out DateTime value)
        {
            if (key == null) { throw new ArgumentNullException("key"); }
            Cache cache;
            if (this.TryGetCache(key, group, out cache))
            {
                value = cache.Created;
                return true;
            }
            value = DateTime.MinValue;
            return false;
        }

        private static long GenerateGroupKey(string key, string group)
        {
            return StructUtility.GetHashCode64(group == NoGroup ? key : string.Concat(key, group));
        }

        /// <summary>
        /// Adds the specified <paramref name="key"/> and <paramref name="value"/> to the cache.
        /// </summary>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="value">The object to be inserted in the cache.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="key"/> is null.
        /// </exception>
        /// <remarks>
        /// This method will not throw an <see cref="ArgumentException"/> in case of an existing cache item whose key matches the key parameter.
        /// </remarks>
        public void Add(string key, object value)
        {
            this.Add(key, value, NoGroup);
        }

        /// <summary>
        /// Adds the specified <paramref name="key"/> and <paramref name="value"/> to the cache.up.
        /// </summary>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="value">The object to be inserted in the cache.</param>
        /// <param name="group">The group to associate the <paramref name="key"/> with.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="key"/> is null.
        /// </exception>
        /// <remarks>
        /// This method will not throw an <see cref="ArgumentException"/> in case of an existing cache item whose key matches the key parameter.
        /// </remarks>
        public void Add(string key, object value, string group)
        {
            this.AddCore(key, value, group, DateTime.MaxValue, TimeSpan.Zero, null);
        }

        /// <summary>
        /// Adds the specified <paramref name="key"/> and <paramref name="value"/> to the cache.
        /// </summary>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="value">The object to be inserted in the cache.</param>
        /// <param name="absoluteExpiration">The time at which the added object expires and is removed from the cache.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="key"/> is null.
        /// </exception>
        /// <remarks>
        /// This method will not throw an <see cref="ArgumentException"/> in case of an existing cache item whose key matches the key parameter.
        /// </remarks>
        public void Add(string key, object value, DateTime absoluteExpiration)
        {
            this.Add(key, value, NoGroup, absoluteExpiration);
        }

        /// <summary>
        /// Adds the specified <paramref name="key"/> and <paramref name="value"/> to the cache.
        /// </summary>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="value">The object to be inserted in the cache.</param>
        /// <param name="group">The group to associate the <paramref name="key"/> with.</param>
        /// <param name="absoluteExpiration">The time at which the added object expires and is removed from the cache.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="key"/> is null.
        /// </exception>
        /// <remarks>
        /// This method will not throw an <see cref="ArgumentException"/> in case of an existing cache item whose key matches the key parameter.
        /// </remarks>
        public void Add(string key, object value, string group, DateTime absoluteExpiration)
        {
            this.AddCore(key, value, group, absoluteExpiration, TimeSpan.Zero, null);
        }

        /// <summary>
        /// Adds the specified <paramref name="key"/> and <paramref name="value"/> to the cache.
        /// </summary>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="value">The object to be inserted in the cache.</param>
        /// <param name="slidingExpiration">The interval between the time the added object was last accessed and the time at which that object expires. If this value is the equivalent of 20 minutes, the object expires and is removed from the cache 20 minutes after it is last accessed.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="key"/> is null.
        /// </exception>
        /// <remarks>
        /// This method will not throw an <see cref="ArgumentException"/> in case of an existing cache item whose key matches the key parameter.
        /// </remarks>
        public void Add(string key, object value, TimeSpan slidingExpiration)
        {
            this.Add(key, value, NoGroup, slidingExpiration);
        }

        /// <summary>
        /// Adds the specified <paramref name="key"/> and <paramref name="value"/> to the cache.
        /// </summary>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="value">The object to be inserted in the cache.</param>
        /// <param name="group">The group to associate the <paramref name="key"/> with.</param>
        /// <param name="slidingExpiration">The interval between the time the added object was last accessed and the time at which that object expires. If this value is the equivalent of 20 minutes, the object expires and is removed from the cache 20 minutes after it was last accessed.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="key"/> is null.
        /// </exception>
        /// <remarks>
        /// This method will not throw an <see cref="ArgumentException"/> in case of an existing cache item whose key matches the key parameter.
        /// </remarks>
        public void Add(string key, object value, string group, TimeSpan slidingExpiration)
        {
            this.AddCore(key, value, group, DateTime.MaxValue, slidingExpiration, null);
        }

        /// <summary>
        /// Adds the specified <paramref name="key"/> and <paramref name="value"/> to the cache.
        /// </summary>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="value">The object to be inserted in the cache.</param>
        /// <param name="dependencies">The dependencies for the <paramref name="value"/>. When any dependency changes, the <paramref name="value"/> becomes invalid and is removed from the cache.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="key"/> is null.
        /// </exception>
        /// <remarks>
        /// This method will not throw an <see cref="ArgumentException"/> in case of an existing cache item whose key matches the key parameter.
        /// </remarks>
        public void Add(string key, object value, params IDependency[] dependencies)
        {
            this.Add(key, value, NoGroup, dependencies);
        }

        /// <summary>
        /// Adds the specified <paramref name="key"/> and <paramref name="value"/> to the cache.
        /// </summary>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="value">The object to be inserted in the cache.</param>
        /// <param name="dependencies">The dependencies for the <paramref name="value"/>. When any dependency changes, the <paramref name="value"/> becomes invalid and is removed from the cache.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="key"/> is null.
        /// </exception>
        /// <remarks>
        /// This method will not throw an <see cref="ArgumentException"/> in case of an existing cache item whose key matches the key parameter.
        /// </remarks>
        public void Add(string key, object value, IEnumerable<IDependency> dependencies)
        {
            this.Add(key, value, NoGroup, dependencies);
        }

        /// <summary>
        /// Adds the specified <paramref name="key"/> and <paramref name="value"/> to the cache.
        /// </summary>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="value">The object to be inserted in the cache.</param>
        /// <param name="group">The group to associate the <paramref name="key"/> with.</param>
        /// <param name="dependencies">The dependencies for the <paramref name="value"/>. When any dependency changes, the <paramref name="value"/> becomes invalid and is removed from the cache.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="key"/> is null.
        /// </exception>
        /// <remarks>
        /// This method will not throw an <see cref="ArgumentException"/> in case of an existing cache item whose key matches the key parameter.
        /// </remarks>
        public void Add(string key, object value, string group, params IDependency[] dependencies)
        {
            this.Add(key, value, group, EnumerableUtility.AsEnumerable(dependencies));
        }

        /// <summary>
        /// Adds the specified <paramref name="key"/> and <paramref name="value"/> to the cache.
        /// </summary>
        /// <param name="key">The cache key used to identify the item.</param>
        /// <param name="value">The object to be inserted in the cache.</param>
        /// <param name="group">The group to associate the <paramref name="key"/> with.</param>
        /// <param name="dependencies">The dependencies for the <paramref name="value"/>. When any dependency changes, the <paramref name="value"/> becomes invalid and is removed from the cache.</param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     <paramref name="key"/> is null.
        /// </exception>
        /// <remarks>
        /// This method will not throw an <see cref="ArgumentException"/> in case of an existing cache item whose key matches the key parameter.
        /// </remarks>
        public void Add(string key, object value, string group, IEnumerable<IDependency> dependencies)
        {
            this.AddCore(key, value, group, DateTime.MaxValue, TimeSpan.Zero, dependencies);
        }

        private void AddCore(string key, object value, string group, DateTime absoluteExpiration, TimeSpan slidingExpiration, IEnumerable<IDependency> dependencies)
        {
            if (key == null) { throw new ArgumentNullException("key"); }
            if ((slidingExpiration < TimeSpan.Zero) || (slidingExpiration > TimeSpan.FromDays(365))) { throw new ArgumentOutOfRangeException("slidingExpiration", "The given argument cannot be set to less than TimeSpan.Zero or more than one year."); }

            long groupKey = GenerateGroupKey(key, group);
            int tries = 0;
        Retry:
            try
            {

                Cache cache = new Cache(key, value, @group, dependencies, absoluteExpiration, slidingExpiration);
                cache.Expired += new EventHandler<CacheEventArgs>(CacheExpired);
                lock (InnerCaches)
                {
                    if (!InnerCaches.ContainsKey(groupKey))
                    {
                        cache.StartDependencies();
                        InnerCaches.Add(groupKey, cache);
                    }
                }
                if (cache.CanExpire && this.ExpirationTimer == null) { this.ExpirationTimer = new Timer(new TimerCallback(this.ExpirationTimerInvoking), null, TimeSpan.Zero, TimeSpan.FromMinutes(30)); }
            }
            catch (IndexOutOfRangeException)
            {
                tries++;
                Thread.Sleep(NumberUtility.GetRandomNumber(25, 250));
                if (tries < 10) { goto Retry; }
            }
        }

        private void ExpirationTimerInvoking(object o)
        {
            this.HandleExpiration();
        }

        private void RemoveExpired(string key, string group)
        {
            this.Remove(key, group);
        }

        private void HandleExpiration()
        {
            bool isTimerRequired = false;
            DateTime current = DateTime.UtcNow;
            IList<Cache> groupCaches = this.GetCaches();

            if (groupCaches.Count > 0)
            {
                foreach (Cache cache in groupCaches)
                {
                    if (cache == null) { continue; }
                    if (cache.CanExpire)
                    {
                        if (cache.HasExpired(current))
                        {
                            ThreadPoolUtility.QueueWork(RemoveExpired, cache.Key, cache.Group);
                        }
                        else
                        {
                            isTimerRequired = true;
                        }
                    }
                }
            }

            if (isTimerRequired) { return; }
            if (this.ExpirationTimer != null)
            {
                this.ExpirationTimer.Dispose();
                this.ExpirationTimer = null;
            }
        }

        private void CacheExpired(object sender, CacheEventArgs e)
        {
            ThreadPoolUtility.QueueWork(RemoveExpired, e.Cache.Key, e.Cache.Group);
            e.Cache.Expired -= new EventHandler<CacheEventArgs>(CacheExpired);
        }

        private IList<Cache> GetCaches()
        {
            return this.GetCaches(NoGroup);
        }

        private IList<Cache> GetCaches(string group)
        {
            DateTime current = DateTime.UtcNow;
            List<Cache> groupCaches = new List<Cache>();
            lock (InnerCaches)
            {
                try
                {
                    List<Cache> snapshot = new List<Cache>(InnerCaches.Values);
                    foreach (Cache cache in snapshot)
                    {
                        if (cache == null) { continue; } // this can happen if a cache has been removed
                        if (cache.CanExpire && cache.HasExpired(current)) { continue; }
                        if (group == NoGroup)
                        {
                            groupCaches.Add(cache); // return all
                        }
                        else if (cache.Group == group)
                        {
                            groupCaches.Add(cache); // return filtered by group
                        }
                    }
                    return groupCaches;
                }
                catch (IndexOutOfRangeException)
                {
                }
            }
            return groupCaches;
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.Dictionary`2"/> contains the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.Dictionary`2"/>.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="T:System.Collections.Generic.Dictionary`2"/> contains an element with the specified key; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="key"/> is null.
        /// </exception>
        public bool ContainsKey(string key)
        {
            return this.ContainsKey(key, NoGroup);
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.Dictionary`2"/> contains the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.Dictionary`2"/>.</param>
        /// <param name="group">The associated group of the key to locate.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="T:System.Collections.Generic.Dictionary`2"/> contains an element with the specified key; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="key"/> is null.
        /// </exception>
        public bool ContainsKey(string key, string group)
        {
            if (key == null) { throw new ArgumentNullException("key"); }
            Cache cache;
            return this.TryGetCache(key, group, out cache);
        }

        /// <summary>
        /// Removes all keys and values from the <see cref="T:System.Collections.Generic.Dictionary`2"/>.
        /// </summary>
        public void Clear()
        {
            this.Clear(NoGroup);
        }

        /// <summary>
        /// Removes all keys and values matching the specified group from the <see cref="T:System.Collections.Generic.Dictionary`2"/>.
        /// </summary>
        public void Clear(string group)
        {
            if (group == NoGroup)
            {
                lock (InnerCaches)
                {
                    InnerCaches.Clear();
                }
            }
            else
            {
                IList<Cache> groupCaches = this.GetCaches(group);
                ParallelThread.ForEach(groupCaches, ClearCore);
            }
        }

        private void ClearCore(Cache cache)
        {
            this.Remove(cache.Key, cache.Group);
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        public int Count()
        {
            return this.Count(NoGroup);
        }

        /// <summary>
        /// Gets the number of elements contained in the specified group of the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="group">The associated group to filter the count by.</param>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        /// <value></value>
        public int Count(string group)
        {
            int count = this.GetCaches(group).Count;
            return count;
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <typeparam name="T">The type of the item in the <see cref="CacheCollection"/>.</typeparam>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.Dictionary`2"/> contains an element with the specified key; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="key"/> is null.
        /// </exception>
        public bool TryGetValue<T>(string key, out T value)
        {
            return this.TryGetValue(key, NoGroup, out value);
        }

        /// <summary>
        /// Gets the value associated with the specified key and group.
        /// </summary>
        /// <typeparam name="T">The type of the item in the <see cref="CacheCollection"/>.</typeparam>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="group">The group of the value to get.</param>
        /// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param>
        /// <returns>
        /// true if the <see cref="T:System.Collections.Generic.Dictionary`2"/> contains an element with the specified key; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="key"/> is null.
        /// </exception>
        public bool TryGetValue<T>(string key, string group, out T value)
        {
            if (key == null) { throw new ArgumentNullException("key"); }
            Cache cache;
            if (this.TryGetCache(key, group, out cache))
            {
                value = (T)cache.Value;
                return true;
            }
            value = default(T);
            return false;
        }

        private bool TryGetCache(string key, string group, out Cache cache)
        {
            DateTime current = DateTime.UtcNow;
            bool hasItem;
            long groupKey = GenerateGroupKey(key, group);
            lock (InnerCaches) { hasItem = InnerCaches.TryGetValue(groupKey, out cache); }
            if (hasItem)
            {
                if (cache.CanExpire && !cache.HasExpired(current))
                {
                    cache.Refresh();
                    return true;
                }

                if (cache.CanExpire && cache.HasExpired(current))
                {
                    ThreadPoolUtility.QueueWork(RemoveExpired, key, group);
                    return false;
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes the value with the specified key from the <see cref="T:System.Collections.Generic.Dictionary`2"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>
        /// true if the element is successfully found and removed; otherwise, false.  This method returns false if <paramref name="key"/> is not found in the <see cref="T:System.Collections.Generic.Dictionary`2"/>.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="key"/> is null.
        /// </exception>
        public bool Remove(string key)
        {
            return this.Remove(key, NoGroup);
        }

        /// <summary>
        /// Removes the value with the specified key from the associated specified group of the <see cref="T:System.Collections.Generic.Dictionary`2"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <param name="group">The associated group to the key of the element to remove.</param>
        /// <returns>
        /// <c>true</c> if the element is successfully found and removed; otherwise, <c>false</c>.  This method returns <c>false</c> if <paramref name="key"/> combined with <paramref name="group"/>  is not found in the <see cref="T:System.Collections.Generic.Dictionary`2"/>.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="key"/> is null.
        /// </exception>
        public bool Remove(string key, string group)
        {
            if (key == null) { throw new ArgumentNullException("key"); }
            long groupKey = GenerateGroupKey(key, group);
            lock (InnerCaches)
            {
                return InnerCaches.Remove(groupKey);
            }
        }

        private IEnumerable<KeyValuePair<long, object>> CreateImpostor()
        {
            lock (InnerCaches)
            {
                foreach (KeyValuePair<long, Cache> keyValuePair in InnerCaches)
                {
                    if (keyValuePair.Value != null)
                    {
                        yield return new KeyValuePair<long, object>(keyValuePair.Key, keyValuePair.Value.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }


        /// <summary>
        /// Retrieves an enumerator that iterates through the key settings and their values contained in the cache.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        /// <remarks>All keys are hashed internally and will not provide useful information.</remarks>
        public IEnumerator<KeyValuePair<long, object>> GetEnumerator()
        {
            List<KeyValuePair<long, object>> impostor = new List<KeyValuePair<long, object>>(this.CreateImpostor());
            return impostor.GetEnumerator();
        }
        #endregion
    }
}