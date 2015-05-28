using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using Cuemon.Collections.Generic;
using Cuemon.Reflection;
using Cuemon.Web;

namespace Cuemon.ServiceModel
{
    /// <summary>
    /// Provides helper methods for the <see cref="HttpMessageBody{TBody}"/> class.
    /// </summary>
    public static class HttpMessageBody
    {
        /// <summary>
        /// Parses all <see cref="HttpMessageBody{TBody}"/> implemented classes for a positive <see cref="HttpMessageBody{TBody}.HasSupportedMimeType"/> from the the specified <paramref name="mimeType"/> .
        /// </summary>
        /// <param name="mimeType">The <see cref="ContentType"/> associated with the entity-body of an HTTP message.</param>
        /// <param name="encoding">The <see cref="Encoding"/> associated with the entity-body of an HTTP message.</param>
        /// <returns>The <see cref="Type"/> of the <see cref="HttpMessageBody{TBody}"/> implemented class having a <paramref name="mimeType"/> that is part of the <see cref="HttpMessageBody{TBody}.SupportedMimeTypes"/>; otherwise null.</returns>
        public static Type Parse(ContentType mimeType, Encoding encoding)
        {
            IReadOnlyCollection<Type> messageBodies = GlobalModule.GetReferencedTypes(typeof(HttpMessageBody<>), "System", "Microsoft");
            foreach (Type messageBody in messageBodies)
            {
                if (messageBody.IsAbstract) { continue; }
                Type[] arguments = messageBody.ContainsGenericParameters ? messageBody.GetGenericArguments() : new Type[0];
                Type[] constraints = arguments.Length == 0 ? new Type[0] : arguments[0].GetGenericParameterConstraints();
                Type genericMessageBody = messageBody.ContainsGenericParameters ? messageBody.MakeGenericType(constraints.Length == 0 ? typeof(object) : constraints[0]) : null;
                object instance = Activator.CreateInstance(genericMessageBody ?? messageBody, mimeType, encoding);
                PropertyInfo hasSupportedMimeType = ReflectionUtility.GetProperty(instance.GetType(), "HasSupportedMimeType", typeof(bool), new Type[0], ReflectionUtility.BindingInstancePublic);
                if (hasSupportedMimeType != null)
                {
                    if ((bool)hasSupportedMimeType.GetValue(instance, null)) { return messageBody; }
                }
            }
            return null;
        }
    }

    /// <summary>
    /// Represents the entity-body of an HTTP message.
    /// </summary>
    /// <typeparam name="TBody">The type of the deserialized entity-body of an HTTP message.</typeparam>
    public abstract class HttpMessageBody<TBody>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMessageBody{TBody}"/> class.
        /// </summary>
        /// <param name="mimeType">The <see cref="ContentType"/> associated with the entity-body of an HTTP message.</param>
        /// <param name="encoding">The <see cref="Encoding"/> associated with the entity-body of an HTTP message.</param>
        protected HttpMessageBody(ContentType mimeType, Encoding encoding) : this(mimeType, encoding, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMessageBody{T}"/> class.
        /// </summary>
        /// <param name="mimeType">The <see cref="ContentType"/> associated with the entity-body of an HTTP message.</param>
        /// <param name="encoding">The <see cref="Encoding"/> associated with the entity-body of an HTTP message.</param>
        /// <param name="deserializer">The function delegate that converts the entity-body of an HTTP message to an object of type <typeparamref name="TBody"/>.</param>
        protected HttpMessageBody(ContentType mimeType, Encoding encoding, Doer<Stream, ContentType, Encoding, TBody> deserializer)
        {
            if (mimeType == null) { throw new ArgumentNullException("mimeType"); }
            if (encoding == null) { throw new ArgumentNullException("encoding"); }
            if (deserializer == null) { throw new ArgumentNullException("deserializer"); }

            this.MimeType = mimeType;
            this.Encoding = encoding;
            this.HasSupportedMimeType = this.SupportedMimeTypes == null ? false : EnumerableUtility.Contains(this.SupportedMimeTypes, mimeType, new PropertyEqualityComparer<ContentType>("MediaType", StringComparer.OrdinalIgnoreCase));
            this.Deserializer = deserializer;
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="MimeType"/> of this instance of an HTTP message entity-body has a positive match in <see cref="SupportedMimeTypes"/>.
        /// </summary>
        /// <value><c>true</c> if <see cref="MimeType"/> of this instance of an HTTP message entity-body has a positive match in <see cref="SupportedMimeTypes"/>; otherwise, <c>false</c>.</value>
        public bool HasSupportedMimeType { get; private set; }

        /// <summary>
        /// Gets the <see cref="ContentType"/> applied to this instance of an HTTP message entity-body.
        /// </summary>
        /// <value>The <see cref="ContentType"/> applied to this instance of an HTTP message entity-body.</value>
        public ContentType MimeType { get; private set; }

        /// <summary>
        /// Gets the <see cref="Encoding"/> applied to this instance of an HTTP message entity-body.
        /// </summary>
        /// <value>The <see cref="Encoding"/> applied to this instance of an HTTP message entity-body.</value>
        public Encoding Encoding { get; private set; }

        /// <summary>
        /// Gets a sequence of supported <see cref="ContentType"/> objects of this instance.
        /// </summary>
        /// <value>A sequence of supported <see cref="ContentType"/> objects of this instance.</value>
        public abstract IEnumerable<ContentType> SupportedMimeTypes { get; }

        /// <summary>
        /// Gets the function delegate that converts the entity-body of an HTTP message to an object of type <typeparamref name="TBody"/>.
        /// </summary>
        /// <value>The function delegate that converts the entity-body of an HTTP message to an object of type <typeparamref name="TBody"/>.</value>
        protected Doer<Stream, ContentType, Encoding, TBody> Deserializer { get; private set; }

        /// <summary>
        /// Converts the specified <paramref name="entityBody"/> to an object of type <typeparamref name="TBody"/>.
        /// </summary>
        /// <param name="entityBody">The entity-body to deserialize.</param>
        /// <returns>The deserialized object.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// Invalid MIME type passed to this instance.
        /// </exception>
        public virtual TBody Deserialize(Stream entityBody)
        {
            if (!this.HasSupportedMimeType)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "The MIME type of this instance is not supported. Expected MIME type of this instance must be one of the following: {0}. Actually MIME type was: {1}.",
                    ConvertUtility.ToDelimitedString(this.SupportedMimeTypes, ", "),
                    this.MimeType == null ? "<unspecified>" : this.MimeType.MediaType));
            }
            return this.Deserializer(entityBody, this.MimeType, this.Encoding);
        }
    }
}
