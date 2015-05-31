using System;

namespace Cuemon.Data.Entity
{
    /// <summary>
    /// Provides data for <see cref="EntityDataAdapter"/> related operations.
    /// </summary>
    public sealed class EntityDataAdapterEventArgs : DataAdapterEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityDataAdapterEventArgs"/> class.
        /// </summary>
        private EntityDataAdapterEventArgs() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityDataAdapterEventArgs"/> class.
        /// </summary>
        /// <param name="entity">The entity of the data binding operation.</param>
        /// <param name="chainedType">The current data bound type of the <paramref name="entity"/> chained hierarchy.</param>
        public EntityDataAdapterEventArgs(Entity entity, Type chainedType)
        {
            this.Entity = entity;
            this.ChainedType = chainedType;
        }

        /// <summary>
        /// Gets the entity of the data binding operation.
        /// </summary>
        /// <value>The entity of the data binding operation.</value>
        public Entity Entity { get; private set; }

        /// <summary>
        /// Gets the current data bound type of the <see cref="Entity"/> chained hierarchy.
        /// </summary>
        /// <value>The current data bound type of the <see cref="Entity"/> chained hierarchy.</value>
        public Type ChainedType { get; private set; }

        /// <summary>
        /// Represents an <see cref="DataAdapter"/> event with no event data.
        /// </summary>
        new public static readonly DataAdapterEventArgs Empty = new EntityDataAdapterEventArgs();
    }
}
