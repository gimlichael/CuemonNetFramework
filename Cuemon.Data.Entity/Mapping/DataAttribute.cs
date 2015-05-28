using System;
using System.Globalization;
using System.Reflection;

namespace Cuemon.Data.Entity.Mapping
{
    /// <summary>
    /// Provides members to describe attributes of data in columns.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public abstract class DataAttribute : Attribute
    {
        private string _name;
        private string _storage;
        private Type _storageType;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataAttribute"/> class.
        /// </summary>
        protected DataAttribute()
        {
        }

        /// <summary>
        /// Gets or sets the name of a column.
        /// </summary>
        /// <value>The name of a column.</value>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets or sets a private storage field to hold the value from a column.
        /// </summary>
        /// <value>The name of the storage field.</value>
        public string Storage
        {
            get { return _storage; }
            set { _storage = value; }
        }

        /// <summary>
        /// Gets or sets the type where to retrieve the storage field.
        /// Default is the class the attribute is located.
        /// </summary>
        /// <value>The type where to retrieve the storage field.</value>
        public Type StorageType
        {
            get { return _storageType; }
            set { _storageType = value; }
        }

        /// <summary>
        /// Gets an instance to the originating decorated property.
        /// </summary>
        /// <value>An instance to of the originating decorated property.</value>
        /// <remarks>This property is set during invoke of the <see cref="DataMapperUtility.ParseColumns"/> method.</remarks>
        internal PropertyInfo DecoratedProperty { get; set; }

        /// <summary>
        /// Gets a value indicating whether to assume automatic property implementation when associating a private storage field to hold the value from a column.
        /// </summary>
        /// <value><c>true</c> if <see cref="Storage"/> is either null or empty; otherwise, <c>false</c>.</value>
        public bool AssumeAutoProperty
        {
            get { return string.IsNullOrEmpty(this.Storage); }
        }

        /// <summary>
        /// Resolves the name of the private storage field to hold the value from a column either specified by <see cref="Storage"/> or of an automatic property implementation.
        /// </summary>
        /// <value>The resolved name of the storage field.</value>
        public string ResolveStorage()
        {
            return (this.AssumeAutoProperty && this.DecoratedProperty != null) ? string.Format(CultureInfo.InvariantCulture, "<{0}>k__BackingField", this.DecoratedProperty.Name) : this.Storage;
        }
    }
}