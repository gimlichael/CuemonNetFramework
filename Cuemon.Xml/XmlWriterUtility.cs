using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Cuemon.Xml
{
    /// <summary>
    /// This utility class is designed to make <see cref="XmlWriter"/> related operations easier to work with.
    /// </summary>
    public static class XmlWriterUtility
    {
        /// <summary>
        /// Copies everything from the specified <paramref name="reader"/> and returns the result as an XML stream.
        /// </summary>
        /// <param name="reader">The <see cref="XmlReader"/> object that contains the XML data.</param>
        /// <param name="setup">The <see cref="XmlCopyOptions"/> which need to be configured.</param>
        /// <returns>A <see cref="Stream"/> holding an exact copy of the source <paramref name="reader"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="reader"/> is null.
        /// </exception>
        public static Stream Copy(XmlReader reader, Act<XmlCopyOptions> setup = null)
        {
            return Copy(reader, DefaultCopier, setup);
        }

        /// <summary>
        /// Copies the specified <paramref name="reader"/> using the specified delegate <paramref name="copier"/> and returns the result as an XML stream.
        /// </summary>
        /// <param name="reader">The <see cref="XmlReader"/> object that contains the XML data.</param>
        /// <param name="copier">The delegate that will create an in-memory copy of <paramref name="reader"/> as a XML stream.</param>
        /// <param name="setup">The <see cref="XmlCopyOptions"/> which need to be configured.</param>
        /// <returns>A <see cref="Stream"/> holding the XML copied by the delegate <paramref name="copier"/> from the source <paramref name="reader"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="reader"/> is null - or - <paramref name="copier"/> is null.
        /// </exception>
        public static Stream Copy(XmlReader reader, Act<XmlWriter, XmlReader> copier, Act<XmlCopyOptions> setup = null)
        {
            Validator.ThrowIfNull(reader, nameof(reader));
            Validator.ThrowIfNull(copier, nameof(copier));
            var options = DelegateUtility.ConfigureAction(setup);
            try
            {
                return CreateXml(copier, reader, options.WriterSettings);
            }
            finally
            {
                if (!options.LeaveStreamOpen) { reader.Close(); }
            }
        }

        /// <summary>
        /// Copies the specified <paramref name="reader"/> using the specified delegate <paramref name="copier"/> and returns the result as an XML stream.
        /// </summary>
        /// <typeparam name="T">The type of the parameter of the delegate <paramref name="copier"/>.</typeparam>
        /// <param name="reader">The <see cref="XmlReader"/> object that contains the XML data.</param>
        /// <param name="copier">The delegate that will create an in-memory copy of <paramref name="reader"/> as a XML stream.</param>
        /// <param name="arg">The parameter of the delegate <paramref name="copier"/>.</param>
        /// <param name="setup">The <see cref="XmlCopyOptions"/> which need to be configured.</param>
        /// <returns>A <see cref="Stream"/> holding the XML copied by the delegate <paramref name="copier"/> from the source <paramref name="reader"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="reader"/> is null - or - <paramref name="copier"/> is null.
        /// </exception>
        public static Stream Copy<T>(XmlReader reader, Act<XmlWriter, XmlReader, T> copier, T arg, Act<XmlCopyOptions> setup = null)
        {
            Validator.ThrowIfNull(reader, nameof(reader));
            Validator.ThrowIfNull(copier, nameof(copier));
            var options = DelegateUtility.ConfigureAction(setup);
            try
            {
                return CreateXml(copier, reader, arg, options.WriterSettings);
            }
            finally
            {
                if (!options.LeaveStreamOpen) { reader.Close(); }
            }
        }

        /// <summary>
        /// Copies the specified <paramref name="reader"/> using the specified delegate <paramref name="copier"/> and returns the result as an XML stream.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="copier"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="copier"/>.</typeparam>
        /// <param name="reader">The <see cref="XmlReader"/> object that contains the XML data.</param>
        /// <param name="copier">The delegate that will create an in-memory copy of <paramref name="reader"/> as a XML stream.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="copier"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="copier"/>.</param>
        /// <param name="setup">The <see cref="XmlCopyOptions"/> which need to be configured.</param>
        /// <returns>A <see cref="Stream"/> holding the XML copied by the delegate <paramref name="copier"/> from the source <paramref name="reader"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="reader"/> is null - or - <paramref name="copier"/> is null.
        /// </exception>
        public static Stream Copy<T1, T2>(XmlReader reader, Act<XmlWriter, XmlReader, T1, T2> copier, T1 arg1, T2 arg2, Act<XmlCopyOptions> setup = null)
        {
            Validator.ThrowIfNull(reader, nameof(reader));
            Validator.ThrowIfNull(copier, nameof(copier));
            var options = DelegateUtility.ConfigureAction(setup);
            try
            {
                return CreateXml(copier, reader, arg1, arg2, options.WriterSettings);
            }
            finally
            {
                if (!options.LeaveStreamOpen) { reader.Close(); }
            }
        }

        /// <summary>
        /// Copies the specified <paramref name="reader"/> using the specified delegate <paramref name="copier"/> and returns the result as an XML stream.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="copier"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="copier"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="copier"/>.</typeparam>
        /// <param name="reader">The <see cref="XmlReader"/> object that contains the XML data.</param>
        /// <param name="copier">The delegate that will create an in-memory copy of <paramref name="reader"/> as a XML stream.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="copier"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="copier"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="copier"/>.</param>
        /// <param name="setup">The <see cref="XmlCopyOptions"/> which need to be configured.</param>
        /// <returns>A <see cref="Stream"/> holding the XML copied by the delegate <paramref name="copier"/> from the source <paramref name="reader"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="reader"/> is null - or - <paramref name="copier"/> is null.
        /// </exception>
        public static Stream Copy<T1, T2, T3>(XmlReader reader, Act<XmlWriter, XmlReader, T1, T2, T3> copier, T1 arg1, T2 arg2, T3 arg3, Act<XmlCopyOptions> setup = null)
        {
            Validator.ThrowIfNull(reader, nameof(reader));
            Validator.ThrowIfNull(copier, nameof(copier));
            var options = DelegateUtility.ConfigureAction(setup);
            try
            {
                return CreateXml(copier, reader, arg1, arg2, arg3, options.WriterSettings);
            }
            finally
            {
                if (!options.LeaveStreamOpen) { reader.Close(); }
            }
        }

        /// <summary>
        /// Copies the specified <paramref name="reader"/> using the specified delegate <paramref name="copier"/> and returns the result as an XML stream.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="copier"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="copier"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="copier"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="copier"/>.</typeparam>
        /// <param name="reader">The <see cref="XmlReader"/> object that contains the XML data.</param>
        /// <param name="copier">The delegate that will create an in-memory copy of <paramref name="reader"/> as a XML stream.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="copier"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="copier"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="copier"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="copier"/>.</param>
        /// <param name="setup">The <see cref="XmlCopyOptions"/> which need to be configured.</param>
        /// <returns>A <see cref="Stream"/> holding the XML copied by the delegate <paramref name="copier"/> from the source <paramref name="reader"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="reader"/> is null - or - <paramref name="copier"/> is null.
        /// </exception>
        public static Stream Copy<T1, T2, T3, T4>(XmlReader reader, Act<XmlWriter, XmlReader, T1, T2, T3, T4> copier, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Act<XmlCopyOptions> setup = null)
        {
            Validator.ThrowIfNull(reader, nameof(reader));
            Validator.ThrowIfNull(copier, nameof(copier));
            var options = DelegateUtility.ConfigureAction(setup);
            try
            {
                return CreateXml(copier, reader, arg1, arg2, arg3, arg4, options.WriterSettings);
            }
            finally
            {
                if (!options.LeaveStreamOpen) { reader.Close(); }
            }
        }

        /// <summary>
        /// Copies the specified <paramref name="reader"/> using the specified delegate <paramref name="copier"/> and returns the result as an XML stream.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="copier"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="copier"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="copier"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="copier"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="copier"/>.</typeparam>
        /// <param name="reader">The <see cref="XmlReader"/> object that contains the XML data.</param>
        /// <param name="copier">The delegate that will create an in-memory copy of <paramref name="reader"/> as a XML stream.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="copier"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="copier"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="copier"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="copier"/>.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="copier"/>.</param>
        /// <param name="setup">The <see cref="XmlCopyOptions"/> which need to be configured.</param>
        /// <returns>A <see cref="Stream"/> holding the XML copied by the delegate <paramref name="copier"/> from the source <paramref name="reader"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="reader"/> is null - or - <paramref name="copier"/> is null.
        /// </exception>
        public static Stream Copy<T1, T2, T3, T4, T5>(XmlReader reader, Act<XmlWriter, XmlReader, T1, T2, T3, T4, T5> copier, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Act<XmlCopyOptions> setup = null)
        {
            Validator.ThrowIfNull(reader, nameof(reader));
            Validator.ThrowIfNull(copier, nameof(copier));
            var options = DelegateUtility.ConfigureAction(setup);
            try
            {
                return CreateXml(copier, reader, arg1, arg2, arg3, arg4, arg5, options.WriterSettings);
            }
            finally
            {
                if (!options.LeaveStreamOpen) { reader.Close(); }
            }
        }


        private static void DefaultCopier(XmlWriter writer, XmlReader reader)
        {
            if (reader == null) { throw new ArgumentNullException(nameof(reader)); }
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.CDATA:
                        writer.WriteCData(reader.Value);
                        break;
                    case XmlNodeType.Comment:
                        writer.WriteComment(reader.Value);
                        break;
                    case XmlNodeType.DocumentType:
                        writer.WriteDocType(reader.Name, reader.GetAttribute("PUBLIC"), reader.GetAttribute("SYSTEM"), reader.Value);
                        break;
                    case XmlNodeType.Element:
                        writer.WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
                        writer.WriteAttributes(reader, true);
                        if (reader.IsEmptyElement) { writer.WriteEndElement(); }
                        break;
                    case XmlNodeType.EndElement:
                        writer.WriteFullEndElement();
                        break;
                    case XmlNodeType.Attribute:
                    case XmlNodeType.Document:
                    case XmlNodeType.DocumentFragment:
                    case XmlNodeType.EndEntity:
                    case XmlNodeType.None:
                    case XmlNodeType.Notation:
                    case XmlNodeType.Entity:
                        break;
                    case XmlNodeType.EntityReference:
                        writer.WriteEntityRef(reader.Name);
                        break;
                    case XmlNodeType.Whitespace:
                    case XmlNodeType.SignificantWhitespace:
                        writer.WriteWhitespace(reader.Value);
                        break;
                    case XmlNodeType.Text:
                        writer.WriteString(reader.Value);
                        break;
                    case XmlNodeType.ProcessingInstruction:
                    case XmlNodeType.XmlDeclaration:
                        writer.WriteProcessingInstruction(reader.Name, reader.Value);
                        break;
                }
            }
        }

        /// <summary>
        /// Specifies a set of features to support the <see cref="XmlWriter"/> object.
        /// </summary>
        /// <returns>A <see cref="XmlWriterSettings"/> instance that specifies a set of features to support the <see cref="XmlWriter"/> object.</returns>
        /// <remarks>
        /// The following table shows the initial property values for an instance of <see cref="XmlWriterSettings"/>.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Property</term>
        ///         <description>Initial Value</description>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="XmlWriterSettings.CheckCharacters"/></term>
        ///         <description><c>true</c></description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="XmlWriterSettings.CloseOutput"/></term>
        ///         <description><c>false</c></description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="XmlWriterSettings.ConformanceLevel"/></term>
        ///         <description><see cref="ConformanceLevel.Document"/></description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="XmlWriterSettings.Encoding"/></term>
        ///         <description><see cref="Encoding.UTF8"/></description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="XmlWriterSettings.Indent"/></term>
        ///         <description><c>false</c></description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="XmlWriterSettings.IndentChars"/></term>
        ///         <description><see cref="StringUtility.Tab"/></description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="XmlWriterSettings.NewLineChars"/></term>
        ///         <description><see cref="StringUtility.NewLine"/></description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="XmlWriterSettings.NewLineHandling"/></term>
        ///         <description><see cref="NewLineHandling.None"/></description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="XmlWriterSettings.NewLineOnAttributes"/></term>
        ///         <description><c>false</c></description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="XmlWriterSettings.OmitXmlDeclaration"/></term>
        ///         <description><c>false</c></description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="XmlWriterSettings.NewLineOnAttributes"/></term>
        ///         <description><c>false</c></description>
        ///     </item>
        /// </list>
        /// </remarks>
        public static XmlWriterSettings CreateSettings(Act<XmlWriterSettings> setup = null)
        {
            var settings = new XmlWriterSettings();
            OverrideDefaultSettings(settings);
            setup?.Invoke(settings);
            return settings;
        }

        private static void OverrideDefaultSettings(XmlWriterSettings settings)
        {
            settings.CheckCharacters = true;
            settings.CloseOutput = false;
            settings.ConformanceLevel = ConformanceLevel.Document;
            settings.Encoding = Encoding.UTF8;
            settings.Indent = false;
            settings.IndentChars = StringUtility.Tab;
            settings.NewLineChars = StringUtility.NewLine;
            settings.NewLineHandling = NewLineHandling.None;
            settings.NewLineOnAttributes = false;
            settings.OmitXmlDeclaration = false;
        }

        /// <summary>
        /// Creates and returns a XML stream by the specified delegate <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">The delegate that will create an in-memory XML stream.</param>
        /// <param name="setup">The <see cref="XmlWriterSettings"/> which need to be configured.</param>
        /// <returns>A <see cref="Stream"/> holding the XML created by the delegate <paramref name="writer"/>.</returns>
        public static Stream CreateXml(Act<XmlWriter> writer, Act<XmlWriterSettings> setup = null)
        {
            var factory = ActFactory.Create(writer, null);
            return CreateXmlCore(factory, setup, MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Creates and returns a XML stream by the specified delegate <paramref name="writer"/>.
        /// </summary>
        /// <typeparam name="T">The type of the parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <param name="writer">The delegate that will create an in-memory XML stream.</param>
        /// <param name="arg">The parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="setup">The <see cref="XmlWriterSettings"/> which need to be configured.</param>
        /// <returns>A <see cref="Stream"/> holding the XML created by the delegate <paramref name="writer"/>.</returns>
        public static Stream CreateXml<T>(Act<XmlWriter, T> writer, T arg, Act<XmlWriterSettings> setup = null)
        {
            var factory = ActFactory.Create(writer, null, arg);
            return CreateXmlCore(factory, setup, MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Creates and returns a XML stream by the specified delegate <paramref name="writer"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <param name="writer">The delegate that will create an in-memory XML stream.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="setup">The <see cref="XmlWriterSettings"/> which need to be configured.</param>
        /// <returns>A <see cref="Stream"/> holding the XML created by the delegate <paramref name="writer"/>.</returns>
        public static Stream CreateXml<T1, T2>(Act<XmlWriter, T1, T2> writer, T1 arg1, T2 arg2, Act<XmlWriterSettings> setup = null)
        {
            var factory = ActFactory.Create(writer, null, arg1, arg2);
            return CreateXmlCore(factory, setup, MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Creates and returns a XML stream by the specified delegate <paramref name="writer"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <param name="writer">The delegate that will create an in-memory XML stream.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="setup">The <see cref="XmlWriterSettings"/> which need to be configured.</param>
        /// <returns>A <see cref="Stream"/> holding the XML created by the delegate <paramref name="writer"/>.</returns>
        public static Stream CreateXml<T1, T2, T3>(Act<XmlWriter, T1, T2, T3> writer, T1 arg1, T2 arg2, T3 arg3, Act<XmlWriterSettings> setup = null)
        {
            var factory = ActFactory.Create(writer, null, arg1, arg2, arg3);
            return CreateXmlCore(factory, setup, MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Creates and returns a XML stream by the specified delegate <paramref name="writer"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <param name="writer">The delegate that will create an in-memory XML stream.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="setup">The <see cref="XmlWriterSettings"/> which need to be configured.</param>
        /// <returns>A <see cref="Stream"/> holding the XML created by the delegate <paramref name="writer"/>.</returns>
        public static Stream CreateXml<T1, T2, T3, T4>(Act<XmlWriter, T1, T2, T3, T4> writer, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Act<XmlWriterSettings> setup = null)
        {
            var factory = ActFactory.Create(writer, null, arg1, arg2, arg3, arg4);
            return CreateXmlCore(factory, setup, MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Creates and returns a XML stream by the specified delegate <paramref name="writer"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <param name="writer">The delegate that will create an in-memory XML stream.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="setup">The <see cref="XmlWriterSettings"/> which need to be configured.</param>
        /// <returns>A <see cref="Stream"/> holding the XML created by the delegate <paramref name="writer"/>.</returns>
        public static Stream CreateXml<T1, T2, T3, T4, T5>(Act<XmlWriter, T1, T2, T3, T4, T5> writer, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Act<XmlWriterSettings> setup = null)
        {
            var factory = ActFactory.Create(writer, null, arg1, arg2, arg3, arg4, arg5);
            return CreateXmlCore(factory, setup, MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Creates and returns a XML stream by the specified delegate <paramref name="writer"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <param name="writer">The delegate that will create an in-memory XML stream.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="setup">The <see cref="XmlWriterSettings"/> which need to be configured.</param>
        /// <returns>A <see cref="Stream"/> holding the XML created by the delegate <paramref name="writer"/>.</returns>
        public static Stream CreateXml<T1, T2, T3, T4, T5, T6>(Act<XmlWriter, T1, T2, T3, T4, T5, T6> writer, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, Act<XmlWriterSettings> setup = null)
        {
            var factory = ActFactory.Create(writer, null, arg1, arg2, arg3, arg4, arg5, arg6);
            return CreateXmlCore(factory, setup, MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Creates and returns a XML stream by the specified delegate <paramref name="writer"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <param name="writer">The delegate that will create an in-memory XML stream.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="setup">The <see cref="XmlWriterSettings"/> which need to be configured.</param>
        /// <returns>A <see cref="Stream"/> holding the XML created by the delegate <paramref name="writer"/>.</returns>
        public static Stream CreateXml<T1, T2, T3, T4, T5, T6, T7>(Act<XmlWriter, T1, T2, T3, T4, T5, T6, T7> writer, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, Act<XmlWriterSettings> setup = null)
        {
            var factory = ActFactory.Create(writer, null, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            return CreateXmlCore(factory, setup, MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Creates and returns a XML stream by the specified delegate <paramref name="writer"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <param name="writer">The delegate that will create an in-memory XML stream.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg8">The eighth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="setup">The <see cref="XmlWriterSettings"/> which need to be configured.</param>
        /// <returns>A <see cref="Stream"/> holding the XML created by the delegate <paramref name="writer"/>.</returns>
        public static Stream CreateXml<T1, T2, T3, T4, T5, T6, T7, T8>(Act<XmlWriter, T1, T2, T3, T4, T5, T6, T7, T8> writer, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, Act<XmlWriterSettings> setup = null)
        {
            var factory = ActFactory.Create(writer, null, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            return CreateXmlCore(factory, setup, MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Creates and returns a XML stream by the specified delegate <paramref name="writer"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <param name="writer">The delegate that will create an in-memory XML stream.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg8">The eighth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg9">The ninth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="setup">The <see cref="XmlWriterSettings"/> which need to be configured.</param>
        /// <returns>A <see cref="Stream"/> holding the XML created by the delegate <paramref name="writer"/>.</returns>
        public static Stream CreateXml<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Act<XmlWriter, T1, T2, T3, T4, T5, T6, T7, T8, T9> writer, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, Act<XmlWriterSettings> setup = null)
        {
            var factory = ActFactory.Create(writer, null, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
            return CreateXmlCore(factory, setup, MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Creates and returns a XML stream by the specified delegate <paramref name="writer"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <param name="writer">The delegate that will create an in-memory XML stream.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg8">The eighth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg9">The ninth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg10">The tenth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="setup">The <see cref="XmlWriterSettings"/> which need to be configured.</param>
        /// <returns>A <see cref="Stream"/> holding the XML created by the delegate <paramref name="writer"/>.</returns>
        public static Stream CreateXml<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Act<XmlWriter, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> writer, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, Act<XmlWriterSettings> setup = null)
        {
            var factory = ActFactory.Create(writer, null, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
            return CreateXmlCore(factory, setup, MethodBase.GetCurrentMethod());
        }

        private static Stream CreateXmlCore<TTuple>(ActFactory<TTuple> factory, Act<XmlWriterSettings> setup, MethodBase caller) where TTuple : Template<XmlWriter>
        {
            var options = DelegateUtility.ConfigureAction(setup, OverrideDefaultSettings);
            Stream output;
            MemoryStream tempOutput = null;
            try
            {
                tempOutput = new MemoryStream();
                using (XmlWriter writer = XmlWriter.Create(tempOutput, options))
                {
                    factory.GenericArguments.Arg1 = writer;
                    factory.ExecuteMethod();
                    writer.Flush();
                }
                tempOutput.Flush();
                tempOutput.Position = 0;
                output = tempOutput;
                tempOutput = null;
            }
            catch (Exception ex)
            {
                List<object> parameters = new List<object>();
                parameters.AddRange(factory.GenericArguments.ToArray());
                parameters.Add(options);
                throw ExceptionUtility.Refine(new InvalidOperationException("There is an error in the XML document.", ex), caller, parameters.ToArray());
            }
            finally
            {
                tempOutput?.Dispose();
            }
            return output;
        }
    }
}