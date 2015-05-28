﻿using System;
using System.Globalization;

namespace Cuemon
{
    /// <summary>
    /// Represents a generic way to provide information about arbitrary data.
    /// </summary>
    public abstract class DataPair
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataPair"/> class.
        /// </summary>
        /// <param name="name">The name of the data pair.</param>
        /// <param name="value">The value of the data pair.</param>
        protected DataPair(string name, object value)
        {
            Validator.ThrowIfNullOrEmpty(name, "name");
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Gets the name of the data pair.
        /// </summary>
        /// <value>The name of the data pair.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the value of the data pair.
        /// </summary>
        /// <value>The value of the data pair.</value>
        public object Value { get; private set; }

        /// <summary>
        /// Gets a value indicating whether <see cref="Value"/> is not null.
        /// </summary>
        /// <value><c>true</c> if <see cref="Value"/> is not null; otherwise, <c>false</c>.</value>
        public bool HasValue
        {
            get { return this.Value != null; }
        }

        /// <summary>
        /// Gets the type of the data pair value.
        /// </summary>
        /// <value>The type of the data pair value.</value>
        public abstract Type Type { get; }
    }

    /// <summary>
    /// Represents a generic way to provide information about arbitrary data. This class cannot be inherited.
    /// </summary>
    /// <typeparam name="T">The type of the data value being represented by this instance.</typeparam>
    public sealed class DataPair<T> : DataPair
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataPair" /> class.
        /// </summary>
        /// <param name="name">The name of the data pair.</param>
        /// <param name="value">The value of the data pair.</param>
        public DataPair(string name, T value) : base(name, value)
        {   
        }

        /// <summary>
        /// Gets the type of the data pair value.
        /// </summary>
        /// <value>The type of the data pair value.</value>
        public override Type Type
        {
            get { return typeof(T); }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "Name: {0}, Value: {1}, Type: {2}", this.Name, this.Value, this.Type.Name);
        }
    }
}
