using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Transactions;
using System.Xml.Serialization;
using Cuemon.Caching;
using Cuemon.Collections.Generic;
using Cuemon.Data.Entity.Mapping;
using Cuemon.Reflection;

namespace Cuemon.Data.Entity
{
	/// <summary>
	/// Infrastructure. This utility class is designed to make common <see cref="BusinessEntity"/> related operations easier to comprehend.
	/// </summary>
	public static class BusinessEntityUtility
	{
        /// <summary>
        /// Parse and match and entity relation from the specified <paramref name="identifier"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the T entity.</typeparam>
        /// <param name="entities">The entities (implementing the <see cref="IIdentifier"/> interface) to search for a matching <paramref name="identifier"/>.</param>
        /// <param name="identifier">The identifier to search and match in the <paramref name="entities"/> sequence.</param>
        /// <returns>The value associated with the specified <paramref name="identifier"/> of the <paramref name="entities"/> sequence or the default value for the type of the <typeparamref name="TEntity"/> parameter.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="entities"/> is null.</exception>
        /// <exception cref="System.ArgumentNullException"><paramref name="identifier"/> is null.</exception>
        public static TEntity ParseEntityRelation<TEntity>(IEnumerable<TEntity> entities, string identifier) where TEntity : IIdentifier
        {
            if (entities == null) { throw new ArgumentNullException("entities"); }
            if (identifier == null) { throw new ArgumentNullException("identifier"); }
            foreach (TEntity entity in entities)
            {
                if (entity.GetIdentifier() == identifier) { return entity; }
            }
            return default(TEntity);
        }

	    /// <summary>
	    /// An experimental method to create <see cref="BusinessEntity"/> instances from the ancestor-and-descendant-or-self <typeparamref name="T"/>.
	    /// </summary>
	    /// <typeparam name="T">The type to lookup derived types from.</typeparam>
	    /// <param name="adapter">An instance of a <see cref="EntityDataAdapter"/> to lookup derived types in the associated database.</param>
	    /// <param name="entityConstructorArgs">The entity constructor args to dynamically invoke a new <see cref="BusinessEntity"/>.</param>
	    /// <returns>An instance of ancestor-and-descendant-or-self derived version of <typeparamref name="T"/>.</returns>
        public static T CreateDescendantOrSelfEntity<T>(EntityDataAdapter adapter, params object[] entityConstructorArgs) where T : Entity
	    {
            return CreateDescendantOrSelfEntity<T>(typeof(T), adapter, entityConstructorArgs);
	    }

		/// <summary>
		/// An experimental method to create <see cref="BusinessEntity"/> instances from the ancestor-and-descendant-or-self <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T">The base type of the <paramref name="derived"/> to lookup derived types from.</typeparam>
		/// <param name="derived">The type to search derived implementations from (descendant-or-self).</param>
		/// <param name="adapter">An instance of a <see cref="EntityDataAdapter"/> to lookup derived types in the associated database.</param>
		/// <param name="entityConstructorArgs">The entity constructor args to dynamically invoke a new <see cref="BusinessEntity"/>.</param>
		/// <returns>An instance of ancestor-and-descendant-or-self derived version of <typeparamref name="T"/>.</returns>
        public static T CreateDescendantOrSelfEntity<T>(Type derived, EntityDataAdapter adapter, params object[] entityConstructorArgs) where T : Entity
        {
            if (adapter == null) { throw new ArgumentNullException("adapter"); }
            if (entityConstructorArgs == null) { throw new ArgumentNullException("entityConstructorArgs"); }

            T validEntity = null;
            if (!TypeUtility.ContainsType(derived, typeof(BusinessEntity))) { return null; }

            List<Type> derivedTypes = new List<Type>(adapter.Settings.EnableDerivedEntityLookup ? TypeUtility.GetAncestorAndDescendantsOrSelfTypes(derived) : ConvertUtility.ToEnumerable(derived));
		    bool enableRowVerification = (derivedTypes.Count > 1);
            foreach (Type derivedType in derivedTypes)
            {
                #region Super Experimental Derived Type Lookup
                if (derivedType == typeof(BusinessEntity) || validEntity != null) { break; }
                if (derivedType.IsAbstract) { break; }
                SortedList<int, object> valuesForConstructor = new SortedList<int, object>();
                int sortOrder = 0;
                foreach (object arg in entityConstructorArgs)
                {
                    valuesForConstructor.Add(sortOrder, arg);
                    sortOrder++;
                }

                int index = 0;
                IEnumerable<ColumnAttribute> primaryKeyColumns = RuntimeEntityPrimaryKeyParser(derivedType);
                SortedList<int, ColumnAttribute> sortedPrimaryKeyColumns = new SortedList<int, ColumnAttribute>();
                foreach (ColumnAttribute primaryKeyColumn in primaryKeyColumns)
                {
                    sortedPrimaryKeyColumns.Add(primaryKeyColumn.CompositePrimaryKeyOrder == 0 ? index : primaryKeyColumn.CompositePrimaryKeyOrder, primaryKeyColumn);
                    index++;
                }

                if (valuesForConstructor.Count != sortedPrimaryKeyColumns.Count) { throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Unexpected amount of constructor arguments: {0}. Expected amount of arguments was: {1}.", valuesForConstructor.Count, sortedPrimaryKeyColumns.Count), "entityConstructorArgs"); }
                
                List<IDataParameter> parameters = new List<IDataParameter>();
                index = 0;
                List<string> fieldTypes = new List<string>();
                List<string> expectedFieldTypes = new List<string>();
                foreach (ColumnAttribute primaryKeyColumn in sortedPrimaryKeyColumns.Values)
                {
                    object fieldValue = valuesForConstructor[index];
                    Type fieldType = fieldValue.GetType();
                    Type expectedFieldType = DataManager.ParseDbType(primaryKeyColumn.DBType);
                    fieldTypes.Add(fieldType.Name);
                    expectedFieldTypes.Add(expectedFieldType.Name);
                    parameters.Add(adapter.GetDataParameter(primaryKeyColumn, fieldValue));
                    index++;
                }

                string fieldTypesResult = ConvertUtility.ToDelimitedString(fieldTypes);
                string expectedFieldTypesResult = ConvertUtility.ToDelimitedString(expectedFieldTypes);
                if (!fieldTypesResult.Equals(expectedFieldTypesResult, StringComparison.Ordinal)) { throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "One or more unexpected types detected in constructor arguments: {0}. Expected types was: {1}.", fieldTypesResult, expectedFieldTypesResult), "entityConstructorArgs"); }

                List<object> primaryKeyValues = new List<object>(parameters.Count);
                foreach (IDataParameter parameter in parameters) { primaryKeyValues.Add(parameter.Value); }
                RuntimeBusinessEntity runtime = new RuntimeBusinessEntity(derivedType, primaryKeyValues);
                ConstructorInfo constructor = runtime.GetConstructor();
                if (adapter.Manager == null) { throw new InvalidOperationException("Specified EntityDataAdapter has no associated DataManager."); }
                validEntity = ConstructRuntimeEntity<T>(runtime, constructor, derivedType, adapter, parameters.ToArray(), enableRowVerification);
                #endregion
            }
            return validEntity;
        }

        private static T ConstructRuntimeEntity<T>(RuntimeBusinessEntity runtime, ConstructorInfo constructor, Type derivedType, EntityDataAdapter adapter, IDataParameter[] parameters, bool enableRowVerification) where T : Entity
        {
            bool returnEntity = true;
            if (enableRowVerification)
            {
                DataManager manager = adapter.Manager.Clone();
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Suppress))
                {
                    DataMapper mapper = new DataMapper(derivedType, RuntimeEntityPrimaryKeyParser);
                    returnEntity = manager.ExecuteExists(adapter.GetDataMappedQuery(QueryType.Exists, mapper), parameters);
                    scope.Complete();
                }    
            }
            return returnEntity ? constructor.Invoke(runtime.ConstructorArguments) as T : default(T);
	    }

        private static IEnumerable<ColumnAttribute> RuntimeEntityPrimaryKeyParser(Type source)
	    {
            return MappingUtility.GetPrimaryKeyColumns(DataMapperUtility.ParseColumns(source));
	    }

        /// <summary>
        /// Infrastructure. Gets a collection of data mapped entity types inherited from <see cref="Entity"/>.
        /// </summary>
        /// <param name="entityType">Type of the <see cref="Entity"/>.</param>
        /// <returns>A collection of entity types that is decorated with the <see cref="DataSourceAttribute"/>.</returns>
	    public static IList<Type> GetDataMappedEntities(Type entityType)
	    {
	        return GetDataMappedEntities(entityType, typeof(Entity));
	    }

		/// <summary>
		/// Infrastructure. Gets a collection of data mapped entity types.
		/// </summary>
		/// <param name="entityType">Type of the <see cref="Entity"/>.</param>
		/// <param name="baseEntityType">Type of the base <see cref="Entity"/>.</param>
		/// <returns>A collection of entity types that is decorated with the <see cref="DataSourceAttribute"/>.</returns>
		public static IList<Type> GetDataMappedEntities(Type entityType, Type baseEntityType)
		{
			if (TypeUtility.ContainsType(entityType, baseEntityType))
			{
				IList<Type> dataMappedEntities = new List<Type>();
				Type currentType = entityType;
				while (currentType != baseEntityType)
				{
					DataSourceAttribute dataSource = (DataSourceAttribute)Attribute.GetCustomAttribute(currentType, typeof(DataSourceAttribute));
					if (dataSource != null)
					{
						dataMappedEntities.Add(currentType);
					}
					currentType = currentType.BaseType;
				}
				return new List<Type>(EnumerableUtility.Reverse(dataMappedEntities));
			}
			return new List<Type>();
		}
	}
}