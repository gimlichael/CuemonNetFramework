using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using Cuemon.Reflection;

namespace Cuemon.Xml.Serialization
{
    /// <summary>
    /// Serialize and deserialize objects into and from XML documents.
    /// </summary>
    public abstract class XmlSerialization : IXmlSerialization
    {
        private XmlSerializationAttribute _serializationAttribute;
        private readonly object _padLock = new object();

        #region Constructors

        #endregion

        #region Properties

        private XmlSerializationAttribute SerializationAttribute
        {
            get
            {
                if (_serializationAttribute == null)
                {
                    lock (_padLock)
                    {
                        if (_serializationAttribute == null)
                        {
                            XmlSerializationAttribute[] attributes = (XmlSerializationAttribute[])GetType().GetCustomAttributes(typeof(XmlSerializationAttribute), true);
                            if (attributes.Length > 0)
                            {
                                _serializationAttribute = attributes[0];
                            }
                            if (_serializationAttribute == null)
                            {
                                _serializationAttribute = new XmlSerializationAttribute();
                            }
                        }
                    }
                }
                return _serializationAttribute;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// This property is reserved, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"></see> to the class instead.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Xml.Schema.XmlSchema"></see> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"></see> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"></see> method.
        /// </returns>
        public virtual XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"></see> stream from which the object is deserialized.</param>
        public virtual void ReadXml(XmlReader reader)
        {
            if (reader == null) { throw new ArgumentNullException(nameof(reader)); }
            if (!SerializationAttribute.EnableAutomatedXmlSerialization) { throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "The automated XML serialization is not enabled on this class: \"{0}\". Either enable the automated XML serialization using the XmlSerializationAttribute or override the ReadXml(..) method for custom deserialization.", GetType().Name)); }
            XmlSerializationUtility.ParseReadXml(reader, this);
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"></see> stream to which the object is serialized.</param>
        public virtual void WriteXml(XmlWriter writer)
        {
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
            if (!SerializationAttribute.OmitXmlDeclaration) { writer.WriteProcessingInstruction("xml", string.Format(CultureInfo.InvariantCulture, "version=\"1.0\" encoding=\"{0}\"", Encoding.Unicode)); }
            if (!SerializationAttribute.EnableAutomatedXmlSerialization) { throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "The automated XML serialization is not enabled on this class: \"{0}\". Either enable the automated XML serialization using the XmlSerializationAttribute or override the WriteXml(..) method for custom serialization.", GetType().Name)); }
            XmlSerializationUtility.ParseWriteXml(writer, ReflectionUtility.GetObjectHierarchy(this, o =>
            {
                o.MaxDepth = XmlSerializationUtility.MaxSerializationDepth;
                o.SkipPropertyType = XmlSkipPropertiesCallback;
                o.SkipProperty = XmlSerializationUtility.SkipPropertyCallback;

            }));
        }

        private static bool XmlSkipPropertiesCallback(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Empty:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.String:
                    return true;
                default:
                    if (TypeUtility.IsKeyValuePair(type)) { return true; }
                    if (TypeUtility.ContainsType(type, typeof(MemberInfo))) { return true; }
                    return false;
            }
        }

        /// <summary>
        /// Creates and returns a XML stream representation of the current object using UTF-16 for the encoding with the little endian byte order.
        /// </summary>
        /// <returns>A <b><see cref="System.IO.Stream"/></b> object.</returns>
        public Stream ToXml()
        {
            return ToXml(false);
        }

        /// <summary>
        /// Creates and returns a XML stream representation of the current object using UTF-16 for the encoding with the little endian byte order.
        /// </summary>
        /// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c> The default is false.</param>
        /// <returns>A <see cref="Stream"/> containing the serialized XML document.</returns>
        public Stream ToXml(bool omitXmlDeclaration)
        {
            return ToXml(omitXmlDeclaration, null);
        }

        /// <summary>
        /// Creates and returns a XML stream representation of the current object using UTF-16 for the encoding with the little endian byte order.
        /// </summary>
        /// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c> The default is false.</param>
        /// <param name="qualifiedRootEntity">A <see cref="XmlQualifiedEntity"/> that overrides and represents the fully qualified name of the XML root element.</param>
        /// <returns>A <see cref="Stream"/> containing the serialized XML document.</returns>
        public Stream ToXml(bool omitXmlDeclaration, XmlQualifiedEntity qualifiedRootEntity)
        {
            return ToXml(omitXmlDeclaration, qualifiedRootEntity, Encoding.Unicode);
        }

        /// <summary>
        /// Creates and returns a XML stream representation of the current object.
        /// </summary>
        /// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c> The default is false.</param>
        /// <param name="qualifiedRootEntity">A <see cref="XmlQualifiedEntity"/> that overrides and represents the fully qualified name of the XML root element.</param>
        /// <param name="encoding">The text encoding to use.</param>
        /// <returns>
        /// A <see cref="Stream"/> containing the serialized XML document.
        /// </returns>
        public virtual Stream ToXml(bool omitXmlDeclaration, XmlQualifiedEntity qualifiedRootEntity, Encoding encoding)
        {
            return XmlUtility.ConvertEncoding(XmlSerializationUtility.Serialize(this, omitXmlDeclaration, qualifiedRootEntity), encoding);
        }

        /// <summary>
        /// Creates and returns a XML stream representation of the current object.
        /// </summary>
        /// <param name="encoding">The text encoding to use.</param>
        /// <returns>
        /// A <see cref="Stream"/> containing the serialized XML document.
        /// </returns>
        public Stream ToXml(Encoding encoding)
        {
            return ToXml(false, null, encoding);
        }

        /// <summary>
        /// Creates and returns a XML stream representation of the current object.
        /// </summary>
        /// <param name="encoding">The text encoding to use.</param>
        /// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c> The default is false.</param>
        /// <returns>
        /// A <see cref="Stream"/> containing the serialized XML document.
        /// </returns>
        public Stream ToXml(Encoding encoding, bool omitXmlDeclaration)
        {
            return ToXml(omitXmlDeclaration, null, encoding);
        }

        /// <summary>
        /// Creates and returns a XML stream representation of the current object.
        /// </summary>
        /// <param name="encoding">The text encoding to use.</param>
        /// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c> The default is false.</param>
        /// <param name="qualifiedRootEntity">A <see cref="XmlQualifiedEntity"/> that overrides and represents the fully qualified name of the XML root element.</param>
        /// <returns>
        /// A <see cref="Stream"/> containing the serialized XML document.
        /// </returns>
        public Stream ToXml(Encoding encoding, bool omitXmlDeclaration, XmlQualifiedEntity qualifiedRootEntity)
        {
            return ToXml(omitXmlDeclaration, qualifiedRootEntity, encoding);
        }

        /// <summary>
        /// Creates and returns a XML stream representation of the current object.
        /// </summary>
        /// <param name="encoding">The text encoding to use.</param>
        /// <param name="order">A <see cref="SerializableOrder"/> that specifies the order (append or prepend) in which the additional serialization information is applied to the current object.</param>
        /// <param name="writer">The delegate that will either prepend or append complementary XML to the current object.</param>
        /// <returns>A <see cref="Stream"/> containing the serialized XML document.</returns>
        public Stream ToXml(Encoding encoding, SerializableOrder order, Act<XmlWriter> writer)
        {
            return ToXml(encoding, order, false, writer);
        }

        /// <summary>
        /// Creates and returns a XML stream representation of the current object.
        /// </summary>
        /// <param name="encoding">The text encoding to use.</param>
        /// <param name="order">A <see cref="SerializableOrder"/> that specifies the order (append or prepend) in which the additional serialization information is applied to the current object.</param>
        /// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c>. The default is false.</param>
        /// <param name="writer">The delegate that will either prepend or append complementary XML to the current object.</param>
        /// <returns>A <see cref="Stream"/> containing the serialized XML document.</returns>
        public Stream ToXml(Encoding encoding, SerializableOrder order, bool omitXmlDeclaration, Act<XmlWriter> writer)
        {
            return ToXml(encoding, order, omitXmlDeclaration, (XmlQualifiedEntity)null, writer);
        }

        /// <summary>
        /// Creates and returns a XML stream representation of the current object.
        /// </summary>
        /// <param name="encoding">The text encoding to use.</param>
        /// <param name="order">A <see cref="SerializableOrder"/> that specifies the order (append or prepend) in which the additional serialization information is applied to the current object.</param>
        /// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c>. The default is false.</param>
        /// <param name="qualifiedRootEntity">A <see cref="XmlQualifiedEntity"/> that overrides and represents the fully qualified name of the XML root element.</param>
        /// <param name="writer">The delegate that will either prepend or append complementary XML to the current object.</param>
        /// <returns>A <see cref="Stream"/> containing the serialized XML document.</returns>
        public virtual Stream ToXml(Encoding encoding, SerializableOrder order, bool omitXmlDeclaration, XmlQualifiedEntity qualifiedRootEntity, Act<XmlWriter> writer)
        {
            return XmlUtility.ConvertEncoding(XmlSerializationUtility.Serialize(this, order, omitXmlDeclaration, qualifiedRootEntity, writer), encoding);
        }

        /// <summary>
        /// Creates and returns a XML stream representation of the current object.
        /// </summary>
        /// <typeparam name="T">The type of the parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <param name="encoding">The text encoding to use.</param>
        /// <param name="order">A <see cref="SerializableOrder"/> that specifies the order (append or prepend) in which the additional serialization information is applied to the current object.</param>
        /// <param name="writer">The delegate that will either prepend or append complementary XML to the current object.</param>
        /// <param name="arg">The parameter of the delegate <paramref name="writer"/>.</param>
        /// <returns>A <see cref="Stream"/> containing the serialized XML document.</returns>
        public Stream ToXml<T>(Encoding encoding, SerializableOrder order, Act<XmlWriter, T> writer, T arg)
        {
            return ToXml(encoding, order, false, writer, arg);
        }

        /// <summary>
        /// Creates and returns a XML stream representation of the current object.
        /// </summary>
        /// <typeparam name="T">The type of the parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <param name="encoding">The text encoding to use.</param>
        /// <param name="order">A <see cref="SerializableOrder"/> that specifies the order (append or prepend) in which the additional serialization information is applied to the current object.</param>
        /// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c>. The default is false.</param>
        /// <param name="writer">The delegate that will either prepend or append complementary XML to the current object.</param>
        /// <param name="arg">The parameter of the delegate <paramref name="writer"/>.</param>
        /// <returns>A <see cref="Stream"/> containing the serialized XML document.</returns>
        public Stream ToXml<T>(Encoding encoding, SerializableOrder order, bool omitXmlDeclaration, Act<XmlWriter, T> writer, T arg)
        {
            return ToXml(encoding, order, omitXmlDeclaration, (XmlQualifiedEntity)null, writer, arg);
        }

        /// <summary>
        /// Creates and returns a XML stream representation of the current object.
        /// </summary>
        /// <typeparam name="T">The type of the parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <param name="encoding">The text encoding to use.</param>
        /// <param name="order">A <see cref="SerializableOrder"/> that specifies the order (append or prepend) in which the additional serialization information is applied to the current object.</param>
        /// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c>. The default is false.</param>
        /// <param name="qualifiedRootEntity">A <see cref="XmlQualifiedEntity"/> that overrides and represents the fully qualified name of the XML root element.</param>
        /// <param name="writer">The delegate that will either prepend or append complementary XML to the current object.</param>
        /// <param name="arg">The parameter of the delegate <paramref name="writer"/>.</param>
        /// <returns>A <see cref="Stream"/> containing the serialized XML document.</returns>
        public virtual Stream ToXml<T>(Encoding encoding, SerializableOrder order, bool omitXmlDeclaration, XmlQualifiedEntity qualifiedRootEntity, Act<XmlWriter, T> writer, T arg)
        {
            return XmlUtility.ConvertEncoding(XmlSerializationUtility.Serialize(this, order, omitXmlDeclaration, qualifiedRootEntity, writer, arg), encoding);
        }

        /// <summary>
        /// Creates and returns a XML stream representation of the current object.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <param name="encoding">The text encoding to use.</param>
        /// <param name="order">A <see cref="SerializableOrder"/> that specifies the order (append or prepend) in which the additional serialization information is applied to the current object.</param>
        /// <param name="writer">The delegate that will either prepend or append complementary XML to the current object.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="writer"/>.</param>
        /// <returns>A <see cref="Stream"/> containing the serialized XML document.</returns>
        public Stream ToXml<T1, T2>(Encoding encoding, SerializableOrder order, Act<XmlWriter, T1, T2> writer, T1 arg1, T2 arg2)
        {
            return ToXml(encoding, order, false, writer, arg1, arg2);
        }

        /// <summary>
        /// Creates and returns a XML stream representation of the current object.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <param name="encoding">The text encoding to use.</param>
        /// <param name="order">A <see cref="SerializableOrder"/> that specifies the order (append or prepend) in which the additional serialization information is applied to the current object.</param>
        /// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c>. The default is false.</param>
        /// <param name="writer">The delegate that will either prepend or append complementary XML to the current object.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="writer"/>.</param>
        /// <returns>A <see cref="Stream"/> containing the serialized XML document.</returns>
        public Stream ToXml<T1, T2>(Encoding encoding, SerializableOrder order, bool omitXmlDeclaration, Act<XmlWriter, T1, T2> writer, T1 arg1, T2 arg2)
        {
            return ToXml(encoding, order, omitXmlDeclaration, (XmlQualifiedEntity)null, writer, arg1, arg2);
        }

        /// <summary>
        /// Creates and returns a XML stream representation of the current object.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <param name="encoding">The text encoding to use.</param>
        /// <param name="order">A <see cref="SerializableOrder"/> that specifies the order (append or prepend) in which the additional serialization information is applied to the current object.</param>
        /// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c>. The default is false.</param>
        /// <param name="qualifiedRootEntity">A <see cref="XmlQualifiedEntity"/> that overrides and represents the fully qualified name of the XML root element.</param>
        /// <param name="writer">The delegate that will either prepend or append complementary XML to the current object.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="writer"/>.</param>
        /// <returns>A <see cref="Stream"/> containing the serialized XML document.</returns>
        public virtual Stream ToXml<T1, T2>(Encoding encoding, SerializableOrder order, bool omitXmlDeclaration, XmlQualifiedEntity qualifiedRootEntity, Act<XmlWriter, T1, T2> writer, T1 arg1, T2 arg2)
        {
            return XmlUtility.ConvertEncoding(XmlSerializationUtility.Serialize(this, order, omitXmlDeclaration, qualifiedRootEntity, writer, arg1, arg2), encoding);
        }

        /// <summary>
        /// Creates and returns a XML stream representation of the current object.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <param name="encoding">The text encoding to use.</param>
        /// <param name="order">A <see cref="SerializableOrder"/> that specifies the order (append or prepend) in which the additional serialization information is applied to the current object.</param>
        /// <param name="writer">The delegate that will either prepend or append complementary XML to the current object.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="writer"/>.</param>
        /// <returns>A <see cref="Stream"/> containing the serialized XML document.</returns>
        public Stream ToXml<T1, T2, T3>(Encoding encoding, SerializableOrder order, Act<XmlWriter, T1, T2, T3> writer, T1 arg1, T2 arg2, T3 arg3)
        {
            return ToXml(encoding, order, false, writer, arg1, arg2, arg3);
        }

        /// <summary>
        /// Creates and returns a XML stream representation of the current object.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <param name="encoding">The text encoding to use.</param>
        /// <param name="order">A <see cref="SerializableOrder"/> that specifies the order (append or prepend) in which the additional serialization information is applied to the current object.</param>
        /// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c>. The default is false.</param>
        /// <param name="writer">The delegate that will either prepend or append complementary XML to the current object.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="writer"/>.</param>
        /// <returns>A <see cref="Stream"/> containing the serialized XML document.</returns>
        public Stream ToXml<T1, T2, T3>(Encoding encoding, SerializableOrder order, bool omitXmlDeclaration, Act<XmlWriter, T1, T2, T3> writer, T1 arg1, T2 arg2, T3 arg3)
        {
            return ToXml(encoding, order, omitXmlDeclaration, (XmlQualifiedEntity)null, writer, arg1, arg2, arg3);
        }

        /// <summary>
        /// Creates and returns a XML stream representation of the current object.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <param name="encoding">The text encoding to use.</param>
        /// <param name="order">A <see cref="SerializableOrder"/> that specifies the order (append or prepend) in which the additional serialization information is applied to the current object.</param>
        /// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c>. The default is false.</param>
        /// <param name="qualifiedRootEntity">A <see cref="XmlQualifiedEntity"/> that overrides and represents the fully qualified name of the XML root element.</param>
        /// <param name="writer">The delegate that will either prepend or append complementary XML to the current object.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="writer"/>.</param>
        /// <returns>A <see cref="Stream"/> containing the serialized XML document.</returns>
        public virtual Stream ToXml<T1, T2, T3>(Encoding encoding, SerializableOrder order, bool omitXmlDeclaration, XmlQualifiedEntity qualifiedRootEntity, Act<XmlWriter, T1, T2, T3> writer, T1 arg1, T2 arg2, T3 arg3)
        {
            return XmlUtility.ConvertEncoding(XmlSerializationUtility.Serialize(this, order, omitXmlDeclaration, qualifiedRootEntity, writer, arg1, arg2, arg3), encoding);
        }

        /// <summary>
        /// Creates and returns a XML stream representation of the current object.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <param name="encoding">The text encoding to use.</param>
        /// <param name="order">A <see cref="SerializableOrder"/> that specifies the order (append or prepend) in which the additional serialization information is applied to the current object.</param>
        /// <param name="writer">The delegate that will either prepend or append complementary XML to the current object.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="writer"/>.</param>
        /// <returns>A <see cref="Stream"/> containing the serialized XML document.</returns>
        public Stream ToXml<T1, T2, T3, T4>(Encoding encoding, SerializableOrder order, Act<XmlWriter, T1, T2, T3, T4> writer, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return ToXml(encoding, order, false, writer, arg1, arg2, arg3, arg4);
        }

        /// <summary>
        /// Creates and returns a XML stream representation of the current object.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <param name="encoding">The text encoding to use.</param>
        /// <param name="order">A <see cref="SerializableOrder"/> that specifies the order (append or prepend) in which the additional serialization information is applied to the current object.</param>
        /// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c>. The default is false.</param>
        /// <param name="writer">The delegate that will either prepend or append complementary XML to the current object.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="writer"/>.</param>
        /// <returns>A <see cref="Stream"/> containing the serialized XML document.</returns>
        public Stream ToXml<T1, T2, T3, T4>(Encoding encoding, SerializableOrder order, bool omitXmlDeclaration, Act<XmlWriter, T1, T2, T3, T4> writer, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return ToXml(encoding, order, omitXmlDeclaration, (XmlQualifiedEntity)null, writer, arg1, arg2, arg3, arg4);
        }

        /// <summary>
        /// Creates and returns a XML stream representation of the current object.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <param name="encoding">The text encoding to use.</param>
        /// <param name="order">A <see cref="SerializableOrder"/> that specifies the order (append or prepend) in which the additional serialization information is applied to the current object.</param>
        /// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c>. The default is false.</param>
        /// <param name="qualifiedRootEntity">A <see cref="XmlQualifiedEntity"/> that overrides and represents the fully qualified name of the XML root element.</param>
        /// <param name="writer">The delegate that will either prepend or append complementary XML to the current object.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="writer"/>.</param>
        /// <returns>A <see cref="Stream"/> containing the serialized XML document.</returns>
        public virtual Stream ToXml<T1, T2, T3, T4>(Encoding encoding, SerializableOrder order, bool omitXmlDeclaration, XmlQualifiedEntity qualifiedRootEntity, Act<XmlWriter, T1, T2, T3, T4> writer, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return XmlUtility.ConvertEncoding(XmlSerializationUtility.Serialize(this, order, omitXmlDeclaration, qualifiedRootEntity, writer, arg1, arg2, arg3, arg4), encoding);
        }

        /// <summary>
        /// Creates and returns a XML stream representation of the current object.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <param name="encoding">The text encoding to use.</param>
        /// <param name="order">A <see cref="SerializableOrder"/> that specifies the order (append or prepend) in which the additional serialization information is applied to the current object.</param>
        /// <param name="writer">The delegate that will either prepend or append complementary XML to the current object.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="writer"/>.</param>
        /// <returns>A <see cref="Stream"/> containing the serialized XML document.</returns>
        public Stream ToXml<T1, T2, T3, T4, T5>(Encoding encoding, SerializableOrder order, Act<XmlWriter, T1, T2, T3, T4, T5> writer, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return ToXml(encoding, order, false, writer, arg1, arg2, arg3, arg4, arg5);
        }

        /// <summary>
        /// Creates and returns a XML stream representation of the current object.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <param name="encoding">The text encoding to use.</param>
        /// <param name="order">A <see cref="SerializableOrder"/> that specifies the order (append or prepend) in which the additional serialization information is applied to the current object.</param>
        /// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c>. The default is false.</param>
        /// <param name="writer">The delegate that will either prepend or append complementary XML to the current object.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="writer"/>.</param>
        /// <returns>A <see cref="Stream"/> containing the serialized XML document.</returns>
        public Stream ToXml<T1, T2, T3, T4, T5>(Encoding encoding, SerializableOrder order, bool omitXmlDeclaration, Act<XmlWriter, T1, T2, T3, T4, T5> writer, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return ToXml(encoding, order, omitXmlDeclaration, null, writer, arg1, arg2, arg3, arg4, arg5);
        }

        /// <summary>
        /// Creates and returns a XML stream representation of the current object.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <param name="encoding">The text encoding to use.</param>
        /// <param name="order">A <see cref="SerializableOrder"/> that specifies the order (append or prepend) in which the additional serialization information is applied to the current object.</param>
        /// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c>. The default is false.</param>
        /// <param name="qualifiedRootEntity">A <see cref="XmlQualifiedEntity"/> that overrides and represents the fully qualified name of the XML root element.</param>
        /// <param name="writer">The delegate that will either prepend or append complementary XML to the current object.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="writer"/>.</param>
        /// <returns>A <see cref="Stream"/> containing the serialized XML document.</returns>
        public virtual Stream ToXml<T1, T2, T3, T4, T5>(Encoding encoding, SerializableOrder order, bool omitXmlDeclaration, XmlQualifiedEntity qualifiedRootEntity, Act<XmlWriter, T1, T2, T3, T4, T5> writer, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return XmlUtility.ConvertEncoding(XmlSerializationUtility.Serialize(this, order, omitXmlDeclaration, qualifiedRootEntity, writer, arg1, arg2, arg3, arg4, arg5), encoding, omitXmlDeclaration);
        }
        #endregion
    }
}