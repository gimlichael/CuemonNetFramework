using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Cuemon.Collections.Generic;
using Cuemon.Runtime.Serialization;
using Cuemon.Xml.XPath;

namespace Cuemon.Xml
{
    /// <summary>
    /// This utility class is designed to make XML based convert operations easier to work with.
    /// </summary>
    public static class XmlConvertUtility
    {
        /// <summary>
        /// Converts the specified <paramref name="exception"/> to an <see cref="XmlElement"/>.
        /// </summary>
        /// <param name="exception">The <see cref="Exception"/> to convert into an <see cref="XmlElement"/>.</param>
        /// <returns>An <see cref="XmlElement"/> variant of the specified <paramref name="exception"/>.</returns>
        public static XmlElement ToXmlElement(Exception exception)
        {
            return ToXmlElement(exception, Encoding.Unicode);
        }

        /// <summary>
        /// Converts the specified <paramref name="exception"/> to an <see cref="XmlElement"/>.
        /// </summary>
        /// <param name="exception">The <see cref="Exception"/> to convert into an <see cref="XmlElement"/>.</param>
        /// <param name="encoding">The preferred encoding to apply to the result.</param>
        /// <returns>An <see cref="XmlElement"/> variant of the specified <paramref name="exception"/>.</returns>
        public static XmlElement ToXmlElement(Exception exception, Encoding encoding)
        {
            return ToXmlElement(exception, encoding, false);
        }

        /// <summary>
        /// Converts the specified <paramref name="exception"/> to an <see cref="XmlElement"/>.
        /// </summary>
        /// <param name="exception">The <see cref="Exception"/> to convert into an <see cref="XmlElement"/>.</param>
        /// <param name="encoding">The preferred encoding to apply to the result.</param>
        /// <param name="includeStackTrace">if set to <c>true</c> the stack trace of the exception (and possible user data) is included in the converted result.</param>
        /// <returns>An <see cref="XmlElement"/> variant of the specified <paramref name="exception"/>.</returns>
        public static XmlElement ToXmlElement(Exception exception, Encoding encoding, bool includeStackTrace)
        {
            using (Stream output = ToStream(exception, encoding, includeStackTrace))
            {
                XmlDocument document = new XmlDocument();
                document.Load(output);
                return document.DocumentElement;
            }
        }

        /// <summary>
        /// Converts the specified <paramref name="exception"/> to an XML <see cref="Stream"/>.
        /// </summary>
        /// <param name="exception">The <see cref="Exception"/> to convert into an XML <see cref="Stream"/>.</param>
        /// <returns>An XML <see cref="Stream"/> variant of the specified <paramref name="exception"/>.</returns>
        /// <remarks>The converted <paramref name="exception"/> defaults to using an instance of <see cref="UTF8Encoding"/> unless specified otherwise.</remarks>
        public static Stream ToStream(Exception exception)
        {
            return ToStream(exception, Encoding.UTF8);
        }

        /// <summary>
        /// Converts the specified <paramref name="exception"/> to an XML <see cref="Stream"/>.
        /// </summary>
        /// <param name="exception">The <see cref="Exception"/> to convert into an XML <see cref="Stream"/>.</param>
        /// <param name="encoding">The preferred encoding to apply to the result.</param>
        /// <returns>An XML <see cref="Stream"/> variant of the specified <paramref name="exception"/>.</returns>
        public static Stream ToStream(Exception exception, Encoding encoding)
        {
            return ToStream(exception, encoding, false);
        }

        /// <summary>
        /// Converts the specified <paramref name="exception"/> to an XML <see cref="Stream"/>.
        /// </summary>
        /// <param name="exception">The <see cref="Exception"/> to convert into an XML <see cref="Stream"/>.</param>
        /// <param name="encoding">The preferred encoding to apply to the result.</param>
        /// <param name="includeStackTrace">if set to <c>true</c> the stack trace of the exception (and possible user data) is included in the converted result.</param>
        /// <returns>An XML <see cref="Stream"/> variant of the specified <paramref name="exception"/>.</returns>
        public static Stream ToStream(Exception exception, Encoding encoding, bool includeStackTrace)
        {
            if (exception == null) { throw new ArgumentNullException(nameof(exception)); }
            if (encoding == null) { throw new ArgumentNullException(nameof(encoding)); }
            MemoryStream tempOutput = null;
            MemoryStream output;
            try
            {
                tempOutput = new MemoryStream();
                using (XmlWriter writer = XmlWriter.Create(tempOutput, XmlWriterUtility.CreateSettings(encoding)))
                {
                    WriteException(writer, exception, includeStackTrace);
                    writer.Flush();
                    tempOutput.Position = 0;
                    output = tempOutput;
                    tempOutput = null;
                }
            }
            finally
            {
                if (tempOutput != null) { tempOutput.Dispose(); }
            }
            output.Position = 0;
            return output;
        }

        private static void WriteException(XmlWriter writer, Exception exception, bool includeStackTrace)
        {
            Type exceptionType = exception.GetType();
            writer.WriteStartElement(XmlUtility.SanitizeElementName(exceptionType.Name));
            writer.WriteAttributeString("namespace", exceptionType.Namespace);
            WriteExceptionCore(writer, exception, includeStackTrace);
            writer.WriteEndElement();
        }

        private static void WriteEndElement<T>(T counter, XmlWriter writer)
        {
            writer.WriteEndElement();
        }

        private static void WriteExceptionCore(XmlWriter writer, Exception exception, bool includeStackTrace)
        {
            if (!string.IsNullOrEmpty(exception.Source))
            {
                writer.WriteElementString("Source", exception.Source);
            }

            if (!string.IsNullOrEmpty(exception.Message))
            {
                writer.WriteElementString("Message", exception.Message);
            }

            if (exception.StackTrace != null && includeStackTrace)
            {
                writer.WriteStartElement("StackTrace");
                string[] lines = exception.StackTrace.Split(new[] { StringUtility.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in lines)
                {
                    writer.WriteElementString("Frame", line.Trim());
                }
                writer.WriteEndElement();
            }

            if (exception.Data.Count > 0)
            {
                writer.WriteStartElement("Data");
                foreach (DictionaryEntry entry in exception.Data)
                {
                    writer.WriteStartElement(XmlUtility.SanitizeElementName(entry.Key.ToString()));
                    writer.WriteString(XmlUtility.SanitizeElementText(entry.Value.ToString()));
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }

            WriteInnerExceptions(writer, exception, includeStackTrace);
        }

        private static void WriteInnerExceptions(XmlWriter writer, Exception exception, bool includeStackTrace)
        {
            var innerExceptions = new List<Exception>(ExceptionUtility.Flatten(exception));
            if (innerExceptions.Count > 0)
            {
                int endElementsToWrite = 0;
                foreach (var inner in innerExceptions)
                {
                    Type exceptionType = inner.GetType();
                    writer.WriteStartElement(XmlUtility.SanitizeElementName(exceptionType.Name));
                    writer.WriteAttributeString("namespace", exceptionType.Namespace);
                    WriteExceptionCore(writer, inner, includeStackTrace);
                    endElementsToWrite++;
                }
                LoopUtility.For(endElementsToWrite, WriteEndElement, writer);
            }
        }

        /// <summary>
        /// Returns a UTF-8 encoded JSON representation of the specified XML stream and whose value is equivalent to the specified XML stream.
        /// </summary>
        /// <param name="xmlValue">The XML to convert to a JSON representation.</param>
        /// <returns>A UTF-8 encoded JSON representation of the specified <paramref name="xmlValue"/> and whose value is equivalent to <paramref name="xmlValue"/>.</returns>
        /// <remarks>The JSON representation is in compliance with RFC 4627. Take note, that all string values is escaped using <see cref="JsonConverter.Escape"/>. This is by design and to help ensure compatibility with a wide range of data.</remarks>
        public static Stream ToJson(Stream xmlValue)
        {
            return ToJson(xmlValue, Encoding.UTF8);
        }

        /// <summary>
        /// Returns a JSON representation of the specified XML stream and whose value is equivalent to the specified XML stream.
        /// </summary>
        /// <param name="xmlValue">The XML to convert to a JSON representation.</param>
        /// <param name="encoding">The text encoding to use.</param>
        /// <returns>A JSON representation of the specified <paramref name="xmlValue"/> and whose value is equivalent to <paramref name="xmlValue"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">This exception is thrown when <paramref name="encoding"/> is not within the boundaries of RFC 4627.</exception>
        /// <exception cref="ArgumentNullException">This exception is thrown should either of <paramref name="xmlValue"/> or <paramref name="encoding"/> have the value of null.</exception>
        /// <remarks>The JSON representation is in compliance with RFC 4627. Take note, that all string values is escaped using <see cref="JsonConverter.Escape"/>. This is by design and to help ensure compatibility with a wide range of data.</remarks>
        public static Stream ToJson(Stream xmlValue, Encoding encoding)
        {
            if (xmlValue == null) throw new ArgumentNullException(nameof(xmlValue));
            if (encoding == null) { throw new ArgumentNullException(nameof(encoding)); }
            JsonWriter.ValidateEncoding(encoding);

            long startingPosition = xmlValue.Position;
            if (xmlValue.CanSeek) { xmlValue.Position = 0; }

            MemoryStream output = null;
            MemoryStream tempOutput = null;
            try
            {
                tempOutput = new MemoryStream();
                using (JsonWriter writer = JsonWriter.Create(tempOutput, encoding))
                {
                    IXPathNavigable navigable = XPathUtility.CreateXPathNavigableDocument(xmlValue);
                    XPathNavigator rootNavigator = navigable.CreateNavigator();
                    XPathNodeIterator rootIterator = rootNavigator.Select("/node()");

                    XmlJsonInstance instance = BuildJsonInstance(rootIterator);
                    writer.WriteStartObject();
                    WriteJsonInstance(writer, instance);
                    writer.WriteEndObject();

                    writer.Flush();
                    tempOutput.Position = 0;
                    output = new MemoryStream(tempOutput.ToArray());
                    tempOutput = null;
                }
            }
            catch (Exception)
            {
                if (output != null) { output.Dispose(); }
                throw;
            }
            finally
            {
                if (tempOutput != null) { tempOutput.Dispose(); }
            }

            if (xmlValue.CanSeek) { xmlValue.Seek(startingPosition, SeekOrigin.Begin); } // reset to original position
            return output;
        }

        private static void WriteJsonInstance(JsonWriter writer, XmlJsonInstance instance)
        {
            switch (instance.NodeType)
            {
                case XPathNodeType.Attribute:
                    writer.WriteObject(instance.Name, instance.Value);
                    break;
                case XPathNodeType.Element:
                    if (instance.IsPartOfArray() && instance.WriteStartArray())
                    {
                        writer.WriteObjectName(instance.Name);
                    }
                    else if (!instance.IsPartOfArray())
                    {
                        writer.WriteObjectName(instance.Name);
                    }
                    if (instance.WriteStartArray())
                    {
                        writer.WriteStartArray();
                        writer.WriteStartObject();
                    }
                    else
                    {
                        writer.WriteStartObject();
                    }
                    break;
                case XPathNodeType.Text:
                    writer.WriteObject(instance.Name, instance.Value);
                    break;
            }

            if (instance.WriteValueSeparator())
            {
                if ((!instance.WriteStartArray() &&
                    (!instance.IsPartOfArray() && instance.NodeType == XPathNodeType.Attribute)) ||
                    instance.NodeType == XPathNodeType.Text)
                {
                    writer.WriteValueSeperator();
                }
            }

            if (instance.Instances.Count > 0)
            {
                instance.Instances.Sort(JsonInstanceCollection.Compare);
                foreach (XmlJsonInstance childInstance in instance.Instances)
                {
                    WriteJsonInstance(writer, childInstance);
                }
            }

            switch (instance.NodeType)
            {
                case XPathNodeType.Attribute:
                    break;
                case XPathNodeType.Element:
                    if (instance.WriteEndArray())
                    {
                        writer.WriteEndObject();
                        writer.WriteEndArray();
                    }
                    else
                    {
                        writer.WriteEndObject();
                    }

                    if (instance.WriteValueSeparator()) { writer.WriteValueSeperator(); }
                    break;
            }
        }

        private static XmlJsonInstance BuildJsonInstance(XPathNodeIterator iterator)
        {
            int nodeNumber = 0;
            return BuildJsonInstance(iterator, null, ref nodeNumber);
        }

        private static XmlJsonInstance BuildJsonInstance(XPathNodeIterator iterator, JsonInstance parent, ref int nodeNumber)
        {
            while (iterator.MoveNext())
            {
                XmlJsonInstance instance = null;
                XPathNavigator navigator = iterator.Current;
                if (navigator == null) { continue; }
                switch (navigator.NodeType)
                {
                    case XPathNodeType.Root:
                    case XPathNodeType.Namespace:
                    case XPathNodeType.SignificantWhitespace:
                    case XPathNodeType.ProcessingInstruction:
                    case XPathNodeType.Whitespace:
                    case XPathNodeType.Text:
                        continue;
                    case XPathNodeType.Attribute:
                        XPathNodeIterator attributes = navigator.Select("@*");
                        while (attributes.MoveNext())
                        {
                            if (attributes.Current == null) { continue; }
                            XPathNavigator attribute = attributes.Current;
                            string value = attribute.Value.Trim();
                            JsonInstance child = new XmlJsonInstance(attribute.Name, ObjectConverter.FromString(value, CultureInfo.InvariantCulture), nodeNumber, XPathNodeType.Attribute);
                            child.Parent = instance;
                            instance.Instances.Add(child);
                        }
                        //if (parent != null) { instance.Parent = parent; }
                        //instances.Add(instance);
                        break;
                    case XPathNodeType.Element:
                        string elementName = navigator.Name;
                        bool directElement = (!navigator.HasAttributes && navigator.Select("child::node()[text()]").Count == 0);
                        instance = new XmlJsonInstance(elementName, null, nodeNumber, XPathNodeType.Element);

                        if ((navigator.SelectSingleNode("text()") == null)) // (navigator.IsEmptyElement || !navigator.HasChildren) && 
                        {
                        }
                        else
                        {
                            XPathNodeIterator textIterator = navigator.Select("text()");
                            string textValue = null;
                            int textValueSpaceCount = -1;
                            if (textIterator.Count > 0)
                            {
                                StringBuilder text = new StringBuilder();
                                while (textIterator.MoveNext())
                                {
                                    if (textIterator.Current == null) { continue; }
                                    text.Append(textIterator.Current.Value.Trim());
                                }
                                textValue = text.ToString();
                                textValueSpaceCount = StringUtility.Count(textValue, ' ');
                            }

                            if (directElement && textValueSpaceCount == 0)
                            {
                                instance = new XmlJsonInstance(elementName, ObjectConverter.FromString(textValue, CultureInfo.InvariantCulture), nodeNumber);
                            }
                            else
                            {
                                JsonInstance child = new XmlJsonInstance("#text", ObjectConverter.FromString(textValue, CultureInfo.InvariantCulture), nodeNumber);
                                child.Parent = instance;
                                instance.Instances.Add(child);
                            }
                        }

                        if (parent != null)
                        {
                            instance.Parent = parent;
                            parent.Instances.Add(instance);
                        }

                        nodeNumber++;

                        if (navigator.HasAttributes) { goto case XPathNodeType.Attribute; }
                        break;
                }

                XPathNodeIterator children = navigator.Select("node()[not(self::text())]");
                if (children.Count > 0)
                {
                    BuildJsonInstance(children, instance, ref nodeNumber);
                }

                if (parent == null) { return instance; }
            }
            return null;
        }
    }
}