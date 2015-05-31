using System;
using Cuemon.Data.Entity.SqlClient;

namespace Cuemon.Data.Entity
{
	/// <summary>
	/// An abstract class representing a Business Entity to be used in your business logic code.
	/// </summary>
	public abstract class BusinessEntity : Entity, IIdentifier
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="BusinessEntity"/> class.
		/// </summary>
		protected BusinessEntity() : base()
		{
		}
		#endregion

		#region Properties
		#endregion

		#region Methods
        /// <summary>
        /// Initializes the <see cref="EntityDataAdapter"/> associated with this <see cref="BusinessEntity"/>
        /// </summary>
        /// <returns>An implementation of the <see cref="EntityDataAdapter"/> object.</returns>
        protected override EntityDataAdapter InitializeDataAdapter()
        {
            if (string.IsNullOrEmpty(DataManager.DefaultConnectionString)) { throw new NotImplementedException("You need to override this method - or - navigate to the Cuemon.Data.DataManager class and specify a value on the static DefaultConnectionString property for a default Microsoft SQL Server implementation."); }
            return new DefaultSqlEntityDataAdapter(this);
        }

		/// <summary>
		/// Gets the identifier of this instance as a string.
		/// </summary>
		/// <returns>A <see cref="System.String"/> object.</returns>
		public abstract string GetIdentifier();

        /// <summary>
        /// An experimental method to create a <see cref="BusinessEntity"/> instance from the descendant-or-self <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The base type of <see cref="object.GetType()"/> to lookup derived types from.</typeparam>
        /// <param name="entityConstructorArgs">The entity constructor args to dynamically invoke a new <see cref="BusinessEntity"/>.</param>
        /// <returns>An instance of either the source type of <see cref="object.GetType()"/> or the derived version of <typeparamref name="TEntity"/>.</returns>
        protected TEntity CreateDescendantOrSelf<TEntity>(params object[] entityConstructorArgs) where TEntity : BusinessEntity
        {
            return this.CreateDescendantOrSelf<TEntity>(this.GetType(), entityConstructorArgs);
        }

        /// <summary>
        /// An experimental method to create a <see cref="BusinessEntity"/> instance from the descendant-or-self <typeparamref name="TEntity"/>.
        /// </summary>
        /// <typeparam name="TEntity">The base type of the <paramref name="entityType"/> to lookup derived types from.</typeparam>
        /// <param name="entityType">The type to search derived implementations from (descendant-or-self).</param>
        /// <param name="entityConstructorArgs">The entity constructor args to dynamically invoke a new <see cref="BusinessEntity"/>.</param>
        /// <returns>An instance of either the source type <paramref name="entityType"/> or the derived version of <typeparamref name="TEntity"/>.</returns>
        protected TEntity CreateDescendantOrSelf<TEntity>(Type entityType, params object[] entityConstructorArgs) where TEntity : BusinessEntity
        {
            return BusinessEntityUtility.CreateDescendantOrSelfEntity<TEntity>(entityType, this.DataAdapter, entityConstructorArgs);
        }

        /// <summary>
        /// Demotes the current instance to it's predecessor specified <typeparamref name="TEntity"/> type.
        /// </summary>
        /// <typeparam name="TEntity">The demoted rank type to apply to this instance.</typeparam>
        /// <param name="entityConstructorArgs">The constructor arguments necessary to retrieve the demoted instance from a data source.</param>
        /// <returns>The demoted (predecessor) instance of this instance.</returns>
        public virtual TEntity Demote<TEntity>(params object[] entityConstructorArgs) where TEntity : BusinessEntity
        {
            if (this.IsReadOnly) { throw new NotSupportedException("This instance is in a read-only state - changes to the repository has been disabled."); }
            if (this.IsNew) { throw new InvalidOperationException("The instance state is new, why it cannot be demoted to a lower rank."); }
            this.DataAdapter.Demote<TEntity>();
            return BusinessEntityUtility.CreateDescendantOrSelfEntity<TEntity>(typeof(TEntity), this.DataAdapter, entityConstructorArgs);
        }

        /// <summary>
        /// Promotes the current instance to it's derived specified <paramref name="newRank"/> instance.
        /// </summary>
        /// <typeparam name="TEntity">The promoted rank type to apply to this instance.</typeparam>
        /// <param name="newRank">The new rank to apply to this instance object hierarchy.</param>
        /// <param name="entityConstructorArgs">The constructor arguments necessary to retrieve the promoted instance from a data source.</param>
        /// <returns>The promoted (derived) instance of this instance.</returns>
        public virtual TEntity Promote<TEntity>(TEntity newRank, params object[] entityConstructorArgs) where TEntity : BusinessEntity
        {
            if (this.IsReadOnly) { throw new NotSupportedException("This instance is in a read-only state - changes to the repository has been disabled."); }
            if (this.IsNew) { throw new InvalidOperationException("The instance state is new, why it cannot be promoted to a higher rank."); }
            this.DataAdapter.Promote(newRank);
            return BusinessEntityUtility.CreateDescendantOrSelfEntity<TEntity>(typeof(TEntity), this.DataAdapter, entityConstructorArgs);
        }
		#endregion
	}
}