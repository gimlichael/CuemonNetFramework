using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
namespace Cuemon.Data
{
	/// <summary>
	/// The exception that is thrown when a unique index violation occurs from a data source.
	/// </summary>
	[Serializable]
	public class UniqueIndexViolationException : Exception
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="UniqueIndexViolationException"/> class.
		/// </summary>
		public UniqueIndexViolationException() : base()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UniqueIndexViolationException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public UniqueIndexViolationException(string message) : base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ParseException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		/// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
		public UniqueIndexViolationException(string message, Exception innerException) : base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UniqueIndexViolationException"/> class.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
		protected UniqueIndexViolationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
		#endregion

		#region Properties
		#endregion
	}
}