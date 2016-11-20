using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net.Mime;
using System.Text;
using Cuemon.Web;

namespace Cuemon.ServiceModel
{
    /// <summary>
    /// Represents a entity-body of an HTTP message that is formatted in name/value fields encoded using application/x-www-form-urlencoded MIME type. This class cannot be inherited.
    /// </summary>
    public sealed class FormUrlEncodedMessageBody : HttpMessageBody<NameValueCollection>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormUrlEncodedMessageBody"/> class.
        /// </summary>
        /// <param name="mimeType">The <see cref="ContentType"/> associated with the entity-body of an HTTP message.</param>
        /// <param name="encoding">The <see cref="Encoding"/> associated with the entity-body of an HTTP message.</param>
        public FormUrlEncodedMessageBody(ContentType mimeType, Encoding encoding) : base(mimeType, encoding, FormUrlEncodedDeserializer)
        {
        }

        /// <summary>
        /// Gets a sequence of supported <see cref="ContentType"/> objects of this instance.
        /// </summary>
        /// <value>A sequence of supported <see cref="ContentType"/> objects of this instance.</value>
        public override IEnumerable<ContentType> SupportedMimeTypes
        {
            get { yield return new ContentType("application/x-www-form-urlencoded"); }
        }

        /// <summary>
        /// Deserializes the specified entity body to a collection of associated <see cref="string"/> keys and <see cref="string"/> values that can be accessed either with the key or with the index.
        /// </summary>
        /// <param name="entityBody">The entity-body to deserialize.</param>
        /// <returns>A <see cref="NameValueCollection"/> representing a collection of form variables that is equivalent to <paramref name="entityBody"/>.</returns>
        public override NameValueCollection Deserialize(Stream entityBody)
        {
            return base.Deserialize(entityBody);
        }

        private static NameValueCollection FormUrlEncodedDeserializer(Stream entityBody, ContentType mimeType, Encoding encoding)
        {
            string form = StringConverter.FromStream(entityBody, options =>
            {
                options.Encoding = encoding;
                options.Preamble = PreambleSequence.Remove;
            });
            return HttpRequestUtility.ParseFieldValuePairs(form);
        }
    }
}