using System;
using System.Globalization;

namespace Cuemon.Annotations
{
    /// <summary>
    /// Denotes one or more properties that validates an email address for an entity.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class EmailAddressValidationAttribute : ValidationAttribute
    {
        private static Doer<string, bool> validator = Condition.IsEmailAddress;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailAddressValidationAttribute"/> class.
        /// </summary>
        public EmailAddressValidationAttribute()
        {
        }

        /// <summary>
        /// Sets the function delegate that is invoked when <see cref="Validate"/> is called.
        /// </summary>
        /// <value>The function delegate that will verify if a string value has a valid format of an email address.</value>
        /// <remarks><see cref="Condition.IsEmailAddress"/> is the default value of this function delegate.</remarks>
        public static Doer<string, bool> EmailAddressValidator
        {
            set
            {
                Validator.ThrowIfNull(value, "value");
                validator = value;
            }
        }

        /// <summary>
        /// Validates the specified <paramref name="value"/> has a valid format of an email address.
        /// </summary>
        /// <param name="name">The name to include in the message of the <see cref="ValidationException"/>.</param>
        /// <param name="value">The string to verify has a valid format of an email address.</param>
        /// <exception cref="Cuemon.Annotations.ValidationException">
        /// <paramref name="value"/> is not valid email address.
        /// </exception>
        public void Validate(string name, string value)
        {
            if (value != null && !validator(value)) { throw new ValidationException(string.IsNullOrEmpty(this.Message) ? string.Format(CultureInfo.InvariantCulture, "Value of {0} does not seem to a valid e-mail address. Validated value was {1}.", name, value) : this.Message); }
        }
    }
}