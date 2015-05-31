using System;

namespace Cuemon.Data.Entity
{
	/// <summary>
    /// An interface providing methods of handling all data binding related logic for an <see cref="Entity"/> implemented class.
	/// </summary>
	public interface IEntityDataAdapter
	{
        /// <summary>
        /// Deletes the specified <paramref name="entityType" /> from the data source.
        /// </summary>
        /// <param name="entityType">The entity type to delete from the data source.</param>
		void Delete(Type entityType);

        /// <summary>
        /// Modifies the specified <paramref name="entityType"/> in the data source.
        /// </summary>
        /// <param name="entityType">The entity type to modify in the data source.</param>
		void Modify(Type entityType);
       
		/// <summary>
        /// Creates the specified <paramref name="entityType"/> in the data source.
		/// </summary>
        /// <param name="entityType">The entity type to create in the data source.</param>
        void Create(Type entityType);

        
		/// <summary>
		/// Opens this instance from the data source using the reflected columns of the data mapped <see cref="Entity"/>.
		/// </summary>
        void Open(Type entityType);


		/// <summary>
        /// Lists a collection of instances from the data source using the reflected columns of the data mapped <see cref="Entity"/>.
		/// </summary>
        void OpenList<T>(Type entityType) where T : BusinessEntity;

        /// <summary>
        /// Gets the <see cref="EntityDataAdapterSettings"/> object of this <see cref="IEntityDataAdapter"/>.
        /// </summary>
        /// <value>The <see cref="EntityDataAdapterSettings"/> object of this <see cref="IEntityDataAdapter"/> instance.</value>
        EntityDataAdapterSettings Settings { get; }
	}
}