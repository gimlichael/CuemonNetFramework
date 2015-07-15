using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Cuemon.Collections.Generic;
using Cuemon.Reflection;

namespace Cuemon
{
	/// <summary>
	/// This utility class is designed to make <see cref="Type"/> operations easier to work with.
	/// </summary>
	public static class TypeUtility
	{
        /// <summary>
        /// Determines whether the specified <paramref name="source"/> is of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to compare with <paramref name="source"/>.</typeparam>
        /// <param name="source">The object to compare with <typeparamref name="T"/>.</param>
        /// <returns><c>true</c> if the specified <paramref name="source"/> is of <typeparamref name="T"/>; otherwise, <c>false</c>.</returns>
        public static bool Is<T>(object source)
        {
            return (source is T);
        }

        /// <summary>
        /// Determines whether the specified <paramref name="source"/> is not of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to compare with <paramref name="source"/>.</typeparam>
        /// <param name="source">The object to compare with <typeparamref name="T"/>.</param>
        /// <returns><c>true</c> if the specified <paramref name="source"/> is not of <typeparamref name="T"/>; otherwise, <c>false</c>.</returns>
        public static bool IsNot<T>(object source)
        {
            return !Is<T>(source);
        }


        /// <summary>
        /// Determines whether the specified <paramref name="source"/> implements either <see cref="IEqualityComparer"/> or <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="source">The source type to check for implements of either <see cref="IEqualityComparer"/> or <see cref="IEqualityComparer{T}"/>.</param>
        /// <returns><c>true</c> if the specified <paramref name="source"/> implements either <see cref="IEqualityComparer"/> or <see cref="IEqualityComparer{T}"/>; otherwise, <c>false</c>.</returns>
        public static bool IsEqualityComparer(Type source)
        {
            return ContainsInterface(source, typeof(IEqualityComparer), typeof(IEqualityComparer<>));
        }

        /// <summary>
        /// Determines whether the specified <paramref name="source"/> implements either <see cref="IComparable"/> or <see cref="IComparable{T}"/>.
        /// </summary>
        /// <param name="source">The source type to check for implements of either <see cref="IComparable"/> or <see cref="IComparable{T}"/>.</param>
        /// <returns><c>true</c> if the specified <paramref name="source"/> implements either <see cref="IComparable"/> or <see cref="IComparable{T}"/>; otherwise, <c>false</c>.</returns>
        public static bool IsComparable(Type source)
        {
            return ContainsInterface(source, typeof(IComparable), typeof(IComparable<>));
        }

        /// <summary>
        /// Determines whether the specified <paramref name="source"/> implements either <see cref="IComparer"/> or <see cref="IComparer{T}"/>.
        /// </summary>
        /// <param name="source">The source type to check for implements of either <see cref="IComparer"/> or <see cref="IComparer{T}"/>.</param>
        /// <returns><c>true</c> if the specified <paramref name="source"/> implements either <see cref="IComparer"/> or <see cref="IComparer{T}"/>; otherwise, <c>false</c>.</returns>
        public static bool IsComparer(Type source)
        {
            return ContainsInterface(source, typeof(IComparer), typeof(IComparer<>));
        }

        /// <summary>
        /// Determines whether the specified <paramref name="source"/> implements either <see cref="IEnumerable"/> or <see cref="IEnumerable{T}"/>.
        /// </summary>
        /// <param name="source">The source type to check for implements of either <see cref="IEnumerable"/> or <see cref="IEnumerable{T}"/>.</param>
        /// <returns><c>true</c> if the specified <paramref name="source"/> implements either <see cref="IEnumerable"/> or <see cref="IEnumerable{T}"/>; otherwise, <c>false</c>.</returns>
        public static bool IsEnumerable(Type source)
        {
            return ContainsInterface(source, typeof(IEnumerable), typeof(IEnumerable<>));
        }

        /// <summary>
        /// Determines whether the specified <paramref name="source"/> implements either <see cref="IDictionary"/>, <see cref="IDictionary{TKey,TValue}"/> or <see cref="IReadOnlyDictionary{TKey,TValue}"/>.
        /// </summary>
        /// <param name="source">The source type to check for implements of either <see cref="IDictionary"/>, <see cref="IDictionary{TKey,TValue}"/> or <see cref="IReadOnlyDictionary{TKey,TValue}"/></param>
        /// <returns><c>true</c> if the specified <paramref name="source"/> implements either <see cref="IDictionary"/>, <see cref="IDictionary{TKey,TValue}"/> or <see cref="IReadOnlyDictionary{TKey,TValue}"/>; otherwise, <c>false</c>.</returns>
        public static bool IsDictionary(Type source)
        {
            return ContainsInterface(source, typeof(IDictionary), typeof(IDictionary<,>), typeof(IReadOnlyDictionary<,>));
        }

        /// <summary>
        /// Determines whether the specified <paramref name="source"/> implements either <see cref="DictionaryEntry"/> or <see cref="KeyValuePair{TKey,TValue}"/>.
        /// </summary>
        /// <param name="source">The source type to check for implements of either <see cref="DictionaryEntry"/> or <see cref="KeyValuePair{TKey,TValue}"/>.</param>
        /// <returns><c>true</c> if the specified <paramref name="source"/> implements either <see cref="DictionaryEntry"/> or <see cref="KeyValuePair{TKey,TValue}"/>.; otherwise, <c>false</c>.</returns>
        public static bool IsKeyValuePair(Type source)
        {
            return ContainsType(source, typeof(KeyValuePair<,>), typeof(DictionaryEntry));
        }

        /// <summary>
        /// Sanitizes the name of the <paramref name="source"/> with the intend to be understood by humans. 
        /// </summary>
        /// <param name="source">The type to sanitize the name from.</param>
        /// <returns>A sanitized <see cref="String"/> representation of <paramref name="source"/>.</returns>
        /// <remarks>Only the simple name of the <paramref name="source"/> is returned, not the fully qualified name.</remarks>
        public static string SanitizeTypeName(Type source)
        {
            return SanitizeTypeName(source, false);
        }

        /// <summary>
        /// Sanitizes the name of the <paramref name="source"/> with the intend to be understood by humans. 
        /// </summary>
        /// <param name="source">The type to sanitize the name from.</param>
        /// <param name="fullName">Specify <c>true</c> to use the fully qualified name of the <paramref name="source"/>; otherwise, <c>false</c> for the simple name of <paramref name="source"/>.</param>
        /// <returns>A sanitized <see cref="String"/> representation of <paramref name="source"/>.</returns>
        public static string SanitizeTypeName(Type source, bool fullName)
        {
            return SanitizeTypeName(source, fullName, false);
        }

        /// <summary>
        /// Sanitizes the name of the <paramref name="source"/> with the intend to be understood by humans. 
        /// </summary>
        /// <param name="source">The type to sanitize the name from.</param>
        /// <param name="fullName">Specify <c>true</c> to use the fully qualified name of the <paramref name="source"/>; otherwise, <c>false</c> for the simple name of <paramref name="source"/>.</param>
        /// <param name="excludeGenericArguments">Specify <c>true</c> to exclude generic arguments from the result; otherwise <c>false</c> to include generic arguments should the <paramref name="source"/> be a generic type.</param>
        /// <returns>A sanitized <see cref="String"/> representation of <paramref name="source"/>.</returns>
        public static string SanitizeTypeName(Type source, bool fullName, bool excludeGenericArguments)
        {
            Validator.ThrowIfNull(source, "source");

            string typeName = String.Format(CultureInfo.InvariantCulture, "{0}", fullName ? source.FullName : source.Name);
            if (!source.IsGenericType) { return typeName; }

            Type[] parameters = source.GetGenericArguments();
            int indexOfGraveAccent = typeName.IndexOf('`');
            typeName = indexOfGraveAccent >= 0 ? typeName.Remove(indexOfGraveAccent) : typeName;
            return excludeGenericArguments ? typeName : String.Format(CultureInfo.InvariantCulture, "{0}<{1}>", typeName, ConvertUtility.ToDelimitedString(parameters, ", ", SanitizeTypeNameConverter, fullName));
        }

        private static string SanitizeTypeNameConverter(Type source, bool fullName)
        {
            return fullName ? source.FullName : source.Name;
        }

		/// <summary>
		/// Determines whether the specified source is a nullable <see cref="ValueType"/>.
		/// </summary>
		/// <param name="source">The source type to check for nullable <see cref="ValueType"/>.</param>
		/// <returns>
		///   <c>true</c> if the specified source is nullable; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsNullable(Type source)
		{
			if (source == null) { throw new ArgumentNullException("source"); }
			if (!source.IsValueType) { return false; }
			return Nullable.GetUnderlyingType(source) != null;
		}

		/// <summary>
		/// Determines whether the specified source is a nullable <see cref="ValueType"/>.
		/// </summary>
		/// <typeparam name="T">The type of the <paramref name="source"/> of <typeparamref name="T"/>.</typeparam>
		/// <param name="source">The source type to check for nullable <see cref="ValueType"/>.</param>
		/// <returns>
		///   <c>true</c> if the specified source is nullable; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsNullable<T>(T source) { return false; }

		/// <summary>
		/// Determines whether the specified source is a nullable <see cref="ValueType"/>.
		/// </summary>
		/// <typeparam name="T">The type of the <paramref name="source"/> of <typeparamref name="T"/>.</typeparam>
		/// <param name="source">The source type to check for nullable <see cref="ValueType"/>.</param>
		/// <returns>
		///   <c>true</c> if the specified source is nullable; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsNullable<T>(T? source) where T : struct { return true; }

		/// <summary>
		/// Gets a sequence of derived types from the <paramref name="source"/> an it's associated <see cref="Assembly"/>.
		/// </summary>
		/// <param name="source">The source type to locate derived types from.</param>
		/// <returns>An <see cref="IEnumerable{Type}"/> holding the derived types from the <paramref name="source"/>.</returns>
		public static IEnumerable<Type> GetDescendantOrSelfTypes(Type source)
		{
			if (source == null) { throw new ArgumentNullException("source"); }
			return GetDescendantOrSelfTypes(source, source.Assembly);
		}

		/// <summary>
		/// Gets a sequence of derived types from the <paramref name="source"/>.
		/// </summary>
		/// <param name="source">The source type to locate derived types from.</param>
		/// <param name="assemblies">The assemblies to search for the <paramref name="source"/>.</param>
		/// <returns>An <see cref="IEnumerable{Type}"/> holding the derived types from the <paramref name="source"/>.</returns>
		public static IEnumerable<Type> GetDescendantOrSelfTypes(Type source, params Assembly[] assemblies)
		{
			if (source == null) { throw new ArgumentNullException("source"); }
			if (assemblies == null) { throw new ArgumentNullException("assemblies"); }
			List<Type> derivedTypes = new List<Type>();
			foreach (Assembly assembly in assemblies)
			{
				IEnumerable<Type> assemblyDerivedTypes = ReflectionUtility.GetAssemblyTypes(assembly, null, source);
				foreach (Type derivedType in assemblyDerivedTypes)
				{
					derivedTypes.Add(derivedType);
				}
			}
			return derivedTypes;
		}

        /// <summary>
        /// Gets the ancestor-or-self <see cref="Type"/> from the specified <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The source type to traverse.</param>
        /// <param name="sourceBaseLimit">The base limit of <paramref name="source"/>.</param>
        /// <returns>The ancestor-or-self type from the specified <paramref name="source"/> that is derived or equal to <paramref name="sourceBaseLimit"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="source"/> - or - <paramref name="sourceBaseLimit"/> is null.
        /// </exception>
        public static Type GetAncestorOrSelf(Type source, Type sourceBaseLimit)
        {
            if (source == null) { throw new ArgumentNullException("source"); }
            if (sourceBaseLimit == null) { throw new ArgumentNullException("sourceBaseLimit"); }
            if (source == sourceBaseLimit) { return source; }

            Type sourceBase = source.BaseType;
            while (sourceBase != null)
            {
                if (sourceBase.BaseType == sourceBaseLimit) { break; }
                sourceBase = sourceBase.BaseType;
            }
            return sourceBase;
        }

		/// <summary>
		/// Gets a sequence of ancestor-or-self types from the <paramref name="source"/>.
		/// </summary>
		/// <param name="source">The source type to locate ancestor-or-self types from.</param>
		/// <returns>An <see cref="IEnumerable{Type}"/> holding the ancestor-or-self types from the <paramref name="source"/>.</returns>
		public static IEnumerable<Type> GetAncestorOrSelfTypes(Type source)
		{
			if (source == null) { throw new ArgumentNullException("source"); }
			List<Type> parentTypes = new List<Type>();
			Type currentType = source;
			while (currentType != null)
			{
				parentTypes.Add(currentType);
				currentType = currentType.BaseType;
			}
			return parentTypes;
		}

		/// <summary>
		/// Gets a sorted (base-to-derived) sequence of ancestor-and-descendant-or-self types from the <paramref name="source"/>.
		/// </summary>
		/// <param name="source">The source type to locate ancestor-and-descendant-or-self types from.</param>
		/// <returns>An <see cref="IEnumerable{Type}"/> holding the ancestor-and-descendant-or-self types from the <paramref name="source"/>.</returns>
		public static IEnumerable<Type> GetAncestorAndDescendantsOrSelfTypes(Type source)
		{
			if (source == null) { throw new ArgumentNullException("source"); }
			return GetAncestorAndDescendantsOrSelfTypes(source, source.Assembly);
		}

		/// <summary>
		/// Gets a sorted (base-to-derived) sequence of ancestor-and-descendant-or-self types from the <paramref name="source"/>.
		/// </summary>
		/// <param name="source">The source type to locate ancestor-and-descendant-or-self types from.</param>
		/// <param name="assemblies">The assemblies to search for the <paramref name="source"/>.</param>
		/// <returns>An <see cref="IEnumerable{Type}"/> holding the ancestor-and-descendant-or-self types from the <paramref name="source"/>.</returns>
		public static IEnumerable<Type> GetAncestorAndDescendantsOrSelfTypes(Type source, params Assembly[] assemblies)
		{
			if (source == null) { throw new ArgumentNullException("source"); }
			if (assemblies == null) { throw new ArgumentNullException("assemblies"); }
			IEnumerable<Type> ancestorOrSelfTypes = GetAncestorOrSelfTypes(source);
			IEnumerable<Type> derivedOrSelfTypes = GetDescendantOrSelfTypes(source, assemblies);
			return EnumerableUtility.SortDescending(EnumerableUtility.Distinct(EnumerableUtility.Concat(derivedOrSelfTypes, ancestorOrSelfTypes)), new ReferenceComparer<Type>());
		}

		/// <summary>
		/// Determines whether the specified source contains one or more of the target types specified throughout this member's inheritance chain.
		/// </summary>
		/// <param name="source">The source type to match against.</param>
		/// <param name="targets">The target interface types to be matched against.</param>
		/// <returns>
		/// 	<c>true</c> if the specified source contains one or more of the target types specified throughout this member's inheritance chain; otherwise, <c>false</c>.
		/// </returns>
        public static bool ContainsInterface(Type source, params Type[] targets)
		{
            return ContainsInterface(source, true, targets);
		}

		/// <summary>
		/// Determines whether the specified source contains one or more of the target types specified throughout this member's inheritance chain.
		/// </summary>
		/// <param name="source">The source object to match against.</param>
		/// <param name="targets">The target interface types to be matched against.</param>
		/// <returns>
		/// 	<c>true</c> if the specified source contains one or more of the target types specified throughout this member's inheritance chain; otherwise, <c>false</c>.
		/// </returns>
		public static bool ContainsInterface(object source, params Type[] targets)
		{
			if (source == null) throw new ArgumentNullException("source");
			return ContainsInterface(source.GetType(), targets);
		}

		/// <summary>
		/// Determines whether the specified source contains one or more of the target types specified.
		/// </summary>
		/// <param name="source">The source object to match against.</param>
		/// <param name="inherit">Specifies whether to search this member's inheritance chain to find the interfaces.</param>
		/// <param name="targets">The target interface types to be matched against.</param>
		/// <returns>
		/// 	<c>true</c> if the specified source contains one or more of the target types specified; otherwise, <c>false</c>.
		/// </returns>
		public static bool ContainsInterface(object source, bool inherit, params Type[] targets)
		{
			if (source == null) throw new ArgumentNullException("source");
			return ContainsInterface(source.GetType(), inherit, targets);
		}

		/// <summary>
		/// Determines whether the specified source contains one or more of the target types specified.
		/// </summary>
		/// <param name="source">The source type to match against.</param>
		/// <param name="inherit">Specifies whether to search this member's inheritance chain to find the interfaces.</param>
		/// <param name="targets">The target interface types to be matched against.</param>
		/// <returns>
		/// 	<c>true</c> if the specified source contains one or more of the target types specified; otherwise, <c>false</c>.
		/// </returns>
		public static bool ContainsInterface(Type source, bool inherit, params Type[] targets)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (targets == null) throw new ArgumentNullException("targets");
			foreach (Type targetType in targets)
			{
				if (inherit) // search all inheritance chains
				{
					foreach (Type interfaceType in source.GetInterfaces())
					{
						if (interfaceType.IsGenericType)
						{
							if (targetType == interfaceType.GetGenericTypeDefinition()) { return true; }
							continue;
						}
						if (targetType == interfaceType) { return true; }
					}
				}
				else // search this type only
				{
					Type interfaceType = source.GetInterface(targetType.Name, true);
					if (interfaceType != null)
					{
						if (interfaceType.IsGenericType)
						{
							if (targetType == interfaceType.GetGenericTypeDefinition()) { return true; }
						}
						if (targetType == interfaceType) { return true; }
					}
				}
			}
			return false;
		}

        /// <summary>
        /// Determines whether the specified source object contains one or more of the specified attribute target types.
        /// </summary>
        /// <param name="source">The source object to match against.</param>
        /// <param name="targets">The attribute target types to be matched against.</param>
        /// <returns>
        /// 	<c>true</c> if the specified source object contains one or more of the specified attribute target types; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsAttributeType(object source, params Type[] targets)
        {
            return ContainsAttributeType(source, false, targets);
        }

		/// <summary>
		/// Determines whether the specified source object contains one or more of the specified attribute target types.
		/// </summary>
		/// <param name="source">The source object to match against.</param>
        /// <param name="inherit"><c>true</c> to search the <paramref name="source"/>  inheritance chain to find the attributes; otherwise, <c>false</c>.</param>
        /// <param name="targets">The attribute target types to be matched against.</param>
		/// <returns>
		/// 	<c>true</c> if the specified source object contains one or more of the specified attribute target types; otherwise, <c>false</c>.
		/// </returns>
        public static bool ContainsAttributeType(object source, bool inherit, params Type[] targets)
		{
			if (source == null) { throw new ArgumentNullException("source"); }
            return ContainsAttributeType(source.GetType(), inherit, targets);
		}

        /// <summary>
        /// Determines whether the specified source type contains one or more of the specified attribute target types.
        /// </summary>
        /// <param name="source">The source type to match against.</param>
        /// <param name="targets">The attribute target types to be matched against.</param>
        /// <returns>
        /// 	<c>true</c> if the specified source type contains one or more of the specified attribute target types; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsAttributeType(Type source, params Type[] targets)
        {
            return ContainsAttributeType(source, false, targets);
        }

		/// <summary>
		/// Determines whether the specified source type contains one or more of the specified attribute target types.
		/// </summary>
		/// <param name="source">The source type to match against.</param>
        /// <param name="inherit"><c>true</c> to search the <paramref name="source"/> inheritance chain to find the attributes; otherwise, <c>false</c>.</param>
		/// <param name="targets">The attribute target types to be matched against.</param>
		/// <returns>
		/// 	<c>true</c> if the specified source type contains one or more of the specified attribute target types; otherwise, <c>false</c>.
		/// </returns>
		public static bool ContainsAttributeType(Type source, bool inherit, params Type[] targets)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (targets == null) throw new ArgumentNullException("targets");
			foreach (Type targetType in targets)
			{
				if (source.GetCustomAttributes(targetType, inherit).Length > 0) { return true; }
			}

			foreach (MemberInfo member in source.GetMembers())
			{
                if (ContainsAttributeType(member, inherit, targets)) { return true; }
			}

			return false;
		}

        /// <summary>
        /// Determines whether the specified source type contains one or more of the specified attribute target types.
        /// </summary>
        /// <param name="source">The member to match against.</param>
        /// <param name="targets">The attribute target types to be matched against.</param>
        /// <returns>
        /// 	<c>true</c> if the specified member contains one or more of the specified attribute target types; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsAttributeType(MemberInfo source, params Type[] targets)
        {
            return ContainsAttributeType(source, false, targets);
        }

        /// <summary>
        /// Determines whether the specified source type contains one or more of the specified attribute target types.
        /// </summary>
        /// <param name="source">The member to match against.</param>
        /// <param name="inherit"><c>true</c> to search the <paramref name="source"/> inheritance chain to find the attributes; otherwise, <c>false</c>.</param>
        /// <param name="targets">The attribute target types to be matched against.</param>
        /// <returns>
        /// 	<c>true</c> if the specified member contains one or more of the specified attribute target types; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsAttributeType(MemberInfo source, bool inherit, params Type[] targets)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (targets == null) throw new ArgumentNullException("targets");
            foreach (Type targetType in targets)
            {
                if (source.GetCustomAttributes(targetType, inherit).Length > 0) { return true; }
            }
            return false;
        }

		/// <summary>
		/// Determines whether the specified collection of source objects contains one or more of the specified target types.
		/// </summary>
		/// <param name="sources">The collection of source objects to match against.</param>
		/// <param name="targets">The target types to be matched against.</param>
		/// <returns>
		/// 	<c>true</c> if the specified collection of source objects contains one more of the target types specified; otherwise, <c>false</c>.
		/// </returns>
		private static bool ContainsType(IEnumerable sources, params Type[] targets)
		{
			if (sources == null) throw new ArgumentNullException("sources");
			foreach (object source in sources)
			{
				if (ContainsType(source.GetType(), targets)) { return true; }
			}
			return false;
		}

		/// <summary>
		/// Determines whether the specified source type contains one or more of the specified target types.
		/// </summary>
		/// <param name="source">The source type to match against.</param>
		/// <param name="targets">The target types to be matched against.</param>
		/// <returns>
		/// 	<c>true</c> if the specified source contains one or more of the specified target types; otherwise, <c>false</c>.
		/// </returns>
		public static bool ContainsType(Type source, params Type[] targets)
		{
			if (targets == null) throw new ArgumentNullException("targets");
			foreach (Type targetType in targets)
			{
				Type sourceTypeCopy = source;
				while (sourceTypeCopy != typeof(object) && sourceTypeCopy != null) // recursively loop through all inheritance types of the source
				{
					if (sourceTypeCopy.IsGenericType)
					{
						if (targetType == sourceTypeCopy.GetGenericTypeDefinition()) { return true; }
					}
					if (sourceTypeCopy == targetType) // we have a matching type as specified in parameter targetTypes
					{
						return true;
					}
					sourceTypeCopy = sourceTypeCopy.BaseType; // get the inheriting type
				}
			}
			return false;
		}

		/// <summary>
		/// Determines whether the specified source contains one or more of the specified target types.
		/// </summary>
		/// <param name="source">The source object to match against.</param>
		/// <param name="targets">The target types to be matched against.</param>
		/// <returns>
		/// 	<c>true</c> if the specified source contains one or more of the specified target types; otherwise, <c>false</c>.
		/// </returns>
		public static bool ContainsType(object source, params Type[] targets)
		{
			if (source == null) throw new ArgumentNullException("source");
			return ContainsType(source.GetType(), targets);
		}

		/// <summary>
		/// Determines whether the specified source/collection of source object(s) contains one or more of the specified target types.
		/// </summary>
		/// <param name="source">The source object to match against.</param>
		/// <param name="treatSourceAsEnumerable">if set to <c>true</c> the source object is cast as an <see cref="IEnumerable"/> object, and the actual matching is now done against the source objects within the collection against the target types specified.</param>
		/// <param name="targets">The target types to be matched against.</param>
		/// <returns>
		/// 	<c>true</c> if the specified source contains one or more of the specified target types; otherwise, <c>false</c>.
		/// </returns>
		public static bool ContainsType(object source, bool treatSourceAsEnumerable, params Type[] targets)
		{
			if (source == null) throw new ArgumentNullException("source");
			return treatSourceAsEnumerable ? ContainsType((source as IEnumerable), targets) : ContainsType(source.GetType(), targets);
		}

        /// <summary>
        /// Determines whether the specified <paramref name="source"/> is a complex <see cref="Type"/>.
        /// </summary>
        /// <param name="source">The <see cref="Type"/> to determine complexity for.</param>
        /// <returns><c>true</c> if specified <paramref name="source"/> is a complex <see cref="Type"/>; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null.
        /// </exception>
	    public static bool IsComplex(Type source)
	    {
            if (source == null) { throw new ArgumentNullException("source"); }
	        return !((source.IsClass && source == typeof(string)) ||
	                 (source.IsClass && source == typeof(object)) ||
	                 (source.IsValueType ||
	                  source.IsPrimitive ||
	                  source.IsEnum));
	    }

        /// <summary>
        /// Gets the default value of the specified <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to retrieve its default value from.</param>
        /// <returns>The default value of <paramref name="type"/>.</returns>
	    public static object GetDefaultValue(Type type)
	    {
            Validator.ThrowIfNull(type, "type");
	        if (type.IsValueType && Nullable.GetUnderlyingType(type) == null) { return Activator.CreateInstance(type); }
	        return null;
	    }
	}
}