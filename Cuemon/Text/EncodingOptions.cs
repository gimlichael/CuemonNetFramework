﻿using System.Text;

namespace Cuemon.Text
{
    /// <summary>
    /// Specifies options that is related to <see cref="Encoding"/> operations.
    /// </summary>
    public class EncodingOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EncodingOptions"/> class.
        /// </summary>
        /// <remarks>
        /// The following table shows the initial property values for an instance of <see cref="EncodingOptions"/>.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Property</term>
        ///         <description>Initial Value</description>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="Preamble"/></term>
        ///         <description><see cref="PreambleSequence.Remove"/></description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="Encoding"/></term>
        ///         <description><see cref="System.Text.Encoding.UTF8"/></description>
        ///     </item>
        /// </list>
        /// </remarks>
        public EncodingOptions()
        {
            Preamble = PreambleSequence.Remove;
            Encoding = Encoding.UTF8;
        }

        /// <summary>
        /// Gets or sets the action to take in regards to encoding related preamble sequences.
        /// </summary>
        /// <value>A value that indicates whether to preserve or remove preamble sequences.</value>
        public PreambleSequence Preamble { get; set; }

        /// <summary>
        /// Gets or sets the encoding for the operation.
        /// </summary>
        /// <value>The encoding for the operation.</value>
        public Encoding Encoding { get; set; }
    }
}