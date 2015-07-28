using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using Cuemon.Collections.Generic;
using Cuemon.Reflection;

namespace Cuemon
{
    /// <summary>
    /// This utility class is designed to make common object related operations easier to work with.
    /// </summary>
    public static class ObjectUtility
    {
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents the specified <paramref name="instance"/>.
        /// </summary>
        /// <param name="instance">The instance to represent.</param>
        /// <returns>A <see cref="System.String" /> that represents the specified <paramref name="instance"/>.</returns>
        public static string ToString(object instance)
        {
            return ToString(instance, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents the specified <paramref name="instance"/>.
        /// </summary>
        /// <param name="instance">The instance to represent.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A <see cref="System.String" /> that represents the specified <paramref name="instance"/>.</returns>
        public static string ToString(object instance, IFormatProvider provider)
        {
            return ToString(instance, provider, ", ");
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents the specified <paramref name="instance"/>.
        /// </summary>
        /// <param name="instance">The instance to represent.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <param name="delimiter">The delimiter specification for when representing public properties of <paramref name="instance"/>.</param>
        /// <returns>A <see cref="System.String" /> that represents the specified <paramref name="instance"/>.</returns>
        public static string ToString(object instance, IFormatProvider provider, string delimiter)
        {
            return ToString(instance, provider, delimiter, DefaultPropertyConverter);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents the specified <paramref name="instance"/>.
        /// </summary>
        /// <param name="instance">The instance to represent.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <param name="delimiter">The delimiter specification for when representing public properties of <paramref name="instance"/>.</param>
        /// <param name="propertyConverter">The function delegate that convert <see cref="PropertyInfo"/> objects to human-readable content.</param>
        /// <returns>A <see cref="System.String" /> that represents the specified <paramref name="instance"/>.</returns>
        public static string ToString(object instance, IFormatProvider provider, string delimiter, Doer<PropertyInfo, object, IFormatProvider, string> propertyConverter)
        {
            return ToString(instance, provider, delimiter, propertyConverter, ReflectionUtility.GetProperties, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents the specified <paramref name="instance"/>.
        /// </summary>
        /// <param name="instance">The instance to represent.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <param name="delimiter">The delimiter specification for when representing public properties of <paramref name="instance"/>.</param>
        /// <param name="propertyConverter">The function delegate that convert <see cref="PropertyInfo"/> objects to human-readable content.</param>
        /// <param name="propertiesReader">The function delegate that read <see cref="PropertyInfo"/> objects from the underlying <see cref="Type"/> of <paramref name="instance"/>.</param>
        /// <param name="propertiesReaderBindingAttr">A bitmask comprised of one or more <see cref="BindingFlags"/> that specify how the search for <see cref="PropertyInfo"/> objects in the function delegate <paramref name="propertiesReader"/> is conducted.</param>
        /// <returns>A <see cref="System.String" /> that represents the specified <paramref name="instance"/>.</returns>
        /// <remarks>
        /// When detemining the representation of the specified <paramref name="instance"/>, these rules applies: <br/>
        /// 1: if the <see cref="object.ToString"/> method has been overridden, any further processing is skipped<br/>
        /// 2: any public properties having index parameters is skipped<br/>
        /// 3: any public properties is appended to the result if <see cref="object.ToString"/> has not been overridden.<br/><br/>
        /// Note: do not call this method from an overridden ToString(..) method; stackoverflow exception will occur.
        /// </remarks>
        public static string ToString(object instance, IFormatProvider provider, string delimiter, Doer<PropertyInfo, object, IFormatProvider, string> propertyConverter, Doer<Type, BindingFlags, IEnumerable<PropertyInfo>> propertiesReader, BindingFlags propertiesReaderBindingAttr)
        {
            Validator.ThrowIfNull(propertyConverter, "propertyConverter");
            Validator.ThrowIfNull(propertiesReader, "propertiesReader");

            if (instance == null) { return "<null>"; }

            Doer<string> toString = instance.ToString;
            if (ReflectionUtility.IsOverride(toString.Method))
            {
                return toString();
            }

            Type instanceType = instance.GetType();
            StringBuilder instanceSignature = new StringBuilder(string.Format(provider, "{0}", TypeUtility.SanitizeTypeName(instanceType, true)));
            IEnumerable<PropertyInfo> properties = EnumerableUtility.Where(propertiesReader(instanceType, propertiesReaderBindingAttr), IndexParametersLengthIsZeroPredicate);
            instanceSignature.AppendFormat(" {{ {0} }}", ConvertUtility.ToDelimitedString(properties, delimiter, propertyConverter, instance, provider));
            return instanceSignature.ToString();
        }

        private static bool IndexParametersLengthIsZeroPredicate(PropertyInfo property)
        {
            return (property.GetIndexParameters().Length == 0);
        }

        private static string DefaultPropertyConverter(PropertyInfo property, object instance, IFormatProvider provider)
        {
            if (property.CanRead)
            {
                if (TypeUtility.IsComplex(property.PropertyType))
                {
                    return string.Format(provider, "{0}={1}", property.Name, TypeUtility.SanitizeTypeName(property.PropertyType, true));
                }
                object instanceValue = ReflectionUtility.GetPropertyValue(instance, property);
                return string.Format(provider, "{0}={1}", property.Name, instanceValue ?? "<null>");
            }
            return string.Format(provider, "{0}=<no getter>", property.Name);
        }
    }
}