using System;
using System.Globalization;

namespace Cuemon.Annotations
{
    /// <summary>
    /// Denotes one or more properties that uniquely identify an entity.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class UniqueValidationAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UniqueValidationAttribute"/> class.
        /// </summary>
        public UniqueValidationAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UniqueValidationAttribute"/> class.
        /// </summary>
        /// <param name="message">The message to relay from this <see cref="ValidationAttribute"/>.</param>
        public UniqueValidationAttribute(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Immediately throws a <see cref="ValidationException"/> when invoked.
        /// </summary>
        /// <param name="name">The name to include in the message of the <see cref="ValidationException"/>.</param>
        /// <param name="value">The unique value that was violated.</param>
        /// <exception cref="Cuemon.Annotations.ValidationException">
        /// <paramref name="value"/> is not unique.
        /// </exception>
        public void Validate(string name, object value)
        {
            throw new ValidationException(string.IsNullOrEmpty(this.Message) ? string.Format(CultureInfo.InvariantCulture, "The value '{0}' of {1} must be unique.", value, name) : this.Message);
        }
    }
}