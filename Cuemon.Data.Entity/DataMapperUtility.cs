using System;
using System.Collections.Generic;
using System.Reflection;
using Cuemon.Caching;
using Cuemon.Collections.Generic;
using Cuemon.Data.Entity.Mapping;
using Cuemon.Reflection;

namespace Cuemon.Data.Entity
{
    /// <summary>
    /// This utility class is designed to make parsing of attributes found in the <see cref="Cuemon.Data.Entity.Mapping"/> namespace easier to work with.
    /// </summary>
    public static class DataMapperUtility
    {
        /// <summary>
        /// Parses and returns a sequence of <see cref="ColumnAttribute"/> decorations from the specified <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The <see cref="Type"/> to parse for <see cref="ColumnAttribute"/> decorations.</param>
        /// <returns>A sequence of <see cref="ColumnAttribute"/> decorations.</returns>
        public static IEnumerable<ColumnAttribute> ParseColumns(Type source)
        {
            Doer<Type, IEnumerable<ColumnAttribute>> getColumns = CachingManager.Cache.Memoize<Type, IEnumerable<ColumnAttribute>>(ParseColumnsCore);
            return getColumns(source);
        }

        private static IEnumerable<ColumnAttribute> ParseColumnsCore(Type source)
        {
            Validator.ThrowIfNull(source, "source");

            IDictionary<PropertyInfo, ColumnAttribute[]> tempDictionary = ReflectionUtility.GetPropertyAttributeDecorations<ColumnAttribute>(source, ReflectionUtility.BindingInstancePublicAndPrivateNoneInheritedIncludeStatic);
            List<ColumnAttribute> columnAttributes = new List<ColumnAttribute>();
            foreach (KeyValuePair<PropertyInfo, ColumnAttribute[]> keyValuePair in tempDictionary)
            {
                foreach (ColumnAttribute column in keyValuePair.Value)
                {
                    column.DecoratedProperty = keyValuePair.Key;
                    columnAttributes.Add(column);
                }
            }


            if (TypeUtility.ContainsInterface(source, typeof(ICollection<>)))
            {
                Type currentGenericTypeToSearch = source;
                Type[] genericTypes = new Type[0];
                while (currentGenericTypeToSearch != typeof(ICollection<>) && genericTypes.Length == 0)
                {
                    genericTypes = currentGenericTypeToSearch.GetGenericArguments();
                    currentGenericTypeToSearch = currentGenericTypeToSearch.BaseType;
                }

                Type generic = genericTypes[0];
                if (generic != null)
                {
                    IDictionary<PropertyInfo, ColumnAttribute[]> genericTempDictionary = ReflectionUtility.GetPropertyAttributeDecorations<ColumnAttribute>(generic, ReflectionUtility.BindingInstancePublicAndPrivateNoneInheritedIncludeStatic);
                    foreach (KeyValuePair<PropertyInfo, ColumnAttribute[]> keyValuePair in genericTempDictionary)
                    {
                        foreach (ColumnAttribute column in keyValuePair.Value)
                        {
                            column.DecoratedProperty = keyValuePair.Key;
                            if (column.IsPrimaryKey) { columnAttributes.Add(column); }
                        }
                    }
                }
            }

            return new List<ColumnAttribute>(EnumerableUtility.Distinct(columnAttributes, new PropertyEqualityComparer<ColumnAttribute>("ParameterName")));
        }
    }
}
