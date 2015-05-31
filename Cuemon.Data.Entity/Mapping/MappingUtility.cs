using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Cuemon.Collections.Generic;
using Cuemon.Reflection;

namespace Cuemon.Data.Entity.Mapping
{
	/// <summary>
	/// This utility class is designed to make data mapping related operations easier to work with.
	/// </summary>
	public static class MappingUtility
	{
		/// <summary>
		/// A predicate for excluding columns having a DB-generated value definition.
		/// </summary>
		/// <param name="column">The column to check upon.</param>
		/// <returns>true if the column is not marked as a DB-generated value; otherwise false.</returns>
		public static bool ExcludeOnlyDbGeneratedColumns(ColumnAttribute column)
		{
			if (column == null) throw new ArgumentNullException("column");
			if (!column.IsDBGenerated) { return true; }
			return false;
		}

		/// <summary>
		/// A predicate for excluding columns having a primary key definition.
		/// </summary>
		/// <param name="column">The column to check upon.</param>
		/// <returns>true if the column is not marked as a primary key; otherwise false.</returns>
		public static bool ExcludeOnlyPrimaryKeyColumns(ColumnAttribute column)
		{
			if (!IncludeOnlyPrimaryKeyColumns(column))
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// A predicate for including only columns having a primary key definition.
		/// </summary>
		/// <param name="column">The column to check upon.</param>
		/// <returns>true if the column is marked as a primary key; otherwise false.</returns>
		public static bool IncludeOnlyPrimaryKeyColumns(ColumnAttribute column)
		{
			if (column == null) { throw new ArgumentNullException("column"); }
			if (column.IsPrimaryKey) { return true; }
			return false;
		}

		/// <summary>
		/// A predicate for including only columns having a foreign key definition.
		/// </summary>
		/// <param name="column">The column to check upon.</param>
		/// <returns>true if the column is marked as a foreign key; otherwise false.</returns>
		public static bool IncludeOnlyForeignKeyColumns(ColumnAttribute column)
		{
			if (column == null) { throw new ArgumentNullException("column"); }
			AssociationAttribute associationColumn = column as AssociationAttribute;
			if (associationColumn != null) { return associationColumn.IsForeignKey; }
			return false;
		}

        /// <summary>
        /// Infrastructure. Gets the data mapped entities associations from the specified <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">The entity to traverse for <see cref="AssociationAttribute"/> decorations.</param>
        /// <returns>A dictionary with the data mapped entities associations.</returns>
        public static IDictionary<Type, AssociationAttribute[]> GetDataMappedEntitiesAssociations(Entity entity)
	    {
            if (entity == null) { throw new ArgumentNullException("entity"); }

            Type entityType = entity.GetType();
            Dictionary<Type, AssociationAttribute[]> dataMappedEntitiesAssociations = new Dictionary<Type, AssociationAttribute[]>();
            List<AssociationAttribute> associations = new List<AssociationAttribute>();
	        foreach (Type chainedType in BusinessEntityUtility.GetDataMappedEntities(entityType))
	        {
	            foreach (ColumnAttribute column in entity.GetDataMappedColumns(chainedType))
	            {
	                AssociationAttribute associationColumn = column as AssociationAttribute;
                    if (associationColumn == null) { continue; }
                    associations.Add(associationColumn);
	            }
                dataMappedEntitiesAssociations.Add(chainedType, associations.ToArray());
                associations.Clear();
	        }
            return dataMappedEntitiesAssociations;
	    }

		/// <summary>
		/// Infrastructure. Gets the data mapped entities associations.
		/// </summary>
		/// <param name="dataMappedEntitiesColumns">The data mapped entities columns to search for associations.</param>
		/// <returns>A dictionary with the data mapped entities associations.</returns>
		public static IDictionary<Type, AssociationAttribute[]> GetDataMappedEntitiesAssociations(IDictionary<Type, ColumnAttribute[]> dataMappedEntitiesColumns)
		{
			if (dataMappedEntitiesColumns == null) throw new ArgumentNullException("dataMappedEntitiesColumns");
			Dictionary<Type, AssociationAttribute[]> dataMappedEntitiesAssociations = new Dictionary<Type, AssociationAttribute[]>();
			List<AssociationAttribute> associations = new List<AssociationAttribute>();
			foreach (KeyValuePair<Type, ColumnAttribute[]> entityValuePair in dataMappedEntitiesColumns)
			{
				foreach (ColumnAttribute column in entityValuePair.Value)
				{
					if (column.GetType() != typeof(AssociationAttribute)) { continue; }
					associations.Add((AssociationAttribute)column);
				}
				dataMappedEntitiesAssociations.Add(entityValuePair.Key, associations.ToArray());
				associations.Clear();
			}
			return dataMappedEntitiesAssociations;
		}

		/// <summary>
		/// Infrastructure. This method will parse the provided field name and resolve possible property names (separated by dots, eg. _myField.MyProperty) as well as the actual field name.
		/// </summary>
		/// <param name="fieldName">The resolved field name from the original value of this parameter.</param>
		/// <param name="propertyNames">The resolved property names from the field name parameter; otherwise empty.</param>
		/// <returns>
		/// 	<c>true</c> if property names are assumed part of the field name; otherwise, <c>false</c>.
		/// </returns>
        [Obsolete("This method has been deprecated. Please use ParseStorageField(string fieldName, Type storageType, out IReadOnlyCollection<PropertyInfo> properties) instead.")]
        public static bool ParseStorageField(ref string fieldName, out string[] propertyNames)
		{
			if (fieldName == null) { throw new ArgumentNullException("fieldName"); }
			int indexOfDot = fieldName.IndexOf(".", StringComparison.OrdinalIgnoreCase); // performance friendly
			bool storageAsProperty = (indexOfDot > 0);
			if (storageAsProperty) // handle private storage field objects with properties
			{
				List<string> filteredPropertyNames = new List<string>();
				propertyNames = fieldName.Split('.');
				for (int i = 0; i < propertyNames.Length; i++)
				{
					if (i == 0) { continue; }
					filteredPropertyNames.Add(propertyNames[i]);
				}
				propertyNames = filteredPropertyNames.ToArray();
				fieldName = fieldName.Substring(0, indexOfDot);
			}
			else
			{
				propertyNames = new string[0];
			}
			return storageAsProperty;
		}

        /// <summary>
        /// Infrastructure. This method will parse the result set of <see cref="ParseStorageField(ref string, Type, out IReadOnlyCollection{PropertyInfo})"/> and return it's associated value.
        /// </summary>
        /// <param name="storage">An instance of the storage where the <paramref name="field"/> is located.</param>
        /// <param name="field">The field to retrieve a value from.</param>
        /// <param name="properties">A read-only collection of <see cref="PropertyInfo"/> instances matching the result of the parsed <paramref name="field"/> or null if no properties where defined.</param>
        /// <returns>The return value of the field or the chained properties of associated <paramref name="properties"/>.</returns>
        public static object ParseStorageFieldValue(object storage, FieldInfo field, IReadOnlyCollection<PropertyInfo> properties)
        {
            if (field == null) { throw new ArgumentNullException("field"); }
            if (storage == null) { throw new ArgumentNullException("storage"); }
            if (properties == null) { throw new ArgumentNullException("properties"); }

            object lastValue = field.GetValue(storage); ;
            if (properties.Count > 0 && lastValue != null)
            {
                foreach (PropertyInfo property in properties)
                {
                    lastValue = property.GetValue(lastValue, null);
                }
                return lastValue;
            }
            return lastValue;
        }

        /// <summary>
        /// Infrastructure. Will set a new value on a chained property if <see cref="PropertyInfo.CanWrite" /> is enabled.
        /// </summary>
        /// <param name="storage">An instance of the storage where the <paramref name="field" /> is located.</param>
        /// <param name="field">The field to retrieve the chained properties from.</param>
        /// <param name="properties">A read-only collection of chained <see cref="PropertyInfo" /> instances.</param>
        /// <param name="propertyType">The type of the property value in <paramref name="propertyValue"/>.</param>
        /// <param name="propertyValue">The new property value to assign the last property in <paramref name="properties"/>.</param>
        public static void SetStorageFieldPropertyValue(object storage, FieldInfo field, IReadOnlyCollection<PropertyInfo> properties, Type propertyType, object propertyValue)
        {
            if (field == null) { throw new ArgumentNullException("field"); }
            if (storage == null) { throw new ArgumentNullException("storage"); }
            if (properties == null) { throw new ArgumentNullException("properties"); }

            object lastValue = field.GetValue(storage); ;
            if (properties.Count > 0 && lastValue != null)
            {
                int counter = 0;
                int max = properties.Count - 1;
                PropertyInfo lastProperty = null;
                foreach (PropertyInfo property in properties)
                {
                    if (counter < max) { lastValue = property.GetValue(lastValue, null); }
                    lastProperty = property;
                    counter++;
                }

                if (lastProperty != null && lastProperty.CanWrite) { lastProperty.SetValue(lastValue, ConvertUtility.ChangeType(propertyValue, propertyType), null); }
            }
        }

        /// <summary>
        /// Infrastructure. This method will parse the provided <paramref name="fieldName" /> with built-in support for chained property retrieval separated by dots (.).
        /// </summary>
        /// <param name="fieldName">Name of the field or the chained property separated by dots (.) to retrieve.</param>
        /// <param name="storageType">The type of the storage where the <paramref name="fieldName" /> is located.</param>
        /// <param name="properties">A read-only collection of <see cref="PropertyInfo"/> instances matching the result of the parsed <paramref name="fieldName"/> or null if no properties where defined.</param>
        /// <returns>A <see cref="FieldInfo" /> instance matching the result of the parsed <paramref name="fieldName" />.</returns>
        /// <remarks>A default implementation where the <paramref name="storageType" /> is parsed using the following <see cref="BindingFlags" /> combination: <see cref="ReflectionUtility.BindingInstancePublicAndPrivateNoneInherited" />.</remarks>
        public static FieldInfo ParseStorageField(ref string fieldName, Type storageType, out IReadOnlyCollection<PropertyInfo> properties)
        {
            return ParseStorageField(ref fieldName, storageType, ReflectionUtility.BindingInstancePublicAndPrivateNoneInherited, out properties);
        }

        /// <summary>
        /// Infrastructure. This method will parse the provided <paramref name="fieldName" /> with built-in support for chained property retrieval separated by dots (.).
        /// </summary>
        /// <param name="fieldName">Name of the field or the chained property separated by dots (.) to retrieve.</param>
        /// <param name="storageType">The type of the storage where the <paramref name="fieldName"/> is located.</param>
        /// <param name="bindings">A bitmask comprised of one or more <see cref="BindingFlags"/> that specify how the search is conducted.</param>
        /// <param name="properties">A read-only collection of <see cref="PropertyInfo"/> instances matching the result of the parsed <paramref name="fieldName"/> or null if no properties where defined.</param>
        /// <returns>A <see cref="FieldInfo"/> instance matching the result of the parsed <paramref name="fieldName"/>.</returns>
        public static FieldInfo ParseStorageField(ref string fieldName, Type storageType, BindingFlags bindings, out IReadOnlyCollection<PropertyInfo> properties)
        {
            if (fieldName == null) { throw new ArgumentNullException("fieldName"); }
            if (storageType == null) { throw new ArgumentNullException("storageType"); }

            string[] propertyNames;
            bool hasProperties = ParseStorageField(ref fieldName, out propertyNames);

            FieldInfo field = storageType.GetField(fieldName, bindings);
            if (field == null)
            {
                Type copyOfStorageType = storageType;
                while (copyOfStorageType != typeof(object))
                {
                    FieldInfo nestedField = copyOfStorageType.GetField(fieldName, bindings);
                    if (nestedField != null)
                    {
                        field = nestedField;
                        break;
                    }
                    copyOfStorageType = copyOfStorageType.BaseType;
                }
            }

            if (field == null) { throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Unable to resolve a FieldInfo from the specified fieldName: '{0}', storageType: {1} and bindings: {2}.", fieldName, storageType, bindings)); }

            if (hasProperties)
            {
                List<PropertyInfo> propertyInfos = new List<PropertyInfo>();
                PropertyInfo lastProperty = null;
                MemberTypes lastMemberType = field.MemberType;
                foreach (string propertyName in propertyNames)
                {
                    switch (lastMemberType)
                    {
                        case MemberTypes.Field:
                            lastProperty = field.FieldType.GetProperty(propertyName, bindings);
                            lastMemberType = lastProperty.MemberType;
                            break;
                        case MemberTypes.Property:
                            if (lastProperty == null) { throw new ParseException(string.Format(CultureInfo.InvariantCulture, "Unable to locate the property named: '{0}'.", lastProperty.Name)); }
                            lastProperty = lastProperty.PropertyType.GetProperty(propertyName, bindings);
                            break;
                    }
                    propertyInfos.Add(lastProperty);
                }
                properties = new ReadOnlyCollection<PropertyInfo>(propertyInfos);
                return field;
            }
            properties = new ReadOnlyCollection<PropertyInfo>(new List<PropertyInfo>());
            return field;
        }

		/// <summary>
		/// Infrastructure. Helper method for filtering columns.
		/// </summary>
		/// <param name="columns">The columns to filter.</param>
		/// <returns>A filtered result of <paramref name="columns"/> containing no DB generated columns.</returns>
		public static ColumnAttribute[] GetNoneDbGeneratedColumns(ColumnAttribute[] columns)
		{
			return Array.FindAll<ColumnAttribute>(columns, ExcludeOnlyDbGeneratedColumns);
		}

		/// <summary>
		/// Infrastructure. Helper method for filtering columns.
		/// </summary>
		/// <param name="columns">The columns to filter.</param>
		/// <returns>A filtered result of <paramref name="columns"/> containing no DB generated columns.</returns>
		public static IEnumerable<ColumnAttribute> GetNoneDbGeneratedColumns(IEnumerable<ColumnAttribute> columns)
		{
			return EnumerableUtility.FindAll<ColumnAttribute>(columns, ExcludeOnlyDbGeneratedColumns);
		}

		/// <summary>
		/// Infrastructure. Helper method for filtering columns.
		/// </summary>
		/// <param name="columns">The columns to filter.</param>
		/// <returns>A filtered result of <paramref name="columns"/> containing no primary key columns.</returns>
		public static ColumnAttribute[] GetColumns(ColumnAttribute[] columns)
		{
			return Array.FindAll<ColumnAttribute>(columns, ExcludeOnlyPrimaryKeyColumns);
		}

		/// <summary>
		/// Infrastructure. Helper method for filtering columns.
		/// </summary>
		/// <param name="columns">The columns to filter.</param>
		/// <returns>A filtered result of <paramref name="columns"/> containing no primary key columns.</returns>
		public static IEnumerable<ColumnAttribute> GetColumns(IEnumerable<ColumnAttribute> columns)
		{
			return EnumerableUtility.FindAll<ColumnAttribute>(columns, ExcludeOnlyPrimaryKeyColumns);
		}

		/// <summary>
		/// Infrastructure. Helper method for filtering columns.
		/// </summary>
		/// <param name="columns">The columns to filter.</param>
		/// <returns>A filtered result of <paramref name="columns"/> containing only foreign key columns.</returns>
		public static ColumnAttribute[] GetForeignKeyColumns(ColumnAttribute[] columns)
		{
			return Array.FindAll<ColumnAttribute>(columns, IncludeOnlyForeignKeyColumns);
		}

		/// <summary>
		/// Infrastructure. Helper method for filtering columns.
		/// </summary>
		/// <param name="columns">The columns to filter.</param>
		/// <returns>A filtered result of <paramref name="columns"/> containing only foreign key columns.</returns>
		public static IEnumerable<ColumnAttribute> GetForeignKeyColumns(IEnumerable<ColumnAttribute> columns)
		{
			return EnumerableUtility.FindAll(columns, IncludeOnlyForeignKeyColumns);
		}

		/// <summary>
		/// Infrastructure. Helper method for filtering columns.
		/// </summary>
		/// <param name="columns">The columns to filter.</param>
		/// <returns>A filtered result of <paramref name="columns"/> containing only primary key columns.</returns>
		public static ColumnAttribute[] GetPrimaryKeyColumns(ColumnAttribute[] columns)
		{
			return Array.FindAll<ColumnAttribute>(columns, IncludeOnlyPrimaryKeyColumns);
		}

		/// <summary>
		/// Infrastructure. Helper method for filtering columns.
		/// </summary>
		/// <param name="columns">The columns to filter.</param>
		/// <returns>A filtered result of <paramref name="columns"/> containing only primary key columns.</returns>
		public static IEnumerable<ColumnAttribute> GetPrimaryKeyColumns(IEnumerable<ColumnAttribute> columns)
		{
			return EnumerableUtility.FindAll(columns, IncludeOnlyPrimaryKeyColumns);
		}
	}
}