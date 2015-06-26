using System;
using System.Globalization;

namespace Cuemon.Annotations
{
    /// <summary>
    /// Denotes one or more properties that is required for an entity.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class RequiredValidationAttribute : ValidationAttribute
    {
        private static Doer<object, bool> validator = Condition.IsNull;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredValidationAttribute"/> class.
        /// </summary>
        public RequiredValidationAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredValidationAttribute"/> class.
        /// </summary>
        /// <param name="message">The message to relay from this <see cref="ValidationAttribute"/>.</param>
        public RequiredValidationAttribute(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Gets or sets the function delegate that is invoked when <see cref="Validate(string,object)"/> is called.
        /// </summary>
        /// <value>The function delegate that will verify if a given object has a value.</value>
        /// <remarks><see cref="Condition.IsNull{T}"/> is the default value of this function delegate.</remarks>
        public static Doer<object, bool> RequiredValidator
        {
            get { return validator; }
            set
            {
                Validator.ThrowIfNull(value, "value");
                validator = value;
            }
        }

        /// <summary>
        /// Validates the specified <paramref name="value"/> has an assigned value.
        /// </summary>
        /// <param name="name">The name to include in the message of the <see cref="ValidationException"/>.</param>
        /// <param name="value">The object to verify has an assigned value.</param>
        /// <exception cref="Cuemon.Annotations.ValidationException">
        /// <paramref name="value"/> is not assigned a value.
        /// </exception>
        public void Validate(string name, object value)
        {
            if (validator(value)) { throw new ValidationException(string.IsNullOrEmpty(this.Message) ? string.Format(CultureInfo.InvariantCulture, "{0} specification is required.", name) : this.Message); }
        }
    }
}