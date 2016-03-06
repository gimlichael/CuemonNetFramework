using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text;
using Cuemon.Collections.Generic;
using Cuemon.Data.Entity.Mapping;
using Cuemon.Reflection;

namespace Cuemon.Data.Entity
{
    /// <summary>
    /// Parses and resolves data mapping of an <see cref="Entity"/>.
    /// </summary>
    public class EntityMapper
    {
        private readonly object _padLock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityMapper"/> class.
        /// </summary>
        /// <param name="entity">The entity to parse.</param>
        public EntityMapper(Entity entity) : this(entity, ValidateEntity(entity))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityMapper"/> class.
        /// </summary>
        /// <param name="entity">The entity to parse.</param>
        /// <param name="entityType">The specific type of the entity to parse.</param>
        public EntityMapper(Entity entity, Type entityType)
        {
            Validator.ThrowIfNull(entity, nameof(entity));
            Validator.ThrowIfNull(entityType, nameof(entityType));

            this.Source = entity;
            this.SourceType = entityType;
        }

        #region Properties
        /// <summary>
        /// Gets the <see cref="Entity"/> of this instance.
        /// </summary>
        /// <value>The <see cref="Entity"/> of this instance.</value>
        public Entity Source { get; private set; }

        /// <summary>
        /// Gets the specific <see cref="Type"/> of the <see cref="Entity"/> of this instance.
        /// </summary>
        /// <value>The specific <see cref="Type"/> of the <see cref="Entity"/> of this instance.</value>
        public Type SourceType { get; private set; }

        /// <summary>
        /// Gets the inheritance chain of the <see cref="SourceType"/>.
        /// </summary>
        /// <returns>A sequence of <see cref="Type"/> objects resolved from the <see cref="SourceType"/>.</returns>
        public IEnumerable<Type> GetInheritanceChain()
        {
            return BusinessEntityUtility.GetDataMappedEntities(this.SourceType);
        }

        /// <summary>
        /// Parses the delete statement of an <see cref="Entity"/>.
        /// </summary>
        /// <param name="deleter">The delegate that performs a delete operation on a data source.</param>
        public void ParseDeleteStatement(Act<IDataCommand, IDataParameter[]> deleter)
        {
            Validator.ThrowIfNull(deleter, nameof(deleter));
            foreach (Type chainedType in EnumerableUtility.Reverse(this.GetInheritanceChain()))
            {
                DataMapper mapper = new DataMapper(chainedType);
                IDataCommand command = this.Source.DataAdapter.GetDataMappedQuery(QueryType.Delete, mapper);
                IDataParameter[] parameters = this.GetDataParameters(chainedType, mapper.Columns);
                deleter(command, parameters);
            }
        }

        /// <summary>
        /// Parses the delete statement of a demoted <see cref="Entity"/>.
        /// </summary>
        /// <param name="demoter">The delegate that performs a delete operation on a data source.</param>
        /// <param name="newRank">The <see cref="Type"/> of the demoted <see cref="Source"/>.</param>
        public void ParseDemoteStatement(Act<IDataCommand, IDataParameter[]> demoter, Type newRank)
        {
            Validator.ThrowIfNull(demoter, nameof(demoter));
            foreach (Type chainedType in EnumerableUtility.Reverse(this.GetInheritanceChain()))
            {
                if (chainedType == newRank) { return; }
                DataMapper mapper = new DataMapper(chainedType);
                IDataCommand command = this.Source.DataAdapter.GetDataMappedQuery(QueryType.Delete, mapper);
                IDataParameter[] parameters = this.GetDataParameters(chainedType, mapper.Columns);
                demoter(command, parameters);
            }
        }

        /// <summary>
        /// Parses the insert statement of a promoted <see cref="Entity"/>.
        /// </summary>
        /// <param name="promoter">The function delegate that performs an insert operation on a data source.</param>
        /// <param name="newRank">The <paramref name="newRank"/> of the promoted <see cref="Source"/>.</param>
        public void ParsePromoteStatement(Doer<IDataCommand, IDataParameter[], object> promoter, Entity newRank)
        {
            Validator.ThrowIfNull(promoter, nameof(promoter));
            IDataParameter[] parameters = new IDataParameter[0];
            foreach (Type chainedType in this.GetInheritanceChain())
            {
                parameters = EnumerableConverter.ToArray(EnumerableUtility.Concat(parameters, this.GetDataParameters(chainedType, this.Source.GetDataMappedColumns(chainedType))));
            }

            Type newRankType = newRank.GetType();
            DataMapper mapper = new DataMapper(newRankType);
            parameters = EnumerableConverter.ToArray(EnumerableUtility.Distinct(EnumerableUtility.Concat(parameters, this.GetDataParametersForObjectRanking(newRank, mapper.Columns)), new DataParameterEqualityComparer()));

            IDataCommand command = this.Source.DataAdapter.GetDataMappedQuery(QueryType.Insert, mapper);
            promoter(command, parameters);
        }

        /// <summary>
        /// Parses the update statement of an <see cref="Entity"/>.
        /// </summary>
        /// <param name="updater">The delegate that performs an update operation on a data source.</param>
        public void ParseUpdateStatement(Act<IDataCommand, IDataParameter[]> updater)
        {
            Validator.ThrowIfNull(updater, nameof(updater));
            foreach (Type chainedType in this.GetInheritanceChain())
            {
                DataMapper mapper = new DataMapper(chainedType);
                bool primaryKeysOnly = true;
                foreach (ColumnAttribute column in mapper.Columns) { primaryKeysOnly &= column.IsPrimaryKey; }
                if (primaryKeysOnly) { continue; }

                IDataCommand command = this.Source.DataAdapter.GetDataMappedQuery(QueryType.Update, mapper);
                IDataParameter[] parameters = this.GetDataParameters(chainedType, mapper.Columns);
                updater(command, parameters);
            }
        }

        /// <summary>
        /// Parses the select statement for looking up an <see cref="Entity"/>.
        /// </summary>
        /// <param name="predicateSelector">The function delegate that performs a select operation on a data source.</param>
        public bool ParseSelectExistsStatement(Doer<IDataCommand, IDataParameter[], bool> predicateSelector)
        {
            Validator.ThrowIfNull(predicateSelector, nameof(predicateSelector));
            DataMapper mapper = new DataMapper(this.SourceType);
            IDataParameter[] parameters = this.GetDataParameters(this.SourceType, mapper.Columns);
            IDataCommand command = this.Source.DataAdapter.GetDataMappedQuery(QueryType.Exists, mapper);
            return predicateSelector(command, parameters);
        }

        /// <summary>
        /// Parses the insert statement of an <see cref="Entity"/>.
        /// </summary>
        /// <param name="inserter">The function delegate that performs an insert operation on a data source.</param>
        public void ParseInsertStatement(Doer<IDataCommand, QueryInsertAction, IDataParameter[], object> inserter)
        {
            Validator.ThrowIfNull(inserter, nameof(inserter));
            IDictionary<Type, ColumnAttribute> persistedTypes = new Dictionary<Type, ColumnAttribute>();
            foreach (Type chainedType in this.GetInheritanceChain())
            {
                DataMapper mapper = new DataMapper(chainedType);
                foreach (Type persistedType in persistedTypes.Keys)
                {
                    foreach (ColumnAttribute association in mapper.Columns)
                    {
                        if (association.GetType() != typeof(AssociationAttribute)) { continue; }
                        AssociationAttribute associationAsAssociationAttribute = (association as AssociationAttribute);
                        if (associationAsAssociationAttribute != null)
                        {
                            if (persistedType == associationAsAssociationAttribute.AssociatedStorageType)
                            {
                                this.SetStorageValue(chainedType, associationAsAssociationAttribute.ResolveStorage(),
                                    this.GetStorageValue(persistedType, associationAsAssociationAttribute.AssociatedStorage));
                            }
                        }
                    }
                }

                DataSourceAttribute dataSourceAttribute = ReflectionUtility.GetAttribute<DataSourceAttribute>(chainedType, true);
                bool executeInsertQuery = !dataSourceAttribute.EnableRowVerification || !this.Source.DataAdapter.Exists(chainedType); // do check exists - if not exists, execute insert
                if (executeInsertQuery)
                {
                    IDataParameter[] parameters = this.GetDataParameters(chainedType, mapper.Columns);
                    foreach (ColumnAttribute column in mapper.Columns)
                    {
                        if (column.IsPrimaryKey)
                        {
                            IDataCommand command = null;
                            if (column.IsDBGenerated)
                            {
                                command = this.Source.DataAdapter.GetDataMappedQuery(QueryType.Insert, mapper);
                                object identity = inserter(command, ResolveQueryOperation(column), parameters);
                                this.SetStorageValue(chainedType, column.ResolveStorage(), identity);
                                persistedTypes.Add(chainedType, column);
                                break;
                            }
                            command = this.Source.DataAdapter.GetDataMappedQuery(QueryType.Insert, mapper);
                            inserter(command, QueryInsertAction.Void, parameters);
                            persistedTypes.Add(chainedType, column);
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Parses the select statement of an <see cref="Entity"/>.
        /// </summary>
        /// <param name="selector">The function delegate that performs a select operation on a data source.</param>
        public void ParseSelectStatement(Doer<IDataCommand, IDataParameter[], IDataReader> selector)
        {
            Validator.ThrowIfNull(selector, nameof(selector));
            foreach (Type chainedType in this.GetInheritanceChain())
            {
                DataMapper mapper = new DataMapper(chainedType);
                IDataCommand command = this.Source.DataAdapter.GetDataMappedQuery(QueryType.Select, mapper);
                IDataParameter[] parameters = this.GetDataParameters(chainedType, mapper.Columns);
                using (IDataReader reader = selector(command, parameters))
                {
                    if (reader.Read())
                    {
                        this.Source.HasValue = true;
                        InvokeBusinessEntityFromRepository(DictionaryConverter.FromEnumerable<string, object>(DataManager.GetReaderColumns(reader)), this.Source, chainedType, mapper.Columns);
                    }
                    else if (this.Source.DataAdapter.Settings.EnableThrowOnNoRowsReturned)
                    {
                        DataAdapterException dataAdapterException = new DataAdapterException(String.Format(CultureInfo.InvariantCulture, "No rows returned from the data mapped values of Entity:{0} using EntityDataAdapter:{1}.", chainedType, this.Source.DataAdapter.GetType()));
                        dataAdapterException.Data.Add("commandText", command.Text);
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            dataAdapterException.Data.Add(string.Format(CultureInfo.InvariantCulture, "parameters[{0}]", i), string.Format(CultureInfo.InvariantCulture, "{0} {1}={2}", parameters[i].DbType, parameters[i].ParameterName, parameters[i].Value));
                        }
                        throw dataAdapterException;
                    }
                }
            }
            if (this.Source.DataAdapter.Settings.EnableConcurrencyCheck) { this.PerformConcurrencyCheck(); }
        }

        /// <summary>
        /// Parses the select statement for the sequence of an <see cref="Entity" />.
        /// </summary>
        /// <typeparam name="T">The type of the <see cref="Entity"/> in the sequence.</typeparam>
        /// <param name="manySelector">The function delegate that performs a select operation on a data source.</param>
        public void ParseSelectManyStatement<T>(Doer<IDataCommand, IDataParameter[], IDataReader> manySelector) where T : Entity
        {
            Validator.ThrowIfNull(manySelector, nameof(manySelector));
            Type entitiesEntityType = typeof(T);
            foreach (Type chainedType in this.GetInheritanceChain())
            {
                DataMapper mapper = new DataMapper(chainedType);
                DataMapper entityMapper = new DataMapper(chainedType, this.ParseBusinessEntityCollectionColumnAttributes);
                IEnumerable<ColumnAttribute> columns = this.Source.DataAdapter.Settings.EnableBulkLoad ? entityMapper.Columns : mapper.Columns;

                IDataCommand command = this.Source.DataAdapter.GetDataMappedQuery(QueryType.Select, this.Source.DataAdapter.Settings.EnableBulkLoad ? entityMapper : mapper);
                IDataParameter[] parameters = this.GetDataParameters(chainedType, columns);

                using (IDataReader reader = manySelector(command, parameters))
                {
                    lock (_padLock)
                    {
                        IEnumerable<ColumnAttribute> primaryKeyColumns = MappingUtility.GetPrimaryKeyColumns(columns);
                        if (primaryKeyColumns == null) { throw new ArgumentException("Unable to match any primary keys from the specified column attributes."); }
                        if (this.Source.DataAdapter.Settings.EnableBulkLoad)
                        {
                            IList<IDictionary<string, object>> dataset = new List<IDictionary<string, object>>();
                            while (reader.Read())
                            {
                                this.Source.HasValue = true;
                                IDictionary<string, object> readerColumns = DataManager.GetReaderColumnsAsDictionary(reader);
                                dataset.Add(readerColumns);
                            }

                            for (int i = 0; i < dataset.Count; i++)
                            {
                                IDictionary<string, object> entityDataset = dataset[i];
                                Entity resolvedEntity = OpenListBulkedWorkItem.InvokeIncognito(this.Source.DataAdapter, entitiesEntityType, entityDataset, primaryKeyColumns, columns);
                                this.InvokeEntitiesAdd(chainedType, entitiesEntityType, resolvedEntity);
                            }
                        }
                        else
                        {
                            IEnumerable<KeyValuePair<int, SortedList<int, object>>> primaryKeyValues = Parse(reader, primaryKeyColumns, entitiesEntityType, "command");
                            foreach (KeyValuePair<int, SortedList<int, object>> item in primaryKeyValues)
                            {
                                this.Source.HasValue = true;
                                Entity resolvedEntity = BusinessEntityUtility.CreateDescendantOrSelfEntity<Entity>(entitiesEntityType, this.Source.DataAdapter, EnumerableConverter.ToArray(item.Value.Values));
                                this.InvokeEntitiesAdd(chainedType, entitiesEntityType, resolvedEntity);
                            }
                        }
                    }
                }
            }
        }

        private void InvokeEntitiesAdd(Type chainedType, Type entitiesEntityType, Entity resolvedEntity)
        {
            MethodInfo entitiesMethod = chainedType.GetMethod("Add", new[] { entitiesEntityType });
            entitiesMethod.Invoke(this.Source, new object[] { resolvedEntity });
        }

        internal bool HasUniqueIndexValidation(Type entityType, ColumnAttribute column, object uniqueValue, Doer<IDataCommand, IDataParameter[], bool> select)
        {
            string tableName = ReflectionUtility.GetAttribute<TableAttribute>(entityType, true).Name;
            bool enableTableAndColumnEncapsulation = ReflectionUtility.GetAttribute<DataSourceAttribute>(entityType, true).EnableTableAndColumnEncapsulation;
            List<IDataParameter> parameters = new List<IDataParameter>();
            parameters.Add(this.Source.DataAdapter.GetDataParameter(column, uniqueValue));
            IEnumerable<ColumnAttribute> entityColumns = this.Source.GetDataMappedColumns(entityType);
            ColumnAttribute[] primaryKeyEntityColumns = EnumerableConverter.ToArray(MappingUtility.GetPrimaryKeyColumns(entityColumns));

            StringBuilder whereClause = new StringBuilder("AND NOT (");
            byte i = 1;
            foreach (ColumnAttribute primaryKey in primaryKeyEntityColumns)
            {
                whereClause.AppendFormat(enableTableAndColumnEncapsulation ? " [{0}]={1}" : " {0}={1}", primaryKey.Name, primaryKey.ParameterName);
                if (i < primaryKeyEntityColumns.Length) { whereClause.Append(" AND"); }
                parameters.Add(this.Source.DataAdapter.GetDataParameter(primaryKey, this.GetStorageValue(primaryKey.StorageType ?? entityType, primaryKey.ResolveStorage())));
                i++;
            }
            whereClause.Append(")");

            return select(new DataCommand(string.Format(CultureInfo.InvariantCulture, "SELECT 1 FROM [{0}] WHERE [{1}] = {2} {3}",
                tableName,
                column.Name,
                parameters[0].ParameterName,
                this.Source.IsNew ? "" : whereClause.ToString())), parameters.ToArray());
        }
        #endregion

        #region Methods
        internal static IDictionary<Type, IList<FieldInfo>> GetDataMappedEntitiesFields(Entity entity)
        {
            Dictionary<Type, IList<FieldInfo>> tempEntityFields = new Dictionary<Type, IList<FieldInfo>>();
            foreach (Type dataMappedEntity in BusinessEntityUtility.GetDataMappedEntities(entity.GetType()))
            {
                IList<FieldInfo> entityFields = new List<FieldInfo>();
                foreach (FieldInfo field in dataMappedEntity.GetFields(ReflectionUtility.BindingInstancePublicAndPrivateNoneInheritedIncludeStatic))
                {
                    entityFields.Add(field);
                }
                tempEntityFields.Add(dataMappedEntity, entityFields);
            }
            return tempEntityFields;
        }

        /// <summary>
        /// Gets the data parameters necessary for data mapping when either promoting or demoting an object.
        /// </summary>
        /// <param name="entity">The entity to fetch private field values from.</param>
        /// <param name="columns">The data mapping columns of the entity type.</param>
        /// <returns>An array of <see cref="IDataParameter"/> compatible objects.</returns>
        protected virtual IDataParameter[] GetDataParametersForObjectRanking(Entity entity, IEnumerable<ColumnAttribute> columns)
        {
            if (columns == null) { throw new ArgumentNullException(nameof(columns)); }
            if (entity == null) { throw new ArgumentNullException(nameof(entity)); }
            List<IDataParameter> parameters = new List<IDataParameter>();
            Type entityType = entity.GetType();
            foreach (ColumnAttribute column in columns)
            {
                if ((column.StorageType == null || column.StorageType == entityType) && !column.IsDBGenerated)
                {
                    IDataParameter parameter = this.Source.DataAdapter.GetDataParameter(column, this.GetStorageValue(entity, column.StorageType ?? entityType, column.ResolveStorage()));
                    parameters.Add(parameter);
                }
            }
            return parameters.ToArray();
        }

        private static IEnumerable<KeyValuePair<int, SortedList<int, object>>> Parse(IDataReader reader, IEnumerable<ColumnAttribute> primaryKeyColumns, Type entitiesEntityType, string argName)
        {
            IDictionary<int, SortedList<int, object>> primaryKeyValues = new Dictionary<int, SortedList<int, object>>();
            IEnumerable<FieldInfo> entitiesEntityFields = new List<FieldInfo>(ReflectionUtility.GetFields(entitiesEntityType));
            int index = 0;
            while (reader.Read())
            {
                int primaryKeyIndex = 0;
                foreach (ColumnAttribute column in primaryKeyColumns)
                {
                    object matchingColumn = DictionaryUtility.FirstMatchOrDefault(DictionaryConverter.FromEnumerable(DataManager.GetReaderColumns(reader)), column.Name, column.NameAlias);
                    if (matchingColumn == null) { throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Unable to find a matching column from {0} (or alias {1}) mapped to storage field {2}.", column.Name ?? "null", column.NameAlias ?? "null", column.ResolveStorage() ?? "null"), argName); }
                    if (!primaryKeyValues.ContainsKey(index))
                    {
                        primaryKeyValues.Add(index, new SortedList<int, object>());
                    }

                    #region Find and change type if necessary (such as enums etc.)
                    Type matchingColumnFieldType = matchingColumn.GetType();
                    int matchingPrimaryKeyIndex = column.CompositePrimaryKeyOrder == 0 ? primaryKeyIndex : column.CompositePrimaryKeyOrder;
                    string fieldName = column.ResolveStorage();
                    IReadOnlyCollection<PropertyInfo> properties;
                    MappingUtility.ParseStorageField(ref fieldName, entitiesEntityType, out properties);
                    foreach (FieldInfo field in entitiesEntityFields)
                    {
                        if (field.Name.Equals(fieldName, StringComparison.OrdinalIgnoreCase))
                        {
                            matchingColumnFieldType = properties.Count == 0 ? column.StorageType ?? field.FieldType : column.StorageType ?? EnumerableUtility.LastOrDefault(properties).PropertyType;
                            matchingPrimaryKeyIndex = column.CompositePrimaryKeyOrder == 0 ? primaryKeyIndex : column.CompositePrimaryKeyOrder;
                            break;
                        }
                    }
                    primaryKeyValues[index].Add(matchingPrimaryKeyIndex, ObjectConverter.ChangeType(matchingColumn, matchingColumnFieldType));
                    #endregion
                    primaryKeyIndex++;
                }
                index++;
            }
            return primaryKeyValues;
        }

        private IEnumerable<ColumnAttribute> ParseBusinessEntityCollectionColumnAttributes(Type entityType)
        {
            Type entitiesEntityType = ParseEntityTypeForOpenList(this.Source, entityType);
            IDictionary<PropertyInfo, ColumnAttribute[]> tempDictionary = ReflectionUtility.GetPropertyAttributeDecorations<ColumnAttribute>(entitiesEntityType);
            List<ColumnAttribute> resolvedEntityColumns = new List<ColumnAttribute>();
            foreach (ColumnAttribute[] columns in tempDictionary.Values)
            {
                foreach (ColumnAttribute column in columns) { resolvedEntityColumns.Add(column); }
            }
            return resolvedEntityColumns;
        }

        /// <summary>
        /// Parse and resolve the <see cref="Type"/> to fill a <see cref="BusinessEntityCollection{T}"/> with.
        /// </summary>
        /// <param name="entity">An <see cref="Entity"/> instance to invoke.</param>
        /// <param name="entitiesType">The type of a <see cref="BusinessEntityCollection{T}"/>.</param>
        /// <returns>The <see cref="Type"/> to fill a <see cref="BusinessEntityCollection{T}"/> with.</returns>
        protected static Type ParseEntityTypeForOpenList(Entity entity, Type entitiesType)
        {
            if (entity == null) { throw new ArgumentNullException(nameof(entity)); }
            if (entitiesType == null) { throw new ArgumentNullException(nameof(entitiesType)); }
            MethodInfo entitiesMethod = entitiesType.GetMethod("GetEntityType");
            return (Type)entitiesMethod.Invoke(entity, null); // get the generic element from the entities object, eg.: BusinessEntityCollection<T>, where T equals the element type ;)
        }

        internal static void InvokeBusinessEntityFromRepository(IDictionary<string, object> columns, Entity entity, Type entityType, IEnumerable<ColumnAttribute> attributes)
        {
            foreach (ColumnAttribute column in attributes)
            {
                object matchingColumn = DictionaryUtility.FirstMatchOrDefault(columns, column.Name, column.NameAlias);
                if (matchingColumn == null) { throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "Unable to find a matching column from {0} (or alias {1}) mapped to storage field {2}.", column.Name ?? "null", column.NameAlias ?? "null", column.ResolveStorage() ?? "null"), nameof(columns)); }
                object value = matchingColumn == DBNull.Value ? null : matchingColumn;
                SetStorageValue(entity, entityType, column.ResolveStorage(), value);
            }
        }

        private static QueryInsertAction ResolveQueryOperation(ColumnAttribute column)
        {
            switch (column.DBType)
            {
                case DbType.Byte:
                case DbType.Int16:
                case DbType.UInt16:
                case DbType.Int32:
                    return QueryInsertAction.IdentityInt32;
                case DbType.UInt32:
                case DbType.Int64:
                    return QueryInsertAction.IdentityInt64;
                case DbType.Double:
                case DbType.Decimal:
                    return QueryInsertAction.IdentityDecimal;
                default:
                    return QueryInsertAction.AffectedRows;
            }
        }

        private static Type ValidateEntity(Entity entity)
        {
            Validator.ThrowIfNull(entity, nameof(entity));
            return entity.GetType();
        }

        /// <summary>
        /// Gets the data parameters necessary for data mapping.
        /// </summary>
        /// <param name="entityType">The entity type to fetch private field values from.</param>
        /// <param name="columns">The data mapping columns of the entity type.</param>
        /// <returns>An array of <see cref="IDataParameter"/> compatible objects.</returns>
        protected IDataParameter[] GetDataParameters(Type entityType, IEnumerable<ColumnAttribute> columns)
        {
            Validator.ThrowIfNull(entityType, nameof(entityType));
            Validator.ThrowIfNull(columns, nameof(columns));
            List<IDataParameter> parameters = new List<IDataParameter>();
            foreach (ColumnAttribute column in columns)
            {
                IDataParameter parameter = this.Source.DataAdapter.GetDataParameter(column, this.GetStorageValue(column.StorageType ?? entityType, column.ResolveStorage()));
                parameters.Add(parameter);
            }
            return parameters.ToArray();
        }

        /// <summary>
        /// Sets the given value in a private storage field in the provided class type.
        /// </summary>
        /// <param name="entityType">The class that holds the private storage field.</param>
        /// <param name="fieldName">Name of the private storage field.</param>
        /// <param name="value">The value to assign the private storage field.</param>
        protected void SetStorageValue(Type entityType, string fieldName, object value)
        {
            SetStorageValue(this.Source, entityType, fieldName, value);
        }

        /// <summary>
        /// Sets the given value in a private storage field in the provided class type.
        /// </summary>
        /// <param name="entity">The <see cref="Entity"/> instance to invoke from.</param>
        /// <param name="entityType">The class that holds the private storage field.</param>
        /// <param name="fieldName">Name of the private storage field.</param>
        /// <param name="value">The value to assign the private storage field.</param>
        internal static void SetStorageValue(Entity entity, Type entityType, string fieldName, object value)
        {
            foreach (KeyValuePair<Type, IList<FieldInfo>> keyValuePair in GetDataMappedEntitiesFields(entity))
            {
                if (keyValuePair.Key == entityType)
                {
                    //TODO: CLEAN UP AND REFACTOR THIS METHOD
                    string originalFieldName = fieldName;
                    string[] propertyNames;
                    MappingUtility.ParseStorageField(ref fieldName, out propertyNames);
                    foreach (FieldInfo fieldInfo in keyValuePair.Value)
                    {
                        if (fieldInfo.Name == fieldName)
                        {
                            IReadOnlyCollection<PropertyInfo> properties;
                            MappingUtility.ParseStorageField(ref originalFieldName, entityType, ReflectionUtility.BindingInstancePublicAndPrivateNoneInheritedIncludeStatic, out properties);
                            if (properties.Count == 0)
                            {
                                Type fieldType = fieldInfo.FieldType;
                                fieldInfo.SetValue(entity, value == null ? null : ObjectConverter.ChangeType(value, fieldType));
                            }
                            else
                            {
                                PropertyInfo property = EnumerableUtility.LastOrDefault(properties);
                                object propertyContainer = fieldInfo.GetValue(entity);
                                if (property != null && property.CanWrite && propertyContainer != null)
                                {
                                    MappingUtility.SetStorageFieldPropertyValue(entity, fieldInfo, properties, property.PropertyType, value); // experimental
                                }
                                else
                                {
                                    if (value == null)
                                    {
                                        fieldInfo.SetValue(entity, null);
                                    }
                                    else
                                    {
                                        Type fieldInfoType = fieldInfo.FieldType;
                                        BusinessEntity newEntity = BusinessEntityUtility.CreateDescendantOrSelfEntity<BusinessEntity>(fieldInfoType, entity.DataAdapter, new object[] { value });
                                        if (newEntity != null) { fieldInfo.SetValue(entity, newEntity); }
                                    }
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Gets the storage fields from the specified <paramref name="entityType"/>.
        /// </summary>
        /// <param name="entityType">The <see cref="Type"/> that holds the private storage fields.</param>
        /// <returns>A sequence of <see cref="FieldInfo"/> objects associated with the specified <paramref name="entityType"/>.</returns>
        protected IEnumerable<FieldInfo> GetStorageFields(Type entityType)
        {
            return this.GetStorageFields(this.Source, entityType);
        }

        /// <summary>
        /// Gets the storage fields from the specified <paramref name="entityType"/>.
        /// </summary>
        /// <param name="entity">An instance from where to access the private storage fields from.</param>
        /// <param name="entityType">The <see cref="Type"/> that holds the private storage fields.</param>
        /// <returns>A sequence of <see cref="FieldInfo"/> objects associated with the specified <paramref name="entityType"/>.</returns>
        protected IEnumerable<FieldInfo> GetStorageFields(Entity entity, Type entityType)
        {
            foreach (KeyValuePair<Type, IList<FieldInfo>> keyValuePair in GetDataMappedEntitiesFields(entity))
            {
                if (keyValuePair.Key == entityType)
                {
                    foreach (FieldInfo fieldInfo in keyValuePair.Value) { yield return fieldInfo; }
                    break;
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="Type"/> of a private storage field from the provided class type.
        /// </summary>
        /// <param name="entityType">The class that holds the private storage field.</param>
        /// <param name="fieldName">Name of the private storage field.</param>
        /// <returns>The <see cref="Type"/> of the private storage field.</returns>
        protected Type GetStorageType(Type entityType, string fieldName)
        {
            return this.GetStorageType(this.Source, entityType, fieldName);
        }

        /// <summary>
        /// Gets the <see cref="Type"/> of a private storage field from the provided class type.
        /// </summary>
        /// <param name="entity">An instance from where to access the private storage fields from.</param>
        /// <param name="entityType">The class that holds the private storage field.</param>
        /// <param name="fieldName">Name of the private storage field.</param>
        /// <returns>The <see cref="Type"/> of the private storage field.</returns>
        protected virtual Type GetStorageType(Entity entity, Type entityType, string fieldName)
        {
            IEnumerable<FieldInfo> fields = this.GetStorageFields(entity, entityType);
            foreach (FieldInfo field in fields)
            {
                if (field.Name == fieldName) { return field.FieldType; }
            }
            return null;
        }

        /// <summary>
        /// Gets the value of a private storage field from the provided class type.
        /// </summary>
        /// <param name="entityType">The class that holds the private storage field.</param>
        /// <param name="fieldName">Name of the private storage field.</param>
        /// <returns>The value of the private storage field.</returns>
        protected object GetStorageValue(Type entityType, string fieldName)
        {
            return this.GetStorageValue(this.Source, entityType, fieldName);
        }

        /// <summary>
        /// Gets the value of a private storage field from the provided class type.
        /// </summary>
        /// <param name="entity">An instance from where to access the private storage fields from.</param>
        /// <param name="entityType">The class that holds the private storage field.</param>
        /// <param name="fieldName">Name of the private storage field.</param>
        /// <returns>The value of the private storage field.</returns>
        protected virtual object GetStorageValue(Entity entity, Type entityType, string fieldName)
        {
            IEnumerable<FieldInfo> fields = this.GetStorageFields(entity, entityType);
            string[] propertyNames;
            string originalFieldName = fieldName;
            MappingUtility.ParseStorageField(ref fieldName, out propertyNames);
            foreach (FieldInfo field in fields)
            {
                if (field.Name == fieldName)
                {
                    IReadOnlyCollection<PropertyInfo> properties;
                    MappingUtility.ParseStorageField(ref originalFieldName, entityType, ReflectionUtility.BindingInstancePublicAndPrivateNoneInheritedIncludeStatic, out properties);
                    return MappingUtility.ParseStorageFieldValue(entity, field, properties);
                }
            }
            return null;
        }

        private void PerformConcurrencyCheck()
        {
            foreach (Type chainedType in this.GetInheritanceChain())
            {
                foreach (KeyValuePair<Type, AssociationAttribute[]> entityAssociationValuePair in MappingUtility.GetDataMappedEntitiesAssociations(this.Source))
                {
                    foreach (AssociationAttribute association in entityAssociationValuePair.Value)
                    {
                        if (association.AssociatedStorageType == chainedType) // we have a type match - lookup value
                        {
                            if (this.GetStorageValue(chainedType, association.AssociatedStorage).ToString() != this.GetStorageValue(entityAssociationValuePair.Key, association.ResolveStorage()).ToString())
                            {
                                throw new DataAdapterException(string.Format(CultureInfo.InvariantCulture, "Concurrency check failed on field {0}.{1} associated with field {2}.{3}!",
                                    chainedType,
                                    association.AssociatedStorage,
                                    entityAssociationValuePair.Key,
                                    association.ResolveStorage()));
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}
