using System;
using System.Globalization;

namespace Cuemon.Annotations
{
    /// <summary>
    /// Denotes one or more properties that specifies the minimum length of an array or string allowed for an entity.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class MinLengthValidationAttribute : ValidationAttribute
    {
        private static Doer<long, long, bool> validator = Condition.IsLowerThan;

        /// <summary>
        /// Initializes a new instance of the <see cref="MaxLengthValidationAttribute"/> class.
        /// </summary>
        /// <param name="length">The minimum length of an array or a string.</param>
        public MinLengthValidationAttribute(long length)
        {
            this.Length = length;
        }

        /// <summary>
        /// Gets or sets the function delegate that is invoked when <see cref="Validate(string,string)"/> or <see cref="Validate(string,Array)"/> is called.
        /// </summary>
        /// <value>The function delegate that will verify if a string or an array meets a specified minimum length.</value>
        /// <remarks><see cref="Condition.IsLowerThan{T}"/> is the default value of this function delegate.</remarks>
        public static Doer<long, long, bool> MinLengthValidator
        {
            get { return validator; }
            set
            {
                Validator.ThrowIfNull(value, "value");
                validator = value;
            }
        }

        /// <summary>
        /// Gets the minimum length of the array or string.
        /// </summary>
        /// <value>The minimum length of the array or string for the decorated property.</value>
        public long Length { get; private set; }

        /// <summary>
        /// Validates the specified <paramref name="value"/> has a length that must be at least the minimum defined <see cref="Length"/>.
        /// </summary>
        /// <param name="name">The name to include in the message of the <see cref="ValidationException"/>.</param>
        /// <param name="value">The string to verify has a length that must be at least the minimum defined <see cref="Length"/>.</param>
        /// <exception cref="Cuemon.Annotations.ValidationException">
        /// <paramref name="value"/> does not meed the minimum <see cref="Length"/>.
        /// </exception>
        public void Validate(string name, string value)
        {
            if (value != null && validator(value.Length, this.Length)) { throw new ValidationException(string.IsNullOrEmpty(this.Message) ? string.Format(CultureInfo.InvariantCulture, "Minimum length of {0} must be at least {1} characters. Actually characters was {2}.", name, this.Length, value.Length) : this.Message); }
        }

        /// <summary>
        /// Validates the specified <paramref name="value"/> has a length that must be at least the minimum defined <see cref="Length"/>.
        /// </summary>
        /// <param name="name">The name to include in the message of the <see cref="ValidationException"/>.</param>
        /// <param name="value">The string to verify has a length that must be at least the minimum defined <see cref="Length"/>.</param>
        /// <exception cref="Cuemon.Annotations.ValidationException">
        /// <paramref name="value"/> does not meed the minimum <see cref="Length"/>.
        /// </exception>
        public void Validate(string name, Array value)
        {
            if (value != null && validator(value.LongLength, this.Length)) { throw new ValidationException(string.IsNullOrEmpty(this.Message) ? string.Format(CultureInfo.InvariantCulture, "Minimum length of {0} must be at least {1} elements. Actually length was {2}.", name, this.Length, value.LongLength) : this.Message); }
        }
    }
}