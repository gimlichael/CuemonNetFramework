using System;

namespace Cuemon.ServiceModel
{
    /// <summary>
    /// Defines the supported HTTP entity body formats for the client to query a web server by.
    /// </summary>
    /// <remarks>This enumeration has a <see cref="FlagsAttribute" /> that allows a bitwise combination of its member values.</remarks>
	[Flags]
    public enum HttpEntityBodyFormats
    {
        /// <summary>
        /// Represents a RAW based HTTP entity body that is related to FORM content types.
        /// </summary>
        Raw = 1,
        /// <summary>
        /// Represents an XML based HTTP entity body.
        /// </summary>
        Xml = 2,
        /// <summary>
        /// Represents a JSON based HTTP entity body.
        /// </summary>
        Json = 4
    }
}