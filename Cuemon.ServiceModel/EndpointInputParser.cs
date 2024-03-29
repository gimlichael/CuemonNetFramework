﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Xml;
using Cuemon.Collections;
using Cuemon.Collections.Generic;
using Cuemon.Data.XmlClient;
using Cuemon.Reflection;
using Cuemon.Xml;

namespace Cuemon.ServiceModel
{
    /// <summary>
    /// Provides methods and properties for parsing HTTP endpoints. This class cannot be inherited.
    /// </summary>
    public sealed class EndpointInputParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointInputParser"/> class.
        /// </summary>
        /// <param name="endpointParameterTypes">A sequence of <see cref="KeyValuePair{TKey,TValue}"/> that represents the endpoint parameter types.</param>
        /// <param name="entityBody">A <see cref="Stream"/> object representing the contents of the incoming HTTP content body.</param>
        /// <param name="entityContentType">A <see cref="System.Net.Mime.ContentType"/> object representing the MIME type of the <paramref name="entityBody"/>.</param>
        /// <param name="entityEncoding">An <see cref="Encoding"/> object representing the encoding of the <paramref name="entityBody"/>.</param>
        public EndpointInputParser(IEnumerable<KeyValuePair<string, Type>> endpointParameterTypes, Stream entityBody, ContentType entityContentType, Encoding entityEncoding)
        {
            Validator.ThrowIfNull(endpointParameterTypes, nameof(endpointParameterTypes));
            Validator.ThrowIfNull(entityBody, nameof(entityBody));
            Validator.ThrowIfNull(entityContentType, nameof(entityContentType));
            Validator.ThrowIfNull(entityEncoding, nameof(entityEncoding));

            EndpointParameterTypes = endpointParameterTypes;
            Body = entityBody;
            ContentType = entityContentType;
            Encoding = entityEncoding;
        }

        private Stream Body { get; set; }

        private ContentType ContentType { get; set; }

        private Encoding Encoding { get; set; }

        private IEnumerable<KeyValuePair<string, Type>> EndpointParameterTypes { get; set; }

        /// <summary>
        /// Converts the input parameters of this instance to its equivalent <see cref="DataPairCollection"/>.
        /// </summary>
        /// <returns>A <see cref="DataPairCollection"/> equivalent to the input parameters of this instance.</returns>
        public DataPairCollection Parse()
        {
            return Parse(DefaultEntityBodyDeserializer);
        }

        /// <summary>
        /// Converts the input parameters of this instance to its equivalent <see cref="DataPairCollection"/>.
        /// </summary>
        /// <param name="entityBodyDeserializer">The function delegate that will handle the deserialization of this instance.</param>
        /// <returns>A <see cref="DataPairCollection"/> equivalent to the input parameters of this instance.</returns>
        public DataPairCollection Parse(Doer<EndpointInputParser, DataPairCollection> entityBodyDeserializer)
        {
            Validator.ThrowIfNull(entityBodyDeserializer, nameof(entityBodyDeserializer));
            return entityBodyDeserializer(this);
        }

        private static DataPairCollection DefaultEntityBodyDeserializer(EndpointInputParser parser)
        {
            try
            {
                Type entityBodyType = HttpMessageBody.Parse(parser.ContentType, parser.Encoding);
                string entityBodyTypeName = StringConverter.FromType(entityBodyType);
                switch (entityBodyTypeName)
                {
                    case "FormUrlEncodedMessageBody":
                        return FormUrlEncodedMessageBodyDeserializer(parser);
                    case "MultipartFormDataMessageBody":
                        return MultipartFormDataMessageBodyDeserializer(parser);
                    case "XmlMessageBody<TBody>":
                        return XmlMessageBodyDeserializer(parser, entityBodyType);
                    default:
                        throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
                            "Deserialization of '{0}' is not supported in this implementation. Consider using the overload taking a custom delegate of an entityBodyDeserializer.",
                                parser.ContentType));
                }
            }
            finally
            {
                if (parser != null && parser.Body != null)
                {
                    parser.Body.Dispose();
                }
            }
        }

        private static void ParseComplexTypes(DataPairCollection result, string endpointParameterName, Type endpointParameterType, Doer<string, bool> predicate, Doer<string, string> resolver)
        {
            ConstructorInfo ctor = endpointParameterType.GetConstructor(Type.EmptyTypes);
            if (ctor == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
"Unable to deserialize endpoint parameter '{0} ({1})' as no default constructor could be found.",
    endpointParameterName,
    StringConverter.FromType(endpointParameterType)));
            }

            object instance = ctor.Invoke(null);
            Type instanceType = instance.GetType();
            IEnumerable<PropertyInfo> properties = instanceType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (predicate(property.Name))
                {
                    string propertyValue = resolver(property.Name);
                    if (property.CanWrite)
                    {
                        if (property.PropertyType == typeof(byte[]))
                        {
                            byte[] propertyValueAsByteArray;
                            bool isBase64 = ByteConverter.TryFromBase64String(propertyValue, out propertyValueAsByteArray);
                            if (!isBase64) { continue; }
                            property.SetValue(instance, propertyValueAsByteArray, null);
                        }
                        else
                        {
                            property.SetValue(instance, ObjectConverter.FromString(propertyValue, CultureInfo.InvariantCulture), null);
                        }
                    }
                    result.Add(endpointParameterName, instance);
                }
            }
        }

        private static void ParseSimpleTypes(DataPairCollection result, string endpointParameterName, Type endpointParameterType, Doer<string, string> resolver)
        {
            string value = resolver(endpointParameterName);
            byte[] valueAsByteArray;
            bool isBase64 = ByteConverter.TryFromBase64String(value, out valueAsByteArray);
            result.Add(endpointParameterName, isBase64 ? valueAsByteArray : ObjectConverter.FromString(value, CultureInfo.InvariantCulture), endpointParameterType);
        }

        private static DataPairCollection FormUrlEncodedMessageBodyDeserializer(EndpointInputParser parser)
        {
            DataPairCollection result = new DataPairCollection();
            FormUrlEncodedMessageBody formUrlEncodedBody = new FormUrlEncodedMessageBody(parser.ContentType, parser.Encoding);
            NameValueCollection form = formUrlEncodedBody.Deserialize(parser.Body);
            foreach (KeyValuePair<string, Type> endpointParameterType in parser.EndpointParameterTypes)
            {
                if (TypeUtility.IsComplex(endpointParameterType.Value))
                {
                    ParseComplexTypes(result,
                        endpointParameterType.Key,
                        endpointParameterType.Value,
                        s => EnumerableUtility.Contains(EnumerableConverter.Cast<string>(form.Keys), s, StringComparer.OrdinalIgnoreCase),
                        s => form[s]);
                }
                else
                {
                    ParseSimpleTypes(result,
                        endpointParameterType.Key,
                        endpointParameterType.Value,
                        s => form[s]);
                }
            }
            return result;
        }

        private static DataPairCollection MultipartFormDataMessageBodyDeserializer(EndpointInputParser parser)
        {
            DataPairCollection result = new DataPairCollection();
            MultipartFormDataMessageBody multipartFormDataBody = new MultipartFormDataMessageBody(parser.ContentType, parser.Encoding);
            IList<HttpMultipartContent> dataParts = new List<HttpMultipartContent>(multipartFormDataBody.Deserialize(parser.Body));
            foreach (KeyValuePair<string, Type> endpointParameterType in parser.EndpointParameterTypes)
            {
                if (TypeUtility.IsComplex(endpointParameterType.Value))
                {
                    foreach (HttpMultipartContent dataPart in dataParts)
                    {
                        ParseComplexTypes(result,
                            endpointParameterType.Key,
                            endpointParameterType.Value,
                            s => dataPart.Name.Equals(s, StringComparison.OrdinalIgnoreCase),
                            s => dataPart.IsFile ? Convert.ToBase64String(dataPart.Data) : dataPart.GetPartAsString(parser.Encoding));
                    }
                }
                else
                {
                    foreach (HttpMultipartContent dataPart in dataParts)
                    {
                        if (dataPart.IsFormItem &&
                            dataPart.Name.Equals(endpointParameterType.Key, StringComparison.OrdinalIgnoreCase))
                        {
                            ParseSimpleTypes(result,
                                endpointParameterType.Key,
                                endpointParameterType.Value,
                                s => dataPart.GetPartAsString(parser.Encoding));
                        }
                    }
                }
            }
            return result;
        }

        private static DataPairCollection XmlMessageBodyDeserializer(EndpointInputParser parser, Type entityBodyType)
        {
            DataPairCollection result = new DataPairCollection();
            foreach (KeyValuePair<string, Type> endpointParameterType in parser.EndpointParameterTypes)
            {
                if (TypeUtility.IsComplex(endpointParameterType.Value))
                {
                    Type genericEntityBodyType = entityBodyType.MakeGenericType(endpointParameterType.Value);
                    object instance = Activator.CreateInstance(genericEntityBodyType, parser.ContentType, parser.Encoding);
                    MethodInfo deserialize = ReflectionUtility.GetMethod(instance.GetType(), "Deserialize", new Type[] { typeof(Stream) });
                    if (deserialize != null)
                    {
                        result.Add(endpointParameterType.Key, deserialize.Invoke(instance, new object[] { parser.Body }));
                    }
                }
                else
                {
                    XmlReader reader = null;
                    XmlDataReader dataReader = null;
                    try
                    {
                        reader = XmlUtility.CreateXmlReader(parser.Body, parser.Encoding);
                        dataReader = new XmlDataReader(reader);
                        while (dataReader.Read())
                        {
                            if (dataReader.Contains(endpointParameterType.Key))
                            {
                                var value = dataReader[endpointParameterType.Key].ToString();
                                ParseSimpleTypes(result,
                                    endpointParameterType.Key,
                                    endpointParameterType.Value,
                                    s => value);
                            }
                        }
                    }
                    finally
                    {
                        if (dataReader != null) { dataReader.Dispose(); }
                        if (reader != null) { reader.Close(); }
                    }
                }
            }
            return result;
        }
    }
}