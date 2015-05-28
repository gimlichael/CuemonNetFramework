using System;

namespace Cuemon.Data.Entity.Mapping
{
    /// <summary>
    /// Specifies certain attributes of a class that is associated with a database table.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = false)]
    public sealed class TableAttribute : Attribute
    {
        private string _name;
        private string _nameAlias;

        /// <summary>
        /// Gets or sets the name of the table or view.
        /// </summary>
        /// <value>The name of the table or view.</value>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets or sets the table name or view alias to use in conjuction with <see cref="Name"/>.
        /// </summary>
        /// <value>The table name or view alias to use in conjuction with <see cref="Name"/>.</value>
        /// <remarks>This property can be set to help clarify meaning in for instance XML related queries.</remarks>
        public string NameAlias
        {
            get { return _nameAlias; }
            set { _nameAlias = value; }
        }
    }
}