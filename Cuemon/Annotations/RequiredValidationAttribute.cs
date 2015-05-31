using System;

namespace Cuemon.Annotations
{
	/// <summary>
	/// Denotes one or more properties that is required for an entity.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public sealed class RequiredValidationAttribute : ValidationAttribute
	{
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
		public RequiredValidationAttribute(string message) : base(message)
		{
        }
    }
}