using System;
using System.Globalization;
using System.Reflection;

namespace Cuemon.Annotations
{
    /// <summary>
    /// Denotes one or more properties that validates an email address for an entity.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class EmailAddressValidationAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailAddressValidationAttribute"/> class.
        /// </summary>
        /// <remarks>
        /// In my search for the most comprehensive and up-to-date regular expression for email address validation, this was the article I choose to implement: http://blog.trojanhunter.com/2012/09/26/the-best-regex-to-validate-an-email-address/.
        /// </remarks>
        public EmailAddressValidationAttribute()
        {
        }

        /// <summary>
        /// Validates the specified <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">The value of the entity to validate.</param>
        /// <param name="entityType">The type of the entity to validate.</param>
        /// <param name="entityProperty">The property of the entity to validate.</param>
        public void Validate(object entity, Type entityType, PropertyInfo entityProperty)
        {
            if (entity == null) { throw new ArgumentNullException("entity"); }
            if (entityType == null) { throw new ArgumentNullException("entityType"); }
            if (entityProperty == null) { throw new ArgumentNullException("entityProperty"); }

            string emailAddress = entityProperty.GetValue(entity, null) as string;
            if (emailAddress == null) { return; }
            if (!Condition.IsEmailAddress(emailAddress)) { throw new ValidationException(string.IsNullOrEmpty(this.Message) ? string.Format(CultureInfo.InvariantCulture, "Value of {0} does not seem to a valid e-mail address. Validated value was {1}.", entityProperty.Name, emailAddress) : this.Message); }
        }
    }
}
