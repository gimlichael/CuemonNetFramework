using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Cuemon.Diagnostics;
namespace Cuemon.Net
{
	/// <summary>
	/// This utility class is designed to make some service point manager operations easier to work with.
	/// </summary>
	public static class ServicePointManagerUtility
	{
		private static bool _enableServerCertificateValidation = true;

		/// <summary>
		/// Gets or sets a value indicating whether validation of server certificates (SSL/TLS) is enabled.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if validation of server certificates (SSL/TLS) is enabled; otherwise, <c>false</c>.
		/// </value>
		/// <remarks>Default value is <c>true</c>. Be advised, that you should only disable this in case of self-signed certificates for internal testing.</remarks>
		public static bool EnableServerCertificateValidation
		{
			get { return _enableServerCertificateValidation; }
			set { _enableServerCertificateValidation = value; }
		}

		/// <summary>
		/// Verifies the remote Secure Sockets Layer (SSL) certificate used for authentication.
		/// </summary>
		/// <param name="sender">An object that contains state information for this validation.</param>
		/// <param name="certificate">The certificate used to authenticate the remote party.</param>
		/// <param name="chain">The chain of certificate authorities associated with the remote certificate.</param>
		/// <param name="policyErrors">One or more errors associated with the remote certificate.</param>
		/// <returns>A <see cref="bool"/> value that determines whether the specified certificate is accepted for authentication.</returns>
		/// <remarks>This callback method follow the <see cref="EnableServerCertificateValidation"/> setting.</remarks>
		public static bool ValidateRemoteCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
		{
			if (EnableServerCertificateValidation)
			{
				return policyErrors == SslPolicyErrors.None;
			}
			return true;
		}
	}
}