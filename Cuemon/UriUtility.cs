using System;
using System.Globalization;
using Cuemon.Collections.Generic;
using Cuemon.Diagnostics;
namespace Cuemon
{
    /// <summary>
    /// This utility class is designed to make <see cref="Uri"/> operations easier to work with.
    /// </summary>
    public static class UriUtility
    {
        private static readonly UriScheme[] AllUriSchemes = ConvertUtility.ToArray<UriScheme>(UriScheme.File, UriScheme.Ftp, UriScheme.Gopher, UriScheme.Http, UriScheme.Https, UriScheme.Mailto, UriScheme.NetPipe, UriScheme.NetTcp, UriScheme.News, UriScheme.Nntp);

        /// <summary>
        /// Gets all URI schemes currently supported by the .NET framework.
        /// </summary>
        /// <returns>An <see cref="Array"/> of all URI schemes currently supported by the .NET framework.</returns>
        public static UriScheme[] GetAllUriSchemes()
        {
            return (UriScheme[])AllUriSchemes.Clone();
        }

        /// <summary>
        /// Determines whether the specified value is an absolute URI string from all known URI schemes.
        /// </summary>
        /// <param name="value">The string value representing the URI.</param>
        /// <returns>
        /// 	<c>true</c> if the specified value evaluates to an URI; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsUri(string value)
        {
            return IsUri(value, UriKind.Absolute);
        }

        /// <summary>
        /// Determines whether the specified value is an URI string from all known URI schemes.
        /// </summary>
        /// <param name="value">The string value representing the URI.</param>
        /// <param name="uriKind">The type of the URI.</param>
        /// <returns>
        /// 	<c>true</c> if the specified value evaluates to an URI; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsUri(string value, UriKind uriKind)
        {
            return IsUri(value, uriKind, AllUriSchemes);
        }

        /// <summary>
        /// Determines whether the specified value is an absolute URI string.
        /// </summary>
        /// <param name="value">The string value representing the URI.</param>
        /// <param name="schemes">An array of <see cref="UriScheme"/> values to use in the validation of the URI.</param>
        /// <returns>
        /// 	<c>true</c> if the specified value evaluates to an absolute URI; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsUri(string value, UriScheme[] schemes)
        {
            return IsUri(value, UriKind.Absolute, schemes);
        }

        /// <summary>
        /// Determines whether the specified value is an URI string.
        /// </summary>
        /// <param name="value">The string value representing the URI.</param>
        /// <param name="uriKind">The type of the URI.</param>
        /// <param name="schemes">An array of <see cref="UriScheme"/> values to use in the validation of the URI.</param>
        /// <returns>
        /// 	<c>true</c> if the specified value evaluates to an URI; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsUri(string value, UriKind uriKind, UriScheme[] schemes)
        {
            Uri ignoreUri;
            return TryParse(value, uriKind, schemes, out ignoreUri);
        }

        /// <summary>
        /// Determines whether an URI string contains one of the <see cref="UriScheme"/> values.
        /// </summary>
        /// <param name="value">The URI string to evaluate.</param>
        /// <param name="schemes">An <see cref="Array"/> of <see cref="UriScheme"/> values.</param>
        /// <returns>
        /// 	<c>true</c> if the specified URI string contains one of the <see cref="UriScheme"/> values; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsScheme(string value, params UriScheme[] schemes)
        {
            Uri uriOut;
            if (TryParse(value, UriKind.Absolute, schemes, out uriOut))
            {
                return ContainsScheme(uriOut, schemes);
            }
            return false;
        }

        /// <summary>
        /// Determines whether an <see cref="Uri"/> contains one of the <see cref="UriScheme"/> values. 
        /// </summary>
        /// <param name="value">The <see cref="Uri"/> to evaluate.</param>
        /// <param name="schemes">An <see cref="Array"/> of <see cref="UriScheme"/> values.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="Uri"/> contains one of the <see cref="UriScheme"/> values; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsScheme(Uri value, params UriScheme[] schemes)
        {
            if (value == null) throw new ArgumentNullException("value");
            return EnumerableUtility.Contains<UriScheme>(schemes, UriUtility.ParseScheme(value.Scheme));
        }

        /// <summary>
        /// Converts the specified string representation of an URI value to its <see cref="Uri"/> equivalent.
        /// </summary>
        /// <param name="value">A string containing the URI to convert.</param>
        /// <param name="uriKind">The type of the URI.</param>
        /// <param name="result">When this method returns, contains the constructed <see cref="Uri"/>.</param>
        /// <returns>
        ///     <c>true</c> if the <see cref="Uri"/> was successfully created; otherwise, false.
        /// </returns>
        /// <remarks>
        /// If this method returns true, the new <see cref="Uri"/> is in result.
        /// </remarks>
        public static bool TryParse(string value, UriKind uriKind, out Uri result)
        {
            return TryParse(value, uriKind, AllUriSchemes, out result);
        }

        /// <summary>
        /// Converts the specified string representation of an URI value to its <see cref="Uri"/> equivalent, limited to what is specified in the <see paramref="schemes"/> parameter.
        /// </summary>
        /// <param name="value">A string containing the URI to convert.</param>
        /// <param name="uriKind">The type of the URI.</param>
        /// <param name="schemes">An array of <see cref="UriScheme"/> values to use in the parsing of the URI.</param>
        /// <param name="result">When this method returns, contains the constructed <see cref="Uri"/>.</param>
        /// <returns>
        ///     <c>true</c> if the <see cref="Uri"/> was successfully created; otherwise, false.
        /// </returns>
        /// <remarks>
        /// If this method returns true, the new <see cref="Uri"/> is in result.
        /// </remarks>
        public static bool TryParse(string value, UriKind uriKind, UriScheme[] schemes, out Uri result)
        {
            if (value == null) { throw new ArgumentNullException("value"); }
            if (schemes == null) throw new ArgumentNullException("schemes");
            if (schemes.Length == 0) { throw new ArgumentException("At least one UriScheme must be specified.", "schemes"); }

            bool isValid = false;
            string format = "{0}{1}";

            foreach (UriScheme scheme in schemes)
            {
                switch (scheme)
                {
                    case UriScheme.File:
                        isValid = value.StartsWith(string.Format(CultureInfo.InvariantCulture, format, Uri.UriSchemeFile, Uri.SchemeDelimiter), StringComparison.OrdinalIgnoreCase);
                        break;
                    case UriScheme.Ftp:
                        isValid = value.StartsWith(string.Format(CultureInfo.InvariantCulture, format, Uri.UriSchemeFtp, Uri.SchemeDelimiter), StringComparison.OrdinalIgnoreCase);
                        break;
                    case UriScheme.Gopher:
                        isValid = value.StartsWith(string.Format(CultureInfo.InvariantCulture, format, Uri.UriSchemeGopher, Uri.SchemeDelimiter), StringComparison.OrdinalIgnoreCase);
                        break;
                    case UriScheme.Http:
                        isValid = value.StartsWith(string.Format(CultureInfo.InvariantCulture, format, Uri.UriSchemeHttp, Uri.SchemeDelimiter), StringComparison.OrdinalIgnoreCase);
                        break;
                    case UriScheme.Https:
                        isValid = value.StartsWith(string.Format(CultureInfo.InvariantCulture, format, Uri.UriSchemeHttps, Uri.SchemeDelimiter), StringComparison.OrdinalIgnoreCase);
                        break;
                    case UriScheme.Mailto:
                        isValid = value.StartsWith(string.Format(CultureInfo.InvariantCulture, format, Uri.UriSchemeMailto, Uri.SchemeDelimiter), StringComparison.OrdinalIgnoreCase);
                        break;
                    case UriScheme.NetPipe:
                        isValid = value.StartsWith(string.Format(CultureInfo.InvariantCulture, format, Uri.UriSchemeNetPipe, Uri.SchemeDelimiter), StringComparison.OrdinalIgnoreCase);
                        break;
                    case UriScheme.NetTcp:
                        isValid = value.StartsWith(string.Format(CultureInfo.InvariantCulture, format, Uri.UriSchemeNetTcp, Uri.SchemeDelimiter), StringComparison.OrdinalIgnoreCase);
                        break;
                    case UriScheme.News:
                        isValid = value.StartsWith(string.Format(CultureInfo.InvariantCulture, format, Uri.UriSchemeNews, Uri.SchemeDelimiter), StringComparison.OrdinalIgnoreCase);
                        break;
                    case UriScheme.Nntp:
                        isValid = value.StartsWith(string.Format(CultureInfo.InvariantCulture, format, Uri.UriSchemeNntp, Uri.SchemeDelimiter), StringComparison.OrdinalIgnoreCase);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("schemes");
                }
                if (isValid) { break; }
            }

            result = null;
            if (isValid)
            {
                isValid = Uri.TryCreate(value, uriKind, out result); // so far, the uri seems valid - lets make sure before we say it is
            }
            return isValid;
        }

        /// <summary>
        /// Converts the specified string representation of an URI scheme to its <see cref="UriScheme"/> equivalent.
        /// </summary>
        /// <param name="scheme">A string containing an URI scheme to convert.</param>
        /// <returns>A <see cref="UriScheme"/> equivalent to the URI scheme contained in <c>scheme</c>.</returns>
        public static UriScheme ParseScheme(string scheme)
        {
            if (scheme == null) { throw new ArgumentNullException("scheme"); }
            if (scheme.Length == 0) { throw new ArgumentException("The URI scheme cannot be empty.", "scheme"); }
            if (scheme.Equals(Uri.UriSchemeFile, StringComparison.OrdinalIgnoreCase)) { return UriScheme.File; }
            if (scheme.Equals(Uri.UriSchemeFtp, StringComparison.OrdinalIgnoreCase)) { return UriScheme.Ftp; }
            if (scheme.Equals(Uri.UriSchemeGopher, StringComparison.OrdinalIgnoreCase)) { return UriScheme.Gopher; }
            if (scheme.Equals(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase)) { return UriScheme.Http; }
            if (scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase)) { return UriScheme.Https; }
            if (scheme.Equals(Uri.UriSchemeMailto, StringComparison.OrdinalIgnoreCase)) { return UriScheme.Mailto; }
            if (scheme.Equals(Uri.UriSchemeNetPipe, StringComparison.OrdinalIgnoreCase)) { return UriScheme.NetPipe; }
            if (scheme.Equals(Uri.UriSchemeNetTcp, StringComparison.OrdinalIgnoreCase)) { return UriScheme.NetTcp; }
            if (scheme.Equals(Uri.UriSchemeNews, StringComparison.OrdinalIgnoreCase)) { return UriScheme.News; }
            if (scheme.Equals(Uri.UriSchemeNntp, StringComparison.OrdinalIgnoreCase)) { return UriScheme.Nntp; }
            return UriScheme.Undefined;
        }
    }

    /// <summary>
    /// Defines the schemes available for an <see cref="Uri"/> class.
    /// </summary>
    public enum UriScheme
    {
        /// <summary>
        /// Specifies an undefined scheme.
        /// </summary>
        Undefined,
        /// <summary>
        /// Specifies that the URI is a pointer to a file.
        /// </summary>
        File,
        /// <summary>
        /// Specifies that the URI is accessed through the File Transfer Protocol (FTP).
        /// </summary>
        Ftp,
        /// <summary>
        /// Specifies that the URI is accessed through the Gopher protocol.
        /// </summary>
        Gopher,
        /// <summary>
        /// Specifies that the URI is accessed through the Hypertext Transfer Protocol (HTTP).
        /// </summary>
        Http,
        /// <summary>
        /// Specifies that the URI is accessed through the Secure Hypertext Transfer Protocol (HTTPS).
        /// </summary>
        Https,
        /// <summary>
        /// Specifies that the URI is an e-mail address and is accessed through the Simple Mail Transport Protocol (SMTP).
        /// </summary>
        Mailto,
        /// <summary>
        /// Specifies that the URI is accessed through the NetPipe scheme of the "Indigo" system.
        /// </summary>
        NetPipe,
        /// <summary>
        /// Specifies that the URI is accessed through the NetTcp scheme of the "Indigo" system.
        /// </summary>
        NetTcp,
        /// <summary>
        /// Specifies that the URI is an Internet news group and is accessed through the Network News Transport Protocol (NNTP).
        /// </summary>
        News,
        /// <summary>
        /// Specifies that the URI is an Internet news group and is accessed through the Network News Transport Protocol (NNTP).
        /// </summary>
        Nntp
    }
}