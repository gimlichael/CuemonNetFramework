using System;
namespace Cuemon.Data.Entity
{
	/// <summary>
	/// The following tables list the members exposed by the IBusinessEntityRepository type.
	/// </summary>
	public interface IBusinessEntityRepository : IBusinessEntity
	{
		/// <summary>
		/// Saves this object to a data source and reloads the instance afterwards.
		/// </summary>
		void Save();

		/// <summary>
		/// Saves this object to a data source. 
		/// Does not reload the instance from the data source.
		/// </summary>
		void SaveOnly();

		/// <summary>
		/// Deletes this object from a data source.
		/// </summary>
		void Delete();

		/// <summary>
		/// Loads this object from a data source.
		/// </summary>
		void Load();

		///// <summary>
		///// Gets an <see cref="IBusinessEntityDataAdapter"/> reference for this <see cref="IBusinessEntityRepository"/> instance.
		///// </summary>
		///// <value>An <see cref="IBusinessEntityDataAdapter"/> reference for this <see cref="IBusinessEntityRepository"/> instance.</value>
		//IBusinessEntityDataAdapter DataAdapter { get; }
	}
}