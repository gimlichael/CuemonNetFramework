﻿using System.Collections.Generic;
using System.Globalization;
using Cuemon.Runtime.Serialization;

namespace Cuemon.Web.Security
{
    /// <summary>
    /// Represents the header information of JSON Web Token that is based on the standard RFC 7519. This class cannot be inherited.
    /// </summary>
    public sealed class JsonWebTokenHeader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonWebTokenHeader"/> class.
        /// </summary>
        public JsonWebTokenHeader() : this("JWT")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonWebTokenHeader"/> class.
        /// </summary>
        /// <param name="type">The type of the token. Default is JWT.</param>
        public JsonWebTokenHeader(string type)
        {
            Validator.ThrowIfNullOrEmpty(type, nameof(type));
            Type = type;
        }

        /// <summary>
        /// Gets or sets the algorithm of the token.
        /// </summary>
        /// <value>The algorithm of the token.</value>
        public JsonWebTokenHashAlgorithm Algorithm { get; set; }

        /// <summary>
        /// Gets the type of the token.
        /// </summary>
        /// <value>The type of the token.</value>
        public string Type { get; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        /// <remarks>Calling this method provides the raw header information of the JSON Web Token.</remarks>
        public override string ToString()
        {
            List<string> header = new List<string>();
            header.Add(string.Format(CultureInfo.InvariantCulture, "\"alg\": {0}", JsonConverter.ToString(JsonWebTokenHashAlgorithmConverter.ToString(Algorithm))));
            header.Add(string.Format(CultureInfo.InvariantCulture, "\"typ\": {0}", JsonConverter.ToString(Type)));
            return string.Format(CultureInfo.InvariantCulture, "{{ {0} }}", StringConverter.ToDelimitedString(header, ", "));
        }
    }
}