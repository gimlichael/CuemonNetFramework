using System;
using System.Collections;
using System.Collections.Generic;
using Cuemon.Data.Entity.SqlClient;

namespace Cuemon.Data.Entity
{
	/// <summary>
	/// Represent a generic <see cref="Cuemon.Data.Entity.BusinessEntityCollection&lt;T&gt;"/> collection.
	/// </summary>
	public abstract class BusinessEntityCollection<T> : Entity, IIdentifiers, ICollection<T> where T : BusinessEntity
	{
		private List<T> _innerCollection = new List<T>();

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="BusinessEntityCollection&lt;T&gt;"/> class.
		/// </summary>
		protected BusinessEntityCollection() : base()
		{
		}
		#endregion

		#region Properties
		private List<T> InnerCollection
		{
			get
			{
				if (!this.HasLoaded) { this.Load(); }
				return _innerCollection;
			}
			set { _innerCollection = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="BusinessEntity"/> at the specified index.
		/// </summary>
		/// <value></value>
		public virtual T this[int index]
		{
			get { return this.InnerCollection[index]; }
			set { this.InnerCollection[index] = value; }
		}

		/// <summary>
		/// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </summary>
		/// <value></value>
		/// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</returns>
		public int Count
		{
			get { return this.InnerCollection.Count; }
		}
	    #endregion

		#region Methods
        /// <summary>
        /// Parse and match and entity relation from the specified <paramref name="identifier"/>.
        /// </summary>
        /// <param name="identifier">The identifier to search and match in this instance.</param>
        /// <returns>The value associated with the specified <paramref name="identifier"/> of this instance or the default value for the type of the <typeparamref name="T"/> parameter.</returns>
		public T ParseEntityRelation(string identifier)
		{
			return BusinessEntityUtility.ParseEntityRelation<T>(this, identifier);
		}

		/// <summary>
		/// Gets the <see cref="EntityDataAdapter"/> associated with this <see cref="BusinessEntityCollection{T}"/>
		/// </summary>
		/// <returns>An implementation of the <see cref="EntityDataAdapter"/> object.</returns>
        protected override EntityDataAdapter InitializeDataAdapter()
        {
            if (string.IsNullOrEmpty(DataManager.DefaultConnectionString)) { throw new NotImplementedException("You need to override this method - or - navigate to the Cuemon.Data.DataManager class and specify a value on the static DefaultConnectionString property for a default Microsoft SQL Server implementation."); }
            return new DefaultSqlEntityDataAdapter(this);
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
	    protected override void InitializeCore()
	    {
	    }

		/// <summary>
		/// Gets the type of elements in this collection.
		/// </summary>
		public Type GetEntityType()
		{
			return typeof(T);
		}

		/// <summary>
		/// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </summary>
		/// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
		public virtual void Add(T item)
		{
			this.InnerCollection.Add(item);
		}

		/// <summary>
		/// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </summary>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only. </exception>
		public void Clear()
		{
			this.InnerCollection.Clear();
		}

		/// <summary>
		/// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
		/// </summary>
		/// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
		/// <returns>
		/// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
		/// </returns>
		public virtual bool Contains(T item)
		{
			return this.InnerCollection.Contains(item);
		}

		/// <summary>
		/// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
		/// </summary>
		/// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
		/// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
		/// <exception cref="T:System.ArgumentNullException">
		/// 	<paramref name="array"/> is null.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		/// 	<paramref name="arrayIndex"/> is less than 0.</exception>
		/// <exception cref="T:System.ArgumentException">
		/// 	<paramref name="array"/> is multidimensional.-or-<paramref name="arrayIndex"/> is equal to or greater than the length of <paramref name="array"/>.-or-The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.-or-Type <typeparamref name="T"/> cannot be cast automatically to the type of the destination <paramref name="array"/>.</exception>
		public void CopyTo(T[] array, int arrayIndex)
		{
			this.InnerCollection.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </summary>
		/// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
		/// <returns>
		/// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
		/// </returns>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.</exception>
		public bool Remove(T item)
		{
			return this.InnerCollection.Remove(item);
		}
        
		/// <summary>
		/// Loads a collection of objects from a data source.
		/// </summary>
		public override void Load(Type entityType)
        {
        	this.DataAdapter.OpenList<T>(entityType);
		}

        /// <summary>
        /// Saves this object to a data source.
        /// Does not reload the instance from the data source.
        /// </summary>
        /// <param name="entityType">The entity type to save to the data source.</param>
        /// <param name="action">The query action to perform. Can be either <see cref="QueryType.Insert" /> or <see cref="QueryType.Update" />.</param>
	    public override void SaveOnly(Type entityType, QueryType action)
	    {
            foreach (T entity in this)
            {
                if (entity.GetType() != entityType) { continue; }
                entity.SaveOnly(entityType, action);
            }
	    }

        /// <summary>
        /// Deletes the specified <paramref name="entityType" /> from the data source.
        /// </summary>
        /// <param name="entityType">The entity type to delete from the data source.</param>
        public override void Delete(Type entityType)
	    {
	        foreach (T entity in this)
	        {
                if (entity.GetType() != entityType) { continue; }
                entity.Delete(entityType);
	        }
	    }

	    /// <summary>
		/// Gets a comma (,) delimited string of identifiers of this object.
		/// </summary>
		/// <returns>A <see cref="T:System.String"/> object.</returns>
		public virtual string GetIdentifiers()
	    {
	        return ConvertUtility.ToDelimitedString(this);
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<T> GetEnumerator()
		{
			return this.InnerCollection.GetEnumerator();
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
		#endregion
	}
}