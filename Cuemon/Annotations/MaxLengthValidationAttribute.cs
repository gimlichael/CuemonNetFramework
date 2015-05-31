using System;
using System.Globalization;
using System.Reflection;
using Cuemon.Reflection;

namespace Cuemon.Annotations
{
    /// <summary>
    /// Denotes one or more properties that specifies the maximum length of an array or string allowed for an entity.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class MaxLengthValidationAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaxLengthValidationAttribute"/> class.
        /// </summary>
        /// <param name="length">The maximum allowable length of an array or a string.</param>
        public MaxLengthValidationAttribute(long length)
        {
            this.Length = length;
        }

        /// <summary>
        /// Gets the maximum allowable length of the array or string.
        /// </summary>
        /// <value>The maximum allowable length of the array or string for the decorated property.</value>
        public long Length { get; private set; }

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

            if (entityProperty.PropertyType.IsArray)
            {
                object array = entityProperty.GetValue(entity, null);
                PropertyInfo longLength = ReflectionUtility.GetProperty(entityProperty.PropertyType, "LongLength", typeof(long), null, ReflectionUtility.BindingInstancePublic);
                if (longLength != null && longLength.CanRead)
                {
                    long arrayLength = (long)longLength.GetValue(array, null);
                    if (arrayLength > this.Length) { throw new ValidationException(string.IsNullOrEmpty(this.Message) ? string.Format(CultureInfo.InvariantCulture, "Maximum length of {0} cannot exceed {1} elements. Actually length was {2}.", entityProperty.Name, this.Length, arrayLength) : this.Message); }
                }
            }
            else
            {
                TypeCode typeCode = Type.GetTypeCode(entityProperty.PropertyType);
                switch (typeCode)
                {
                    case TypeCode.String:
                        this.Validate(entityProperty.GetValue(entity, null) as string, entityProperty.Name);
                        break;
                    case TypeCode.Object:
                        if (entityProperty.PropertyType == typeof (Uri))
                        {
                            Uri uri = entityProperty.GetValue(entity, null) as Uri;
                            if (uri != null) { this.Validate(uri.OriginalString, entityProperty.Name); }
                            break;
                        }
                        goto default;
                    default:
                        throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Property '{0}' on '{1}' having a typecode of '{2}' is not suitable for the MaxLengthValidationAttribute.", entityProperty.Name, entityType, typeCode), "entityProperty");
                }
            }
        }

        private void Validate(string value, string propertyName)
        {
            if (value != null &&
                value.Length > this.Length) { throw new ValidationException(string.IsNullOrEmpty(this.Message) ? string.Format(CultureInfo.InvariantCulture, "Maximum length of {0} cannot exceed {1} characters. Actually characters was {2}.", propertyName, this.Length, value.Length) : this.Message); }
        }
    }
}
