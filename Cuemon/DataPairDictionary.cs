﻿using System.Collections.Generic;

namespace Cuemon
{
    /// <summary>
    /// Provides a collection of <see cref="DataPair"/>. This class cannot be inherited.
    /// </summary>
    public sealed class DataPairDictionary : Dictionary<string, DataPair>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataPairDictionary"/> class.
        /// </summary>
        public DataPairDictionary()
            : base()
        {
        }

        /// <summary>
        /// Adds a new <see cref="DataPair{T}"/> to the end of this <see cref="DataPairDictionary"/>.
        /// </summary>
        /// <typeparam name="T">The type of the data being added to this instance.</typeparam>
        /// <param name="name">The name of the data pair.</param>
        /// <param name="value">The value of the data pair.</param>
        public void Add<T>(string name, T value)
        {
            base.Add(name, new DataPair<T>(name, value));
        }
    }
}