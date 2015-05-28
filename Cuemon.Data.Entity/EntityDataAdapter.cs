using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Transactions;
using Cuemon.Caching;
using Cuemon.Collections.Generic;
using Cuemon.Data.Entity.Mapping;
using Cuemon.Reflection;
using Cuemon.Threading;

namespace Cuemon.Data.Entity
{
	/// <summary>
	/// An abstract class for handling all data binding related logic for an <see cref="Entity"/> implemented class.
	/// </summary>
	public abstract class EntityDataAdapter : DataAdapter, IEntityDataAdapter
	{
        private readonly Entity _entity;
	    private readonly EntityDataAdapterSettings _settings = new EntityDataAdapterSettings();

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="EntityDataAdapter"/> class.
		/// </summary>
		/// <param name="entity">The calling business entity.</param>
        protected EntityDataAdapter(Entity entity)
			: this(entity, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EntityDataAdapter"/> class.
		/// </summary>
		/// <param name="entity">The calling business entity.</param>
		/// <param name="manager">The data manager to be used for the underlying operations.</param>
        protected EntityDataAdapter(Entity entity, DataManager manager)
            : base(manager)
		{
			_entity = entity;
		}
		#endregion

		#region Events
		/// <summary>
		/// Occurs just before the data is being bound and processed.
		/// </summary>
		public event EventHandler<EntityDataAdapterEventArgs> DataBinding;


		/// <summary>
		/// Occurs just after the data has been bound, processed and optional distributed.
		/// </summary>
        public event EventHandler<EntityDataAdapterEventArgs> DataBindingCompleted;
		#endregion

		#region Properties
		/// <summary>
        /// Gets the reference to the calling <see cref="Cuemon.Data.Entity.Entity"/>.
		/// </summary>
		/// <value>A reference to the calling <see cref="Cuemon.Data.Entity.Entity"/>.</value>
        protected Entity Entity
		{
			get { return _entity; }
		}

        /// <summary>
        /// Gets the <see cref="EntityDataAdapterSettings" /> object of this <see cref="EntityDataAdapter" />.
        /// </summary>
        /// <value>The <see cref="EntityDataAdapterSettings" /> object of this <see cref="EntityDataAdapter" /> instance.</value>
        public EntityDataAdapterSettings Settings { get { return _settings; } }
		#endregion

		#region Methods
		/// <summary>
		/// Raises the <see cref="DataBindingCompleted"/> event.
		/// </summary>
        /// <param name="entity">The entity that caused the data binding.</param>
        /// <param name="chainedType">The current data bound type of the <paramref name="entity"/> chained hierarchy.</param>
        protected virtual void OnDataBindingCompleted(Entity entity, Type chainedType)
		{
            EventHandler<EntityDataAdapterEventArgs> handler = DataBindingCompleted;
            EventUtility.Raise(handler, this, new EntityDataAdapterEventArgs(entity, chainedType));
		}

        /// <summary>
        /// Raises the <see cref="DataBinding" /> event.
        /// </summary>
        /// <param name="entity">The entity that caused the data binding.</param>
        /// <param name="chainedType">The current data bound type of the <paramref name="entity"/> chained hierarchy.</param>
		protected virtual void OnDataBinding(Entity entity, Type chainedType)
		{
			EventHandler<EntityDataAdapterEventArgs> handler = DataBinding;
			EventUtility.Raise(handler, this, new EntityDataAdapterEventArgs(entity, chainedType));
		}

        /// <summary>
        /// Deletes the specified <paramref name="entityType" /> from the data source.
        /// </summary>
        /// <param name="entityType">The entity type to delete from the data source.</param>
        public void Delete(Type entityType)
		{
            Validator.ThrowIfNull(entityType, "entityType");
            this.HasRequiredAttributes(entityType);
            this.OnDataBinding(this.Entity, entityType);
            this.Delete(new EntityMapper(this.Entity, entityType));
            this.OnDataBindingCompleted(this.Entity, entityType);
	    }

        /// <summary>
        /// Deletes data from the data source as resolved by the specified <paramref name="mapper"/>.
        /// </summary>
        /// <param name="mapper">The <see cref="EntityMapper"/> of this instance.</param>
	    protected virtual void Delete(EntityMapper mapper)
	    {
            Validator.ThrowIfNull(mapper, "mapper");
	        mapper.ParseDeleteStatement(base.Delete);
	    }

		/// <summary>
        /// Modifies the specified <paramref name="entityType"/> in the data source.
		/// </summary>
        /// <param name="entityType">The entity type to update in the data source.</param>
        public void Modify(Type entityType) 
		{
            Validator.ThrowIfNull(entityType, "entityType");
            this.HasRequiredAttributes(entityType);
            this.OnDataBinding(this.Entity, entityType);
            this.Modify(new EntityMapper(this.Entity, entityType));
            this.OnDataBindingCompleted(this.Entity, entityType);
		}

        /// <summary>
        /// Modifies data in the data source as resolved by the specified <paramref name="mapper"/>.
        /// </summary>
        /// <param name="mapper">The <see cref="EntityMapper"/> of this instance.</param>
	    protected virtual void Modify(EntityMapper mapper)
	    {
            Validator.ThrowIfNull(mapper, "mapper");
            mapper.ParseUpdateStatement(base.Update);
	    }
        
		/// <summary>
        /// Determines from the specified <paramref name="entityType"/> whether a record already exists in the data source.
		/// </summary>
        /// <param name="entityType">The entity type to locate in the data source.</param>
        /// <returns><c>true</c> if a record exists in the data source; otherwise <c>false</c>.</returns>
        public bool Exists(Type entityType)
		{
            Validator.ThrowIfNull(entityType, "entityType");
            this.HasRequiredAttributes(entityType);
            this.OnDataBinding(this.Entity, entityType);
		    bool result = this.Exists(new EntityMapper(this.Entity, entityType));
            this.OnDataBindingCompleted(this.Entity, entityType);
		    return result;
		}

        /// <summary>
        /// Determines the existing of data from the data source as resolved by the specified <paramref name="mapper"/>.
        /// </summary>
        /// <param name="mapper">The <see cref="EntityMapper"/> of this instance.</param>
        /// <returns><c>true</c> if a record exists in the data source; otherwise <c>false</c>.</returns>
	    protected virtual bool Exists(EntityMapper mapper)
	    {
            Validator.ThrowIfNull(mapper, "mapper");
            return mapper.ParseSelectExistsStatement(base.Manager.ExecuteExists);
	    }
        
        /// <summary>
        /// Creates the specified <paramref name="entityType"/> in the data source.
        /// </summary>
        /// <param name="entityType">The entity type to create in the data source.</param>
        public void Create(Type entityType)
		{
            Validator.ThrowIfNull(entityType, "entityType");
            this.HasRequiredAttributes(entityType);
            this.OnDataBinding(this.Entity, entityType);
            this.Create(new EntityMapper(this.Entity, entityType));
            this.OnDataBindingCompleted(this.Entity, entityType);
		}

        /// <summary>
        /// Creates data in the data source as resolved by the specified <paramref name="mapper"/>.
        /// </summary>
        /// <param name="mapper">The <see cref="EntityMapper"/> of this instance.</param>
	    protected virtual void Create(EntityMapper mapper)
	    {
            Validator.ThrowIfNull(mapper, "mapper");
	        mapper.ParseInsertStatement(base.Insert);
	    }

		/// <summary>
        /// Opens the specified <paramref name="entityType"/> from the data source.
		/// </summary>
        /// <param name="entityType">The entity type to select in the data source.</param>
        public void Open(Type entityType)
		{
            Validator.ThrowIfNull(entityType, "entityType");
            this.HasRequiredAttributes(entityType);
            this.OnDataBinding(this.Entity, entityType);
            this.Open(new EntityMapper(this.Entity, entityType));
            this.OnDataBindingCompleted(this.Entity, entityType);
		}

        /// <summary>
        /// Reads data from the data source as resolved by the specified <paramref name="mapper"/>.
        /// </summary>
        /// <param name="mapper">The <see cref="EntityMapper"/> of this instance.</param>
	    protected virtual void Open(EntityMapper mapper)
	    {
            Validator.ThrowIfNull(mapper, "mapper");
            mapper.ParseSelectStatement(base.Select);
	    }

        /// <summary>
        /// Opens a collection of <paramref name="entityType" /> from the data source.
        /// </summary>
        /// <typeparam name="T">The type of the entity in the <paramref name="entityType" />.</typeparam>
        /// <param name="entityType">The current type of the chained <see cref="Entity" /> hierarchy.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="entityType"/> is null.
        /// </exception>
        public void OpenList<T>(Type entityType)  where T : BusinessEntity
		{
            Validator.ThrowIfNull(entityType, "entityType");
            this.HasRequiredAttributes(entityType);
            this.OnDataBinding(this.Entity, entityType);
            this.OpenList<T>(new EntityMapper(this.Entity, entityType));
            this.OnDataBindingCompleted(this.Entity, entityType);
		}

        /// <summary>
        /// Reads data from the data source as resolved by the specified <paramref name="mapper"/>.
        /// </summary>
        /// <typeparam name="T">The type of the entity to propagate into a <see cref="ICollection{T}"/>.</typeparam>
        /// <param name="mapper">The <see cref="EntityMapper"/> of this instance.</param>
	    protected virtual void OpenList<T>(EntityMapper mapper) where T : Entity
	    {
            Validator.ThrowIfNull(mapper, "mapper");
            mapper.ParseSelectManyStatement<T>(base.Select);
	    }

        /// <summary>
		/// A specialized method for promotion of this <see cref="BusinessEntity"/> object with the specified new rank.
		/// </summary>
		/// <typeparam name="T">The new rank type of the <see cref="BusinessEntity"/>.</typeparam>
		/// <param name="newRank">The new rank to apply for a <see cref="BusinessEntity"/> object hierarchy.</param>
		/// <remarks>Experimental implementation tested on normal 1-1 entity relationships.</remarks>
        public virtual void Promote<T>(T newRank) where T : BusinessEntity
		{
            Type entityType = typeof(T);
            Type currentEntityType = this.Entity.GetType();
			if (entityType == currentEntityType) { return; }
            EntityMapper mapper = new EntityMapper(this.Entity, currentEntityType);
            mapper.ParsePromoteStatement(base.Insert, newRank);
		}

		/// <summary>
		/// A specialized method for demotion of an <see cref="Entity"/> object.
		/// </summary>
		/// <typeparam name="T">The type to apply as a new rank to an <see cref="Entity"/> object hierarchy.</typeparam>
        public virtual void Demote<T>() where T : BusinessEntity
		{
            Type entityType = typeof(T);
		    Type currentEntityType = this.Entity.GetType();
            if (entityType == currentEntityType) { return; }
            EntityMapper mapper = new EntityMapper(this.Entity, currentEntityType);
		    mapper.ParseDemoteStatement(base.Delete, entityType);
		}

        /// <summary>
        /// Determines whether the specified <paramref name="entityType"/> is decorated with the required attributes.
        /// </summary>
        /// <param name="entityType">The type of the <see cref="Entity"/>.</param>
        /// <exception cref="System.InvalidOperationException">
        /// <paramref name="entityType"/> does not contain the required <see cref="TableAttribute"/>.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// <paramref name="entityType"/> does not contain the required <see cref="DataSourceAttribute"/>.
        /// </exception>
        protected virtual void HasRequiredAttributes(Type entityType)
	    {
            TableAttribute table = ReflectionUtility.GetAttribute<TableAttribute>(entityType, true);
            DataSourceAttribute dataSource = ReflectionUtility.GetAttribute<DataSourceAttribute>(entityType, true);

            if (table == null) { throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Missing TableAttribute decoration of Entity:{0} using EntityDataAdapter:{1}.", entityType, this.GetType())); }
            if (dataSource == null) { throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Missing DataSourceAttribute decoration of Entity:{0} using EntityDataAdapter:{1}.", entityType, this.GetType())); }
	    }

		internal bool ValidateUniqueIndex(Type entityType, ColumnAttribute column, object uniqueValue)
		{
            this.HasRequiredAttributes(entityType);
            EntityMapper mapper  = new EntityMapper(this.Entity);
		    return mapper.HasUniqueIndexValidation(entityType, column, uniqueValue, base.Manager.ExecuteExists);
		}

		/// <summary>
		/// The core method for returning an <see cref="IDataParameter"/> compatible object to use against the preferred data source.
		/// </summary>
		/// <param name="column">An instance of the <see cref="ColumnAttribute"/> object.</param>
		/// <param name="value">The value to assign the <see cref="IDataParameter"/> compatible object.</param>
		/// <returns>An <see cref="IDataParameter"/> compatible object.</returns>
		public abstract IDataParameter GetDataParameter(ColumnAttribute column, object value);

		/// <summary>
		/// The core method for building a data mapped query.
		/// </summary>
		/// <param name="queryType">The query type to use in data source.</param>
        /// <param name="mapper">The mapper that parses an entity type to a data source.</param>
		/// <returns>
        /// An object implementing the <see cref="IDataCommand"/> interface containing the resolved query from data binding.
		/// </returns>
        public abstract IDataCommand GetDataMappedQuery(QueryType queryType, DataMapper mapper);
		#endregion
	}
}