﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using Cuemon.Collections.Generic;
using Cuemon.Reflection;
using Cuemon.Runtime.Serialization;

namespace Cuemon.Xml.Serialization
{
    public static partial class XmlSerializationUtility
    {
        internal static void ParseWriteXml(XmlWriter writer, IHierarchy<object> node)
        {
            if (writer == null) { throw new ArgumentNullException("writer"); }
            if (node == null) { throw new ArgumentNullException("node"); }
            ParseWriteXml(writer, node, false);
        }

        internal static void ParseWriteXml(XmlWriter writer, IHierarchy<object> node, bool enumerableCaller)
        {
            if (writer == null) { throw new ArgumentNullException("writer"); }
            if (node == null) { throw new ArgumentNullException("node"); }
            DefaultOr<XmlSerializationAttribute> serializationRules = DefaultOrXmlSerializationAttribute(node.Instance);
            ParseWriteXml(writer, node, enumerableCaller, serializationRules.Instance.DefaultSerializationMethod);
        }

        internal static void ParseWriteXml(XmlWriter writer, IHierarchy<object> node, bool enumerableCaller, SerializationMethod method)
        {
            if (writer == null) { throw new ArgumentNullException("writer"); }
            if (node == null) { throw new ArgumentNullException("node"); }
            if (node.HasChildren)
            {
                WriteEnumerable(writer, node);
                WriteChildren(writer, node);
            }
            else
            {
                WriteEnumerable(writer, node);
                WriteValue(writer, node, method, ParseInstanceForXml, enumerableCaller);
            }
        }

        internal static void WriteXml(XmlWriter writer, IHierarchy<object> node)
        {
            WriteXml(writer, node, false);
        }

        internal static void WriteXml(XmlWriter writer, IHierarchy<object> node, bool enumerableCaller)
        {
            if (writer == null) { throw new ArgumentNullException("writer"); }
            if (node == null) { throw new ArgumentNullException("node"); }

            bool hasIgnoreAttribute = node.HasMemberReference && TypeUtility.ContainsAttributeType(node.MemberReference, typeof(XmlIgnoreAttribute));
            if (hasIgnoreAttribute) { return; }

            MethodInfo writeXmlMethod = node.InstanceType.GetMethod("WriteXml", ReflectionUtility.BindingInstancePublicAndPrivate);
            DefaultOr<XmlSerializationAttribute> serializationRules = DefaultOrXmlSerializationAttribute(node.Instance);
            bool useXmlWriter = (writeXmlMethod != null) &&
                        (TypeUtility.ContainsInterface(node.InstanceType, typeof(IXmlSerializable)) &&
                        ((!serializationRules.IsDefault && !serializationRules.Instance.EnableAutomatedXmlSerialization) || TypeUtility.ContainsInterface(node.InstanceType, typeof(IXmlSerialization))));

            if (!useXmlWriter && (serializationRules.IsDefault || serializationRules.Instance.EnableAutomatedXmlSerialization))
            {
                ParseWriteXml(writer, node, enumerableCaller, serializationRules.Instance.DefaultSerializationMethod);
            }
            else
            {
                writeXmlMethod.Invoke(node.Instance, new object[] { writer });
            }
        }

        private static void WriteChildren(XmlWriter writer, IHierarchy<object> node)
        {
            if (writer == null) { throw new ArgumentNullException("writer"); }
            if (node == null) { throw new ArgumentNullException("node"); }

            foreach (IHierarchy<object> currentNode in SortChildren(node.GetChildren()))
            {
                bool hasIgnoreAttribute = currentNode.HasMemberReference && TypeUtility.ContainsAttributeType(currentNode.MemberReference, typeof(XmlIgnoreAttribute));
                if (hasIgnoreAttribute) { continue; }

                MethodInfo writeXmlMethod = currentNode.InstanceType.GetMethod("WriteXml", ReflectionUtility.BindingInstancePublicAndPrivate);
                DefaultOr<XmlSerializationAttribute> serializationRules = DefaultOrXmlSerializationAttribute(currentNode.Instance);
                bool useXmlWriter = (writeXmlMethod != null) &&
                        (TypeUtility.ContainsInterface(currentNode.InstanceType, typeof(IXmlSerializable)) &&
                        ((!serializationRules.IsDefault && !serializationRules.Instance.EnableAutomatedXmlSerialization) || TypeUtility.ContainsInterface(currentNode.InstanceType, typeof(IXmlSerialization))));

                XmlQualifiedEntity qualifiedEntity = GetXmlStartElementQualifiedName(currentNode);
                if (currentNode.HasChildren && TypeUtility.IsComplex(currentNode.InstanceType)) { writer.WriteStartElement(qualifiedEntity.Prefix, qualifiedEntity.LocalName, qualifiedEntity.Namespace); }
                if (!useXmlWriter && (serializationRules.IsDefault || serializationRules.Instance.EnableAutomatedXmlSerialization))
                {
                    ParseWriteXml(writer, currentNode, false, serializationRules.Instance.DefaultSerializationMethod);
                }
                else
                {
                    writeXmlMethod.Invoke(currentNode.Instance, new object[] { writer });
                }
                if (currentNode.HasChildren && TypeUtility.IsComplex(currentNode.InstanceType)) { writer.WriteEndElement(); }
            }
        }

        internal static XmlQualifiedEntity GetXmlStartElementQualifiedName(IHierarchy<object> node)
        {
            return GetXmlStartElementQualifiedName(node, null);
        }

        internal static XmlQualifiedEntity GetXmlStartElementQualifiedName(IHierarchy<object> node, XmlQualifiedEntity qualifiedRootEntity)
        {
            if (node == null) { throw new ArgumentNullException("node"); }
            if (qualifiedRootEntity != null && !string.IsNullOrEmpty(qualifiedRootEntity.LocalName)) { return qualifiedRootEntity; }
            bool hasRootAttribute = TypeUtility.ContainsAttributeType(node.InstanceType, typeof(XmlRootAttribute));
            bool hasElementAttribute = node.HasMemberReference && TypeUtility.ContainsAttributeType(node.MemberReference, typeof(XmlElementAttribute));
            bool hasWrapperAttribute = TypeUtility.ContainsAttributeType(Hierarchy.Root(node).InstanceType, true, typeof(XmlWrapperAttribute));
            string rootOrElementName = XmlUtility.SanitizeElementName(node.HasMemberReference ? node.MemberReference.Name : TypeUtility.SanitizeTypeName(node.InstanceType, false, true));
            string ns = null;

            if (hasRootAttribute || hasElementAttribute)
            {
                string elementName = null;
                if (hasRootAttribute)
                {
                    XmlRootAttribute rootAttribute = ReflectionUtility.GetAttribute<XmlRootAttribute>(node.InstanceType);
                    elementName = rootAttribute.ElementName;
                    ns = rootAttribute.Namespace;
                }

                if (hasElementAttribute)
                {
                    XmlElementAttribute elementAttribute = ReflectionUtility.GetAttribute<XmlElementAttribute>(node.MemberReference);
                    elementName = elementAttribute.ElementName;
                    ns = elementAttribute.Namespace;
                }

                if (!string.IsNullOrEmpty(elementName))
                {
                    rootOrElementName = elementName;
                }
            }

            if (hasWrapperAttribute)
            {
                PropertyInfo property = ReflectionUtility.GetProperty(node.InstanceType, "InstanceName");
                if (property != null)
                {
                    XmlQualifiedEntity qualifiedEntity = property.GetValue(node.Instance, null) as XmlQualifiedEntity;
                    if (qualifiedEntity != null)
                    {
                        rootOrElementName = qualifiedEntity.LocalName;
                        ns = qualifiedEntity.Namespace;
                    }
                }
            }

            XmlQualifiedEntity instance = node.Instance as XmlQualifiedEntity;
            return instance ?? new XmlQualifiedEntity(XmlUtility.SanitizeElementName(rootOrElementName), ns);
        }

        internal static void WriteValue(XmlWriter writer, IHierarchy<object> node, SerializationMethod method, Doer<IWrapper<object>, string> parser)
        {
            WriteValue(writer, node, method, parser, false);
        }

        internal static void WriteValue(XmlWriter writer, IHierarchy<object> node, SerializationMethod method, Doer<IWrapper<object>, string> parser, bool enumerableCaller)
        {
            if (writer == null) { throw new ArgumentNullException("writer"); }
            if (node == null) { throw new ArgumentNullException("node"); }
            if (parser == null) { throw new ArgumentNullException("parser");}
            if (IsEnumerable(node)) { return; }

            bool hasAttributeAttribute = node.HasMemberReference && TypeUtility.ContainsAttributeType(node.MemberReference, typeof(XmlAttributeAttribute));
            bool hasElementAttribute = node.HasMemberReference && TypeUtility.ContainsAttributeType(node.MemberReference, typeof(XmlElementAttribute));
            bool hasTextAttribute = node.HasMemberReference && TypeUtility.ContainsAttributeType(node.MemberReference, typeof(XmlTextAttribute));
            bool hasWrapperAttribute = TypeUtility.ContainsAttributeType(Hierarchy.Root(node).InstanceType, true, typeof(XmlWrapperAttribute));

            bool isType = node.Instance is Type;
            Type nodeType = isType ? (Type)node.Instance : node.InstanceType;
            string attributeOrElementName = XmlUtility.SanitizeElementName(node.HasMemberReference ? node.MemberReference.Name : TypeUtility.SanitizeTypeName(nodeType));

            if (!hasAttributeAttribute && !hasElementAttribute && !hasTextAttribute)
            {
                switch (method)
                {
                    case SerializationMethod.XmlAttributeAttribute:
                        hasAttributeAttribute = true;
                        break;
                    case SerializationMethod.XmlElementAttribute:
                        hasElementAttribute = true;
                        break;
                }

                if ((!TypeUtility.IsComplex(nodeType) || isType) && !node.HasChildren && (!enumerableCaller || isType))
                {
                    if (!node.HasParent)
                    {
                        hasAttributeAttribute = false;
                        hasElementAttribute = false;
                    }
                    hasTextAttribute = true;
                }
            }
            else
            {
                string elementName = null;
                if (hasAttributeAttribute) { elementName = ReflectionUtility.GetAttribute<XmlAttributeAttribute>(node.MemberReference).AttributeName; }
                if (hasElementAttribute) { elementName = ReflectionUtility.GetAttribute<XmlElementAttribute>(node.MemberReference).ElementName; }

                if (!string.IsNullOrEmpty(elementName))
                {
                    attributeOrElementName = elementName;
                }
            }

            if (hasWrapperAttribute)
            {
                if (node.HasMemberReference && node.MemberReference.Name == "Instance")
                {
                    IHierarchy<object> itemNode = new Hierarchy<object>();
                    itemNode.Add(node.Instance);
                    WriteXml(writer, itemNode);   
                }
                return;    
            }

            if (hasAttributeAttribute)
            {
                writer.WriteAttributeString(attributeOrElementName, hasWrapperAttribute ? parser(node.Instance as IWrapper<object>) : parser(node));
            }
            else if (hasElementAttribute)
            {
                writer.WriteElementString(attributeOrElementName, hasWrapperAttribute ? parser(node.Instance as IWrapper<object>) : parser(node));    
            }
            else if (hasTextAttribute)
            {
                writer.WriteString(hasWrapperAttribute ? parser(node.Instance as IWrapper<object>) : parser(node));
            }
        }

        internal static void WriteEnumerable(XmlWriter writer, IHierarchy<object> current)
        {
            WriteEnumerable(writer, current, false);
        }

        internal static void WriteEnumerable(XmlWriter writer, IHierarchy<object> current, bool skipStartElement)
        {
            if (writer == null) { throw new ArgumentNullException("writer"); }
            if (current == null) { throw new ArgumentNullException("current"); }

            Type currentType = current.Instance.GetType();
            if (TypeUtility.IsEnumerable(currentType) && currentType != typeof(string))
            {
                bool hasWrapperAttribute = TypeUtility.ContainsAttributeType(Hierarchy.Root(current).InstanceType, true, typeof(XmlWrapperAttribute));
                if (hasWrapperAttribute && current.HasMemberReference && current.MemberReference.Name == "Data") { return; }

                bool isDictionary = TypeUtility.IsDictionary(currentType);
                IEnumerable enumerable = current.Instance as IEnumerable;
                if (enumerable != null)
                {
                    if (!skipStartElement && !current.HasChildren && !hasWrapperAttribute)
                    {
                        XmlQualifiedEntity qualifiedEntity = null;
                        if (!TypeUtility.ContainsAttributeType(current.InstanceType, typeof (XmlRootAttribute)) && current.MemberReference == null)
                        {
                            qualifiedEntity = new XmlQualifiedEntity(TypeUtility.SanitizeTypeName(current.InstanceType, false, true));
                        }
                        qualifiedEntity = GetXmlStartElementQualifiedName(current, qualifiedEntity);
                        writer.WriteStartElement(qualifiedEntity.Prefix, qualifiedEntity.LocalName, qualifiedEntity.Namespace);
                    }
                    IEnumerator enumerator = enumerable.GetEnumerator();
                    IHierarchy<object> enumeratorNode = new Hierarchy<object>();
                    enumeratorNode.Add(enumerator);
                    while (enumerator.MoveNext())
                    {
                        object value = enumerator.Current;
                        if (value == null) { continue; }
                        Type valueType = value.GetType();
                        bool isComplexType = TypeUtility.IsComplex(valueType);
                        if (isDictionary)
                        {
                            writer.WriteStartElement("KeyValuePair");
                            PropertyInfo keyProperty = valueType.GetProperty("Key");
                            PropertyInfo valueProperty = valueType.GetProperty("Value");
                            object keyValue = keyProperty.GetValue(value, null) ?? "null";
                            object valueValue = valueProperty.GetValue(value, null) ?? "null";
                            IHierarchy<object> keyValueNode = ReflectionUtility.GetObjectHierarchy(keyValue, 0);
                            keyValueNode.MemberReference = new DynamicMethod("Key", keyValueNode.InstanceType, null);
                            IHierarchy<object> valueValueNode = ReflectionUtility.GetObjectHierarchy(valueValue, 0);
                            valueValueNode.MemberReference = new DynamicMethod("Value", valueValueNode.InstanceType, null);
                            WriteXml(writer, keyValueNode, true);
                            WriteXml(writer, valueValueNode, true);
                            writer.WriteEndElement();
                        }
                        else
                        {
                            IHierarchy<object> itemNode = ReflectionUtility.GetObjectHierarchy(value, 0);
                            XmlQualifiedEntity qualifiedEntity = null;

                            if (currentType.IsGenericType && !hasWrapperAttribute)
                            {
                                Type[] genericParameters = currentType.GetGenericArguments();
                                if (genericParameters.Length > 0)
                                {
                                    qualifiedEntity = new XmlQualifiedEntity(ConvertUtility.ToDelimitedString(genericParameters, "And", XmlStartNameConverter));
                                }
                            }

                            if (isComplexType)
                            {
                                qualifiedEntity = GetXmlStartElementQualifiedName(itemNode, qualifiedEntity);
                                writer.WriteStartElement(qualifiedEntity.Prefix, qualifiedEntity.LocalName, qualifiedEntity.Namespace);
                            }
                            WriteXml(writer, itemNode, true);
                            if (isComplexType) { writer.WriteEndElement(); }
                        }
                    }
                    if (!skipStartElement && !current.HasChildren && !hasWrapperAttribute) { writer.WriteEndElement(); }
                }
            }
        }

        private static string XmlStartNameConverter(Type type)
        {
            return TypeUtility.SanitizeTypeName(type);
        }
    }
}