using System;

namespace Cuemon.Data.Entity.Mapping
{
    /// <summary>
    /// Designates a property to represent a database association, such as a foreign key relationship.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public sealed class AssociationAttribute : ColumnAttribute
    {
        private bool _isForeignKey;
        private string _associationStorage;
        private Type _associationStorageType;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssociationAttribute"/> class.
        /// </summary>
        public AssociationAttribute()
        {
        }

        /// <summary>
        /// Gets or sets the member as the foreign key in an association representing a database relationship.
        /// </summary>
        /// <value>
        /// 	<c>true</c> this member represent a foreign key in an association representing a database relationship; otherwise, <c>false</c>.
        /// </value>
        public bool IsForeignKey
        {
            get { return _isForeignKey; }
            set { _isForeignKey = value; }
        }

        /// <summary>
        /// Gets or sets the private storage field for the associated type to hold the value from a column.
        /// </summary>
        /// <value>The private storage field for the associated type to hold the value from a column.</value>
        public string AssociatedStorage
        {
            get { return _associationStorage; }
            set { _associationStorage = value; }
        }

        /// <summary>
        /// Gets or sets the associated type where to retreive the storage field.
        /// </summary>
        /// <value>The associated type where to retreive the storage field.</value>
        public Type AssociatedStorageType
        {
            get { return _associationStorageType; }
            set { _associationStorageType = value; }
        }
    }
}