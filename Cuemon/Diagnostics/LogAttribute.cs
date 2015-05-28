using System;
namespace Cuemon.Diagnostics
{
    /// <summary>
    /// Provides members to describe name and source of an incident.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Struct, AllowMultiple = false)]
    public sealed class LogAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogAttribute"/> class.
        /// </summary>
        LogAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of the log to categorize possible events under.</param>
        /// <param name="source">The source name to associate with the provided <paramref name="name"/>.</param>
        public LogAttribute(string name, string source)
        {
            this.Name = name;
            this.Source = source;
        }

        /// <summary>
        /// Gets or sets the name of the log to categorize possible events under.
        /// </summary>
        /// <value>The name of the log to categorize possible events under.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the source name to associate with the log.
        /// </summary>
        /// <value>The source name to associate with the log.</value>
        public string Source { get; private set; }
    }
}