using System;
using System.Security;
using System.Security.Principal;
using System.Web;

namespace Cuemon.Web.Security
{
    /// <summary>
    /// Provide a set of generic ways to work with HTTP based authentication.
    /// </summary>
    public static class AuthenticationUtility
    {
        /// <summary>
        /// The value of the header credential separator of a HTTP Basic access authentication.
        /// </summary>
        public const char BasicAuthenticationCredentialSeparator = ':';

        /// <summary>
        /// The value of the header credential separator of a HTTP Digest access authentication.
        /// </summary>
        public const char DigestAuthenticationCredentialSeparator = ',';

        /// <summary>
        /// The value of the ETag header for an HTTP response.
        /// </summary>
        public const string HttpEtagHeader = "Etag";

        /// <summary>
        /// The value of the WWW-Authenticate header for an HTTP response.
        /// </summary>
        public const string HttpWwwAuthenticateHeader = "WWW-Authenticate";

        /// <summary>
        /// The value of the Authorization header for an HTTP request.
        /// </summary>
        public const string HttpAuthorizationHeader = "Authorization";

        /// <summary>
        /// The value of the status description associated with <see cref="HttpNotAuthorizedStatusCode"/>.
        /// </summary>
        public const string HttpNotAuthorizedStatus = "401 Unauthorized";

        /// <summary>
        /// Equivalent to HTTP status 401. Unauthorized indicates that the requested resource requires authentication.
        /// </summary>
        public const int HttpNotAuthorizedStatusCode = 401;

        /// <summary>
        /// Provides a generic way to make authentication requests using the specified <paramref name="context"/>.
        /// </summary>
        /// <typeparam name="T">The type of the credentials returned from <paramref name="authorizationParser"/> and passed to <paramref name="principalParser"/>.</typeparam>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <param name="requireSecureConnection">When <c>true</c>, the HTTP connection is required to use secure sockets (that is, HTTPS); when <c>false</c> no requirement is enforced.</param>
        /// <param name="authorizationParser">The function delegate that will parse the authorization header of a web request and return the credentials of <typeparamref name="T"/>.</param>
        /// <param name="principalParser">The function delegate that will parse the credentials of <typeparamref name="T"/> returned from <paramref name="authorizationParser"/> and if successful returns a <see cref="IPrincipal"/> object.</param>
        /// <returns><c>true</c> if the specified parameters triggers a successful authentication; otherwise, <c>false</c>.</returns>
        public static bool TryAuthenticate<T>(HttpApplication context, bool requireSecureConnection, Doer<string, T> authorizationParser, TesterDoer<HttpApplication, T, IPrincipal, bool> principalParser)
        {
            try
            {
                Authenticate(context, requireSecureConnection, authorizationParser, principalParser);
                return true;
            }
            catch (SecurityException)
            {
                return false;
            }
        }

        /// <summary>
        /// Provides a generic way to make authentication requests using the specified <paramref name="context"/>.
        /// </summary>
        /// <typeparam name="T">The type of the credentials returned from <paramref name="authorizationParser"/> and passed to <paramref name="principalParser"/>.</typeparam>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <param name="requireSecureConnection">When <c>true</c>, the HTTP connection is required to use secure sockets (that is, HTTPS); when <c>false</c> no requirement is enforced.</param>
        /// <param name="authorizationParser">The function delegate that will parse the authorization header of a web request and return the credentials of <typeparamref name="T"/>.</param>
        /// <param name="principalParser">The function delegate that will parse the credentials of <typeparamref name="T"/> returned from <paramref name="authorizationParser"/> and if successful returns a <see cref="IPrincipal"/> object.</param>
        /// <exception cref="SecurityException">
        /// Authorized failed for the request.
        /// </exception>
        public static void Authenticate<T>(HttpApplication context, bool requireSecureConnection, Doer<string, T> authorizationParser, TesterDoer<HttpApplication, T, IPrincipal, bool> principalParser)
        {
            Validator.ThrowIfNull(context, "context");
            if (requireSecureConnection &&
                !context.Request.IsSecureConnection)
            { throw new SecurityException("An SSL connection is required for the request."); }
            IPrincipal principal;
            if (TryGetPrincipal(context, authorizationParser, principalParser, out principal))
            {
                context.Context.User = principal;
                return;
            }
            throw new SecurityException("Authorized failed for the request.");
        }

        private static bool TryGetPrincipal<T>(HttpApplication context, Doer<string, T> authorizationParser, TesterDoer<HttpApplication, T, IPrincipal, bool> principalParser, out IPrincipal principal)
        {
            principal = null;
            string authorizationHeader = context.Request.Headers[HttpAuthorizationHeader];
            if (String.IsNullOrEmpty(authorizationHeader)) { return false; }
            T credentials = authorizationParser(authorizationHeader);
            if (credentials != null && principalParser(context, credentials, out principal)) { return true; }
            return false;
        }

        internal static bool IsAuthenticationSchemeValid(string authorizationHeader, string authenticationSchemeName)
        {
            return (!String.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith(authenticationSchemeName, StringComparison.Ordinal));
        }
    }
}