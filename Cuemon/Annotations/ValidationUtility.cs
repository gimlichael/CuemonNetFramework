using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Cuemon.Reflection;

namespace Cuemon.Annotations
{
    /// <summary>
    /// Provide ways to validate objects decorated with one or more property specific <see cref="ValidationAttribute"/> attributes.
    /// </summary>
    public static class ValidationUtility
    {
        /// <summary>
        /// Validates the <see cref="ValidationAttribute"/> attributes discovered from the specified <paramref name="instance"/>.
        /// </summary>
        /// <param name="instance">The object to read values from properties decorated with <see cref="ValidationAttribute"/> attributes.</param>
        public static void Validate(object instance)
        {
            Validator.ThrowIfNull(instance, "instance");

            Validate(instance, instance.GetType());
        }

        /// <summary>
        /// Validates the <see cref="ValidationAttribute"/> attributes discovered from the specified <paramref name="instanceType"/>.
        /// </summary>
        /// <param name="instance">The object to read values from properties decorated with <see cref="ValidationAttribute"/> attributes.</param>
        /// <param name="instanceType">The <see cref="Type"/> to discover properties decorated with <see cref="ValidationAttribute"/> attributes.</param>
        public static void Validate(object instance, Type instanceType)
        {
            Validator.ThrowIfNull(instance, "instance");
            Validator.ThrowIfNull(instanceType, "instanceType");

            Validate(instance, instanceType, ValidationUtility.Parse);
        }

        /// <summary>
        /// Validates the <see cref="ValidationAttribute"/> attributes discovered from the specified <paramref name="instanceType"/>.
        /// </summary>
        /// <param name="instance">The object to read values from properties decorated with <see cref="ValidationAttribute"/> attributes.</param>
        /// <param name="instanceType">The <see cref="Type"/> to discover properties decorated with <see cref="ValidationAttribute"/> attributes.</param>
        /// <param name="parser">The function delegate that will parse, invoke and validate properties decorated with <see cref="ValidationAttribute"/> attributes.</param>
        /// <remarks>The default value of <paramref name="parser"/> function delegate is <see cref="Parse"/>.</remarks>
        public static void Validate(object instance, Type instanceType, Act<ValidationAttribute, PropertyInfo, object, Type> parser)
        {
            Validator.ThrowIfNull(instance, "instance");
            Validator.ThrowIfNull(instanceType, "instanceType");
            Validator.ThrowIfNull(parser, "parser");

            IDictionary<PropertyInfo, ValidationAttribute[]> propertiesWithDecorations = ReflectionUtility.GetPropertyAttributeDecorations<ValidationAttribute>(instanceType);
            foreach (KeyValuePair<PropertyInfo, ValidationAttribute[]> propertyWithDecoration in propertiesWithDecorations)
            {
                foreach (ValidationAttribute validation in propertyWithDecoration.Value)
                {
                    parser(validation, propertyWithDecoration.Key, instance, instanceType);
                }
            }
        }

        /// <summary>
        /// Parses the specified <paramref name="property"/> while retrieving the value from <paramref name="instance"/> and validates using the provided <paramref name="attribute"/>.
        /// </summary>
        /// <param name="attribute">The <see cref="ValidationAttribute"/> to use when validating.</param>
        /// <param name="property">The property on which the <paramref name="attribute"/> was decorated.</param>
        /// <param name="instance">The instance on which the <paramref name="property"/> can be invoked.</param>
        /// <param name="instanceType">The <see cref="Type"/> on which both <paramref name="property"/> and <paramref name="attribute"/> was discovered.</param>
        public static void Parse(ValidationAttribute attribute, PropertyInfo property, object instance, Type instanceType)
        {
            ParseRequiredValidation(attribute as RequiredValidationAttribute, property, instance);
            ParseRequiredSequenceValidation(attribute as RequiredSequenceValidationAttribute, property, instance);
            ParseMaxLengthValidation(attribute as MaxLengthValidationAttribute, property, instance);
            ParseMinLengthValidation(attribute as MinLengthValidationAttribute, property, instance);
            ParseRangeValidation(attribute as RangeValidationAttribute, property, instance);
            ParseEmailAddressValidation(attribute as EmailAddressValidationAttribute, property, instance);
        }

        private static void ParseRequiredValidation(RequiredValidationAttribute attribute, PropertyInfo property, object instance)
        {
            if (attribute == null) { return; }
            attribute.Validate(property.Name, property.GetValue(instance, null));
        }

        private static void ParseRequiredSequenceValidation(RequiredSequenceValidationAttribute attribute, PropertyInfo property, object instance)
        {
            if (attribute == null) { return; }
            attribute.Validate(property.Name, property.GetValue(instance, null) as IEnumerable);
        }

        private static void ParseRangeValidation(RangeValidationAttribute attribute, PropertyInfo property, object instance)
        {
            if (attribute == null) { return; }
            TypeCode typeCode = Type.GetTypeCode(property.PropertyType);
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
                    double rangeValue = Convert.ToDouble(property.GetValue(instance, null), CultureInfo.InvariantCulture);
                    attribute.Validate(property.Name, rangeValue);
                    break;
            }
        }

        private static void ParseMaxLengthValidation(MaxLengthValidationAttribute attribute, PropertyInfo property, object instance)
        {
            if (attribute == null) { return; }
            if (property.PropertyType.IsArray)
            {
                attribute.Validate(property.Name, GetArrayForLengthValidationCore(property, instance));
            }
            else
            {
                attribute.Validate(property.Name, GetStringForLengthValidationCore(property, instance));
            }
        }

        private static void ParseMinLengthValidation(MinLengthValidationAttribute attribute, PropertyInfo property, object instance)
        {
            if (attribute == null) { return; }
            if (property.PropertyType.IsArray)
            {
                attribute.Validate(property.Name, GetArrayForLengthValidationCore(property, instance));
            }
            else
            {
                attribute.Validate(property.Name, GetStringForLengthValidationCore(property, instance));
            }
        }

        private static string GetStringForLengthValidationCore(PropertyInfo property, object instance)
        {
            string value = null;
            TypeCode typeCode = Type.GetTypeCode(property.PropertyType);
            switch (typeCode)
            {
                case TypeCode.String:
                    value = property.GetValue(instance, null) as string;
                    break;
                case TypeCode.Object:
                    if (property.PropertyType == typeof(Uri))
                    {
                        Uri uri = property.GetValue(instance, null) as Uri;
                        if (uri != null)
                        {
                            value = uri.OriginalString;
                        }
                    }
                    break;
            }
            return value;
        }

        private static Array GetArrayForLengthValidationCore(PropertyInfo property, object instance)
        {
            return property.GetValue(instance, null) as Array;
        }

        private static void ParseEmailAddressValidation(EmailAddressValidationAttribute attribute, PropertyInfo property, object instance)
        {
            if (attribute == null) { return; }
            string emailAddress = property.GetValue(instance, null) as string;
            attribute.Validate(property.Name, emailAddress);
        }
    }
}