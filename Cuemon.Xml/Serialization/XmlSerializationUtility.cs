using System;
using System.IO;
using System.Xml.Serialization;
using Cuemon.Reflection;

namespace Cuemon.Xml.Serialization
{
	/// <summary>
	/// This utility class is designed to make XML serialization operations easier to work with.
	/// </summary>
	public static partial class XmlSerializationUtility
	{
		/// <summary>
		/// Determines whether the specified object contains XML serialization attributes.
		/// </summary>
		/// <param name="source">The object to parse for XML serialization attributes.</param>
		/// <returns>
		/// 	<c>true</c> if the specified object contains XML serialization attributes; otherwise, <c>false</c>.
		/// </returns>
		public static bool ContainsXmlSerializationAttributes(object source)
		{
			if (source == null) throw new ArgumentNullException("source");
			return ContainsXmlSerializationAttributes(source.GetType());
		}

		/// <summary>
		/// Determines whether the specified type contains XML serialization attributes.
		/// </summary>
		/// <param name="sourceType">The type to parse for XML serialization attributes.</param>
		/// <returns>
		/// 	<c>true</c> if the specified type contains XML serialization attributes; otherwise, <c>false</c>.
		/// </returns>
		public static bool ContainsXmlSerializationAttributes(Type sourceType)
		{
			return TypeUtility.ContainsAttributeType(sourceType, typeof(XmlAnyAttributeAttribute), typeof(XmlAnyElementAttribute), typeof(XmlArrayAttribute), typeof(XmlArrayItemAttribute),
				typeof(XmlAttributeAttribute), typeof(XmlChoiceIdentifierAttribute), typeof(XmlElementAttribute), typeof(XmlEnumAttribute),
				typeof(XmlTextAttribute), typeof(XmlTypeAttribute));
			// decided to leave out XmlRootAttribute, as this can favor more use of the automated serialization (where the coder still can override the class name with a new root name).
		}

		/// <summary>
		/// Creates and returns a XML string representation of the given object.
		/// </summary>
		/// <param name="serializable">An object implementing the <see cref="Cuemon.Xml.Serialization.IXmlSerialization"/> interface.</param>
        /// <returns>A <see cref="String"/> holding the serialized version of <paramref name="serializable"/>.</returns>
		public static string SerializeAsString(IXmlSerialization serializable)
		{
			if (serializable == null) { throw new ArgumentNullException("serializable"); }
			return SerializeAsString(serializable, false);
		}

		/// <summary>
		/// Creates and returns a XML string representation of the given object using the provided parameters.
		/// </summary>
		/// <param name="serializable">An object implementing the <see cref="Cuemon.Xml.Serialization.IXmlSerialization"/> interface.</param>
		/// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c> The default is false.</param>
        /// <returns>A <see cref="String"/> holding the serialized version of <paramref name="serializable"/>.</returns>
		public static string SerializeAsString(IXmlSerialization serializable, bool omitXmlDeclaration)
		{
			return SerializeAsString(serializable, omitXmlDeclaration, null);
		}

		/// <summary>
		/// Creates and returns a XML string representation of the given object using the provided parameters.
		/// </summary>
		/// <param name="serializable">An object implementing the <see cref="Cuemon.Xml.Serialization.IXmlSerialization"/> interface.</param>
		/// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c> The default is false.</param>
        /// <param name="qualifiedRootEntity">A <see cref="XmlQualifiedEntity"/> that overrides and represents the fully qualified name of the XML root element.</param>
        /// <returns>A <see cref="String"/> holding the serialized version of <paramref name="serializable"/>.</returns>
        public static string SerializeAsString(IXmlSerialization serializable, bool omitXmlDeclaration, XmlQualifiedEntity qualifiedRootEntity)
		{
			if (serializable == null) { throw new ArgumentNullException("serializable"); }
			using (Stream value = serializable.ToXml(omitXmlDeclaration, qualifiedRootEntity))
			{
				return serializable.ToString(value);
			}
		}

        /// <summary>
        /// Returns a <see cref="DefaultOr{T}"/> that intermediates an <see cref="XmlSerializationAttribute"/>.
        /// </summary>
        /// <typeparam name="T">The type of the <paramref name="source"/> object.</typeparam>
        /// <param name="source">The object to retrieve a <see cref="XmlSerializationAttribute"/> from.</param>
        /// <returns>An instance of <see cref="DefaultOr{T}"/> that intermediates an <see cref="XmlSerializationAttribute"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="source"/> is null.
        /// </exception>
        public static DefaultOr<XmlSerializationAttribute> DefaultOrXmlSerializationAttribute<T>(T source)
        {
            return DefaultOr.Create(ReflectionUtility.GetAttribute<XmlSerializationAttribute>(source.GetType(), true));
        }
	}
}