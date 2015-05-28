using System;
using System.Collections.Generic;
using System.Text;
namespace Cuemon.Annotations
{
	/// <summary>
	/// Denotes one or more properties that uniquely identify an entity.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public sealed class UniqueValidationAttribute : ValidationAttribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UniqueValidationAttribute"/> class.
		/// </summary>
		public UniqueValidationAttribute()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UniqueValidationAttribute"/> class.
		/// </summary>
		/// <param name="message">The message to relay from this <see cref="ValidationAttribute"/>.</param>
		public UniqueValidationAttribute(string message) : base(message)
		{
		}
	}
}