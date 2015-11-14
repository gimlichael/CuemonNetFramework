using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Principal;
using System.Web;
using Cuemon.Security.Cryptography;

namespace Cuemon.Web.Security
{
    /// <summary>
    /// Provides a HTTP Digest Access Authentication implementation of an <see cref="IHttpModule"/>.
    /// </summary>
    public class DigestAccessAuthenticationModule : ApplicationEventBinderModule<DigestAccessAuthenticationModule>
    {
        private static readonly Dictionary<string, Template<DateTime, string>> NonceCounter = new Dictionary<string, Template<DateTime, string>>();

        /// <summary>
        /// Gets or sets the algorithm of the HTTP Digest Access Authentication. Default is <see cref="HashAlgorithmType.MD5"/>.
        /// </summary>
        /// <value>The algorithm of the HTTP Digest Access Authentication.</value>
        public static HashAlgorithmType Algorithm { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="DigestAccessAuthenticationModule"/> is required to use secure sockets (that is, HTTPS).
        /// </summary>
        /// <value><c>true</c> if the HTTP connection is required to use secure sockets (that is, HTTPS); otherwise, <c>false</c>.</value>
        public static bool RequireSecureConnection { get; set; }

        /// <summary>
        /// Gets or sets the function delegate for credentials validation.
        /// </summary>
        /// <value>The function delegate for credentials validation.</value>
        public static TesterDoer<string, string, IPrincipal> ValidateCredentialsCallback { get; set; }

        /// <summary>
        /// Gets or sets the function delegate for generating opaque string values.
        /// </summary>
        /// <value>The function delegate for generating opaque string values.</value>
        public static Doer<string> OpaqueGeneratorCallback { get; set; }

        /// <summary>
        /// Gets or sets the function delegate for retrieving the cryptographic secret used in nonce string values.
        /// </summary>
        /// <value>The function delegate for retrieving the cryptographic secret used in nonce string values.</value>
        public static Doer<byte[]> NonceSecretCallback { get; set; }

        /// <summary>
        /// Gets or sets the function delegate for generating nonce string values.
        /// </summary>
        /// <value>The function delegate for generating nonce string values.</value>
        public static Doer<DateTime, string, byte[], string> NonceGeneratorCallback { get; set; }

        /// <summary>
        /// Gets or sets the function delegate for parsing nonce string values for expiration.
        /// </summary>
        /// <value>The function delegate for parsing nonce string values for expiration.</value>
        public static Doer<string, TimeSpan, bool> NonceExpiredParserCallback { get; set; }

        /// <summary>
        /// Gets the name of the authentication scheme.
        /// </summary>
        /// <value>The name of the authentication scheme.</value>
        public static string AuthenticationSchemeName
        {
            get { return "Digest"; }
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
                HttpResponseUtility.RemoveResponseHeader(context.Response, "ETag");
                string etag = context.Response.Headers[AuthenticationUtility.HttpEtagHeader];
                if (string.IsNullOrEmpty(etag)) { etag = "no-entity-tag"; }
                Doer<string> opaqueGenerator = OpaqueGeneratorCallback ?? DigestAuthenticationUtility.DefaultOpaqueGenerator;
                Doer<byte[]> nonceSecret = NonceSecretCallback ?? DigestAuthenticationUtility.DefaultPrivateKey;
                Doer<DateTime, string, byte[], string> nonceGenerator = NonceGeneratorCallback ?? DigestAuthenticationUtility.DefaultNonceGenerator;
                string staleNonce = context.Context.Items["staleNonce"] as string ?? "FALSE";
                context.Response.AddHeader(AuthenticationUtility.HttpWwwAuthenticateHeader, string.Format(CultureInfo.InvariantCulture, "{0} realm=\"{1}\", qop=\"{2}\", nonce=\"{3}\", opaque=\"{4}\", stale=\"{5}\", algorithm=\"{6}\"",
                    AuthenticationSchemeName,
                    Realm,
                    DigestAuthenticationUtility.CredentialQualityOfProtectionOptions,
                    nonceGenerator(DateTime.UtcNow, etag, nonceSecret()),
                    opaqueGenerator(),
                    staleNonce,
                    DigestAuthenticationUtility.ParseAlgorithm(Algorithm)));
            }
        }



        private bool PrincipalParser(HttpApplication context, Dictionary<string, string> credentials, out IPrincipal result)
        {
            if (ValidateCredentialsCallback == null) { throw new InvalidOperationException("The ValidateCredentialsCallback delegate cannot be null."); }
            string password, userName, clientResponse, nonce, nonceCount;
            credentials.TryGetValue(DigestAuthenticationUtility.CredentialUserName, out userName);
            credentials.TryGetValue(DigestAuthenticationUtility.CredentialResponse, out clientResponse);
            credentials.TryGetValue(DigestAuthenticationUtility.CredentialNonceCount, out nonceCount);
            if (credentials.TryGetValue(DigestAuthenticationUtility.CredentialNonce, out nonce))
            {
                result = null;
                Doer<string, TimeSpan, bool> nonceExpiredParser = NonceExpiredParserCallback ?? DigestAuthenticationUtility.DefaultNonceExpiredParser;
                bool staleNonce = nonceExpiredParser(nonce, TimeSpan.FromSeconds(30));
                context.Context.Items["staleNonce"] = staleNonce.ToString().ToUpperInvariant();
                if (staleNonce) { return false; }

                lock (NonceCounter)
                {
                    Template<DateTime, string> previousNonce;
                    if (NonceCounter.TryGetValue(nonce, out previousNonce))
                    {
                        if (previousNonce.Arg2.Equals(nonceCount, StringComparison.Ordinal)) { return false; }
                    }
                    else
                    {
                        NonceCounter.Add(nonce, TupleUtility.CreateTwo(DateTime.UtcNow, nonceCount));
                    }
                }
            }
            result = ValidateCredentialsCallback(userName, out password);
            string ha1 = DigestAuthenticationUtility.ComputeHash1(credentials, password, Algorithm);
            string ha2 = DigestAuthenticationUtility.ComputeHash2(credentials, context.Request.HttpMethod, Algorithm);
            string serverResponse = DigestAuthenticationUtility.ComputeResponse(credentials, ha1, ha2, Algorithm);
            return serverResponse.Equals(clientResponse, StringComparison.Ordinal) && Condition.IsNotNull(result);
        }

        private Dictionary<string, string> AuthorizationParser(string authorizationHeader)
        {
            if (AuthenticationUtility.IsAuthenticationSchemeValid(authorizationHeader, AuthenticationSchemeName))
            {
                string digestCredentials = authorizationHeader.Remove(0, AuthenticationSchemeName.Length + 1);
                string[] credentials = digestCredentials.Split(AuthenticationUtility.DigestAuthenticationCredentialSeparator);
                if (IsDigestCredentialsValid(credentials))
                {
                    Dictionary<string, string> result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    for (int i = 0; i < credentials.Length; i++)
                    {
                        string[] credentialPair = StringUtility.Split(credentials[i], "=");
                        result.Add(credentialPair[0].Trim(), ConvertUtility.ParseWith(credentialPair[1], QuotedStringParser));
                    }
                    return IsDigestCredentialsValid(result) ? result : null;
                }
            }
            return null;
        }

        private static bool IsDigestCredentialsValid(Dictionary<string, string> credentials)
        {
            bool valid = credentials.ContainsKey("username");
            valid |= credentials.ContainsKey("realm");
            valid |= credentials.ContainsKey("nonce");
            valid |= credentials.ContainsKey("uri");
            valid |= credentials.ContainsKey("response");
            return valid;
        }

        private string QuotedStringParser(string value)
        {
            if (value.StartsWith("\"", StringComparison.OrdinalIgnoreCase) &&
                value.EndsWith("\"", StringComparison.OrdinalIgnoreCase))
            {
                value = value.Trim('"');
            }
            return value.Trim();
        }

        private static bool IsDigestCredentialsValid(string[] credentials)
        {
            bool valid = (credentials.Length >= 5 && credentials.Length <= 10);
            for (int i = 0; i < credentials.Length; i++)
            {
                valid |= !string.IsNullOrEmpty(credentials[i]);
            }
            return valid;
        }
    }
}