using System;

namespace Cuemon.Annotations
{
	/// <summary>
	/// Serves as the base class for all validation attributes.
	/// </summary>
	public abstract class ValidationAttribute : Attribute
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="ValidationAttribute"/> class.
		/// </summary>
		protected ValidationAttribute()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidationAttribute"/> class.
		/// </summary>
		/// <param name="message">The message to relay from this <see cref="ValidationAttribute"/>.</param>
		protected ValidationAttribute(string message)
		{
			this.Message = message;
		}
		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the message to relay from this <see cref="ValidationAttribute"/>.
		/// </summary>
		/// <value>
		/// The message to relay from this <see cref="ValidationAttribute"/>.
		/// </value>
		public string Message { get; set; }
		#endregion

        #region Methods
        #endregion
    }
}