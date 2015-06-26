using System;
using System.Globalization;

namespace Cuemon.Annotations
{
    /// <summary>
    /// Denotes one or more properties that specifies the numeric range constraints for an entity.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class RangeValidationAttribute : ValidationAttribute
    {
        private static Doer<double, double, double, bool> validator = Condition.IsWithinRange;

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeValidationAttribute"/> class.
        /// </summary>
        /// <param name="minimum">Specifies the minimum value allowed.</param>
        /// <param name="maximum">Specifies the maximum value allowed.</param>
        public RangeValidationAttribute(double minimum, double maximum)
        {
            this.Minimum = minimum;
            this.Maximum = maximum;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RangeValidationAttribute"/> class.
        /// </summary>
        /// <param name="minimum">Specifies the minimum value allowed.</param>
        /// <param name="maximum">Specifies the maximum value allowed.</param>
        /// <param name="message">The message to relay from this <see cref="ValidationAttribute"/>.</param>
        public RangeValidationAttribute(double minimum, double maximum, string message)
            : base(message)
        {
            this.Minimum = minimum;
            this.Maximum = maximum;
        }

        /// <summary>
        /// Gets or sets the function delegate that is invoked when <see cref="Validate(string,double)"/>  is called.
        /// </summary>
        /// <value>The function delegate that will verify if a value is within a given range.</value>
        /// <remarks><see cref="Condition.IsWithinRange{T}"/> is the default value of this function delegate.</remarks>
        public static Doer<double, double, double, bool> RangeValidator
        {
            get { return validator; }
            set
            {
                Validator.ThrowIfNull(value, "value");
                validator = value;
            }
        }

        /// <summary>
        /// Gets the maximum allowed value.
        /// </summary>
        /// <value>The maximum value that is allowed for the decorated property.</value>
        public double Maximum { get; private set; }

        /// <summary>
        /// Gets the minimum allowed value.
        /// </summary>
        /// <value>The minimum value that is allowed for the decorated property.</value>
        public double Minimum { get; private set; }

        /// <summary>
        /// Validates the specified <paramref name="value"/> is within the range of <see cref="Minimum"/> and <see cref="Maximum"/>.
        /// </summary>
        /// <param name="name">The name to include in the message of the <see cref="ValidationException"/>.</param>
        /// <param name="value">The double to verify is within the range of <see cref="Minimum"/> and <see cref="Maximum"/>.</param>
        /// <exception cref="Cuemon.Annotations.ValidationException">
        /// <paramref name="value"/> is outside the range of <see cref="Minimum"/> and <see cref="Maximum"/>.
        /// </exception>
        public void Validate(string name, double value)
        {
            if (!validator(value, this.Minimum, this.Maximum)) { throw new ValidationException(string.IsNullOrEmpty(this.Message) ? string.Format(CultureInfo.InvariantCulture, "Value of {0} must be between {1} and {2}. Actually value was {3}.", name, this.Minimum, this.Maximum, value) : this.Message); }
        }
    }
}