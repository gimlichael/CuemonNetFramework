using System;
using System.Globalization;
using System.Security.Principal;
using System.Text;
using System.Web;

namespace Cuemon.Web.Security
{
    /// <summary>
    /// Provides a HTTP Basic Authentication implementation of an <see cref="IHttpModule"/>.
    /// </summary>
    public class BasicAuthenticationModule : ApplicationEventBinderModule<BasicAuthenticationModule>
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="BasicAuthenticationModule"/> is required to use secure sockets (that is, HTTPS).
        /// </summary>
        /// <value><c>true</c> if the HTTP connection is required to use secure sockets (that is, HTTPS); otherwise, <c>false</c>.</value>
        public static bool RequireSecureConnection { get; set; }

        /// <summary>
        /// Gets or sets the function delegate callback for credentials validation.
        /// </summary>
        /// <value>The function delegate callback for credentials validation.</value>
        public static Doer<string, string, IPrincipal> ValidateCredentialsCallback { get; set; }

        /// <summary>
        /// Gets the name of the authentication scheme.
        /// </summary>
        /// <value>The name of the authentication scheme.</value>
        public static string AuthenticationSchemeName
        {
            get { return "Basic"; }
        }

        /// <summary>
        /// Gets the realm that defines the protection space.
        /// </summary>
        /// <value>The realm that defines the protection space.</value>
        public virtual string Realm
        {
            get { return HttpRequestUtility.GetHostAuthority().OriginalString; }
        }

        /// <summary>
        /// Provides access to the AuthenticateRequest event of the <see cref="HttpApplication" />.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked when a security module is establishing the identity of the user.</remarks>
        protected override void OnAuthenticateRequest(HttpApplication context)
        {
            if (!AuthenticationUtility.TryAuthenticate(context, RequireSecureConnection, AuthorizationParser, PrincipalParser))
            {
                context.Context.Response.Status = AuthenticationUtility.HttpNotAuthorizedStatus;
                context.Context.Response.StatusCode = AuthenticationUtility.HttpNotAuthorizedStatusCode;
                context.CompleteRequest();
            }
        }

        /// <summary>
        /// Provides access to the EndRequest event of the <see cref="HttpApplication" />.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked as the last event in the HTTP pipeline chain of execution when ASP.NET responds to a request.</remarks>
        protected override void OnEndRequest(HttpApplication context)
        {
            if (context.Response.StatusCode == AuthenticationUtility.HttpNotAuthorizedStatusCode)
            {
                context.Response.AddHeader(AuthenticationUtility.HttpWwwAuthenticateHeader, string.Format(CultureInfo.InvariantCulture, "{0} realm=\"{1}\"", AuthenticationSchemeName, Realm));
            }
        }

        private bool PrincipalParser(HttpApplication context, Template<string, string> credentials, out IPrincipal result)
        {
            if (ValidateCredentialsCallback == null) { throw new InvalidOperationException("The ValidateCredentialsCallback delegate cannot be null."); }
            result = ValidateCredentialsCallback(credentials.Arg1, credentials.Arg2);
            return Condition.IsNotNull(result);
        }

        private Template<string, string> AuthorizationParser(string authorizationHeader)
        {
            if (AuthenticationUtility.IsAuthenticationSchemeValid(authorizationHeader, AuthenticationSchemeName))
            {
                string base64Credentials = authorizationHeader.Remove(0, AuthenticationSchemeName.Length + 1);
                if (StringUtility.IsBase64(base64Credentials))
                {
                    string[] credentials = ConvertUtility.ToString(Convert.FromBase64String(base64Credentials), PreambleSequence.Remove, Encoding.ASCII).Split(AuthenticationUtility.BasicAuthenticationCredentialSeparator);
                    if (credentials.Length == 2 &&
                        !string.IsNullOrEmpty(credentials[0]) &&
                        !string.IsNullOrEmpty(credentials[1]))
                    { return TupleUtility.CreateTwo(credentials[0], credentials[1]); }
                }
            }
            return null;
        }
    }
}