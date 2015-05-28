using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using Cuemon.Reflection;

namespace Cuemon.Annotations
{
    /// <summary>
    /// Denotes one or more properties that specifies the numeric range constraints for an entity.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class RangeValidationAttribute : ValidationAttribute
    {
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
        public RangeValidationAttribute(double minimum, double maximum, string message) : base(message)
        {
            this.Minimum = minimum;
            this.Maximum = maximum;
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
        /// Validates the specified <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">The value of the entity to validate.</param>
        /// <param name="entityType">The type of the entity to validate.</param>
        /// <param name="entityProperty">The property of the entity to validate.</param>
        /// <exception cref="System.ArgumentException">entityType
        /// <paramref name="entityProperty"/> is not suitable for this attribute.
        /// </exception>
        public void Validate(object entity, Type entityType, PropertyInfo entityProperty)
        {
            if (entity == null) { throw new ArgumentNullException("entity"); }
            if (entityType == null) { throw new ArgumentNullException("entityType"); }
            if (entityProperty == null) { throw new ArgumentNullException("entityProperty"); }

            TypeCode typeCode = Type.GetTypeCode(entityProperty.PropertyType);
            switch (typeCode)
            {
                case TypeCode.Byte:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    double rangeValue = Convert.ToDouble(entityProperty.GetValue(entity, null), CultureInfo.InvariantCulture);
                    if (rangeValue < this.Minimum | rangeValue > this.Maximum) { throw new ValidationException(string.IsNullOrEmpty(this.Message) ? string.Format(CultureInfo.InvariantCulture, "Value of {0} must be between {1} and {2}. Actually value was {3}.", entityProperty.Name, this.Minimum, this.Maximum, rangeValue) : this.Message); }
                    break;
                default:
                    throw new ArgumentOutOfRangeException("entityType", string.Format(CultureInfo.InvariantCulture, "Property '{0}' on '{1}' having a typecode of '{2}' is not suitable for the RangeValidationAttribute.", entityProperty.Name, entityType, typeCode));
            }
        }
    }
}
