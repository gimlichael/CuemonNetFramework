using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Cuemon.Collections.Generic;
using Cuemon.Data.Entity.Mapping;
using Cuemon.Reflection;

namespace Cuemon.Data.Entity
{
    internal static class OpenListBulkedWorkItem
    {
        internal static Entity InvokeIncognito(EntityDataAdapter adapter, Type entitiesEntityType, IDictionary<string, object> entityDataset, IEnumerable<ColumnAttribute> primaryKeyColumns, IEnumerable<ColumnAttribute> entityColumns)
        {
            IEnumerable<FieldInfo> fields = ReflectionUtility.GetFields(entitiesEntityType);
            List<object> constructorParams = new List<object>();
            foreach (ColumnAttribute primaryKeyColumn in primaryKeyColumns)
            {
                object matchingPrimaryKeyColumn = DictionaryUtility.FirstMatchOrDefault(entityDataset, primaryKeyColumn.Name, primaryKeyColumn.NameAlias);
                if (matchingPrimaryKeyColumn == null) { throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Unable to find a matching column from {0} (or alias {1}) mapped to storage field {2}.", primaryKeyColumn.Name ?? "null", primaryKeyColumn.NameAlias ?? "null", primaryKeyColumn.ResolveStorage() ?? "null")); }

                #region Find and change type if necessary (such as enums etc.)
                Type matchingColumnFieldType = matchingPrimaryKeyColumn.GetType();
                foreach (FieldInfo field in fields)
                {
                    if (field.Name.Equals(primaryKeyColumn.ResolveStorage(), StringComparison.OrdinalIgnoreCase))
                    {
                        matchingColumnFieldType = primaryKeyColumn.StorageType ?? field.FieldType;
                        break;
                    }
                }
                constructorParams.Add(ObjectConverter.ChangeType(matchingPrimaryKeyColumn, matchingColumnFieldType));
                #endregion
            }

            Entity newEntity = BusinessEntityUtility.CreateDescendantOrSelfEntity<Entity>(entitiesEntityType, adapter, constructorParams.ToArray());
            if (newEntity != null)
            {
                PropertyInfo hasLoaded = ReflectionUtility.GetProperty(newEntity.GetType().BaseType, "HasLoaded", typeof(bool), null, ReflectionUtility.BindingInstancePublicAndPrivate);
                PropertyInfo isNew = ReflectionUtility.GetProperty(newEntity.GetType(), "IsNew", typeof(bool), null, ReflectionUtility.BindingInstancePublicAndPrivate);
                PropertyInfo hasInitialized = ReflectionUtility.GetProperty(newEntity.GetType().BaseType, "HasInitialized", typeof(bool), null, ReflectionUtility.BindingInstancePublicAndPrivate);
                PropertyInfo isInitializing = ReflectionUtility.GetProperty(newEntity.GetType().BaseType, "IsInitializing", typeof(bool), null, ReflectionUtility.BindingInstancePublicAndPrivate);
                if (isInitializing != null)
                {
                    isInitializing.SetValue(newEntity, true, null);
                }
                if (hasInitialized != null)
                {
                    hasInitialized.SetValue(newEntity, true, null);
                }
                if (hasLoaded != null)
                {
                    hasLoaded.SetValue(newEntity, true, null);
                }
                if (isNew != null)
                {
                    isNew.SetValue(newEntity, false, null);
                }
            }
            EntityMapper.InvokeBusinessEntityFromRepository(entityDataset, newEntity, entitiesEntityType, entityColumns);
            return newEntity;
        }
    }
}
