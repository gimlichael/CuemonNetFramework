using System;
using System.Collections;
using System.Globalization;
using Cuemon.Collections.Generic;

namespace Cuemon.Annotations
{
    /// <summary>
    /// Denotes one or more properties that is required for an entity.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class RequiredSequenceValidationAttribute : ValidationAttribute
    {
        private static Doer<IEnumerable, bool> validator = EnumerableUtility.Any;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredSequenceValidationAttribute"/> class.
        /// </summary>
        public RequiredSequenceValidationAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredSequenceValidationAttribute"/> class.
        /// </summary>
        /// <param name="message">The message to relay from this <see cref="ValidationAttribute"/>.</param>
        public RequiredSequenceValidationAttribute(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Gets or sets the function delegate that is invoked when <see cref="Validate(string,IEnumerable)"/> is called.
        /// </summary>
        /// <value>The function delegate that will verify if a given sequence has at least one element.</value>
        /// <remarks><see cref="EnumerableUtility.Any"/> is the default value of this function delegate.</remarks>
        public static Doer<IEnumerable, bool> RequiredSequenceValidator
        {
            get { return validator; }
            set
            {
                Validator.ThrowIfNull(value, "value");
                validator = value;
            }
        }

        /// <summary>
        /// Validates the specified <paramref name="value"/> has at least one element.
        /// </summary>
        /// <param name="name">The name to include in the message of the <see cref="ValidationException"/>.</param>
        /// <param name="value">The sequence to verify has at least one element.</param>
        /// <exception cref="Cuemon.Annotations.ValidationException">
        /// <paramref name="value"/> is not assigned any elements.
        /// </exception>
        public void Validate(string name, IEnumerable value)
        {
            if (value != null && !validator(value)) { throw new ValidationException(string.IsNullOrEmpty(this.Message) ? string.Format(CultureInfo.InvariantCulture, "{0} sequence is required.", name) : this.Message); }
        }
    }
}