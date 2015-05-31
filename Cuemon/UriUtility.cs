using System;
using System.Collections.Generic;
using System.Globalization;
using Cuemon.Collections.Generic;

namespace Cuemon
{
    /// <summary>
    /// This utility class is designed to make <see cref="Uri"/> operations easier to work with.
    /// </summary>
    public static class UriUtility
    {
        /// <summary>
        /// Gets all URI schemes currently supported by the .NET framework.
        /// </summary>
        /// <returns>A sequence of all URI schemes currently supported by the .NET framework.</returns>
        public static IEnumerable<UriScheme> AllUriSchemes
        {
            get
            {
                return EnumerableUtility.AsEnumerable(UriScheme.File, UriScheme.Ftp, UriScheme.Gopher, UriScheme.Http, UriScheme.Https, UriScheme.Mailto, UriScheme.NetPipe, UriScheme.NetTcp, UriScheme.News, UriScheme.Nntp);
            }
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
        /// <param name="uriSchemes">A sequence of <see cref="UriScheme"/> values to use in the validation of the URI.</param>
        /// <returns>
        /// 	<c>true</c> if the specified value evaluates to an absolute URI; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsUri(string value, IEnumerable<UriScheme> uriSchemes)
        {
            return IsUri(value, UriKind.Absolute, uriSchemes);
        }

        /// <summary>
        /// Determines whether the specified value is an URI string.
        /// </summary>
        /// <param name="value">The string value representing the URI.</param>
        /// <param name="uriKind">The type of the URI.</param>
        /// <param name="uriSchemes">A sequence of <see cref="UriScheme"/> values to use in the validation of the URI.</param>
        /// <returns>
        /// 	<c>true</c> if the specified value evaluates to an URI; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsUri(string value, UriKind uriKind, IEnumerable<UriScheme> uriSchemes)
        {
            Uri ignoreUri;
            return TryParse(value, uriKind, uriSchemes, out ignoreUri);
        }

        /// <summary>
        /// Determines whether an URI string contains one of the <see cref="UriScheme"/> values.
        /// </summary>
        /// <param name="value">The URI string to evaluate.</param>
        /// <param name="uriSchemes">An <see cref="Array"/> of <see cref="UriScheme"/> values.</param>
        /// <returns>
        /// 	<c>true</c> if the specified URI string contains one of the <see cref="UriScheme"/> values; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsScheme(string value, params UriScheme[] uriSchemes)
        {
            Uri uriOut;
            if (TryParse(value, UriKind.Absolute, uriSchemes, out uriOut))
            {
                return ContainsScheme(uriOut, uriSchemes);
            }
            return false;
        }

        /// <summary>
        /// Determines whether an <see cref="Uri"/> contains one of the <see cref="UriScheme"/> values. 
        /// </summary>
        /// <param name="value">The <see cref="Uri"/> to evaluate.</param>
        /// <param name="uriSchemes">An <see cref="Array"/> of <see cref="UriScheme"/> values.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="Uri"/> contains one of the <see cref="UriScheme"/> values; otherwise, <c>false</c>.
        /// </returns>
        public static bool ContainsScheme(Uri value, params UriScheme[] uriSchemes)
        {
            Validator.ThrowIfNull(value, "value");
            return EnumerableUtility.Contains<UriScheme>(uriSchemes, ParseScheme(value.Scheme));
        }

        /// <summary>
        /// Converts the specified string representation of an URI value to its <see cref="Uri"/> equivalent.
        /// </summary>
        /// <param name="uriString">A string containing the URI to convert.</param>
        /// <param name="uriKind">The type of the URI.</param>
        /// <param name="result">When this method returns, contains the constructed <see cref="Uri"/>.</param>
        /// <returns>
        ///     <c>true</c> if the <see cref="Uri"/> was successfully created; otherwise, false.
        /// </returns>
        /// <remarks>
        /// If this method returns true, the new <see cref="Uri"/> is in result.
        /// </remarks>
        public static bool TryParse(string uriString, UriKind uriKind, out Uri result)
        {
            return TryParse(uriString, uriKind, AllUriSchemes, out result);
        }

        /// <summary>
        /// Converts the specified string representation of an URI value to its <see cref="Uri"/> equivalent, limited to what is specified in the <see paramref="schemes"/> parameter.
        /// </summary>
        /// <param name="uriString">A string containing the URI to convert.</param>
        /// <param name="uriKind">The type of the URI.</param>
        /// <param name="uriSchemes">A sequence of <see cref="UriScheme"/> values to use in the parsing of the URI.</param>
        /// <param name="result">When this method returns, contains the constructed <see cref="Uri"/>.</param>
        /// <returns>
        ///     <c>true</c> if the <see cref="Uri"/> was successfully created; otherwise, false.
        /// </returns>
        /// <remarks>
        /// If this method returns true, the new <see cref="Uri"/> is in result.
        /// </remarks>
        public static bool TryParse(string uriString, UriKind uriKind, IEnumerable<UriScheme> uriSchemes, out Uri result)
        {
            return TesterDoerUtility.TryExecuteFunction(Parse, uriString, uriKind, uriSchemes, out result);
        }

        /// <summary>
        /// Converts the specified string representation of an URI value to its <see cref="Uri"/> equivalent, limited to what is specified in the <see paramref="schemes"/> parameter.
        /// </summary>
        /// <param name="uriString">A string containing the URI to convert.</param>
        /// <param name="uriKind">The type of the URI.</param>
        /// <returns>An <see cref="Uri"/> that is equivalent to the value contained in <paramref name="uriString"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="uriString"/> is null.
        /// </exception>
        /// <exception cref="ArgumentEmptyException">
        /// <paramref name="uriString"/> is empty.
        /// </exception>
        public static Uri Parse(string uriString, UriKind uriKind)
        {
            return Parse(uriString, uriKind, AllUriSchemes);
        }

        /// <summary>
        /// Converts the specified string representation of an URI value to its <see cref="Uri"/> equivalent, limited to what is specified in the <see paramref="schemes"/> parameter.
        /// </summary>
        /// <param name="uriString">A string containing the URI to convert.</param>
        /// <param name="uriKind">The type of the URI.</param>
        /// <param name="uriSchemes">A sequence of <see cref="UriScheme"/> values to use in the parsing of the URI.</param>
        /// <returns>An <see cref="Uri"/> that is equivalent to the value contained in <paramref name="uriString"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="uriString"/> is null - or - <paramref name="uriSchemes"/> is null.
        /// </exception>
        /// <exception cref="ArgumentEmptyException">
        /// <paramref name="uriString"/> is empty.
        /// </exception>
        public static Uri Parse(string uriString, UriKind uriKind, IEnumerable<UriScheme> uriSchemes)
        {
            Validator.ThrowIfNullOrEmpty(uriString, "uriString");
            Validator.ThrowIfNull(uriSchemes, "uriSchemes");

            bool isValid = false;
            string format = "{0}{1}";
            foreach (UriScheme scheme in uriSchemes)
            {
                switch (scheme)
                {
                    case UriScheme.File:
                        isValid = uriString.StartsWith(string.Format(CultureInfo.InvariantCulture, format, Uri.UriSchemeFile, Uri.SchemeDelimiter), StringComparison.OrdinalIgnoreCase);
                        break;
                    case UriScheme.Ftp:
                        isValid = uriString.StartsWith(string.Format(CultureInfo.InvariantCulture, format, Uri.UriSchemeFtp, Uri.SchemeDelimiter), StringComparison.OrdinalIgnoreCase);
                        break;
                    case UriScheme.Gopher:
                        isValid = uriString.StartsWith(string.Format(CultureInfo.InvariantCulture, format, Uri.UriSchemeGopher, Uri.SchemeDelimiter), StringComparison.OrdinalIgnoreCase);
                        break;
                    case UriScheme.Http:
                        isValid = uriString.StartsWith(string.Format(CultureInfo.InvariantCulture, format, Uri.UriSchemeHttp, Uri.SchemeDelimiter), StringComparison.OrdinalIgnoreCase);
                        break;
                    case UriScheme.Https:
                        isValid = uriString.StartsWith(string.Format(CultureInfo.InvariantCulture, format, Uri.UriSchemeHttps, Uri.SchemeDelimiter), StringComparison.OrdinalIgnoreCase);
                        break;
                    case UriScheme.Mailto:
                        isValid = uriString.StartsWith(string.Format(CultureInfo.InvariantCulture, format, Uri.UriSchemeMailto, Uri.SchemeDelimiter), StringComparison.OrdinalIgnoreCase);
                        break;
                    case UriScheme.NetPipe:
                        isValid = uriString.StartsWith(string.Format(CultureInfo.InvariantCulture, format, Uri.UriSchemeNetPipe, Uri.SchemeDelimiter), StringComparison.OrdinalIgnoreCase);
                        break;
                    case UriScheme.NetTcp:
                        isValid = uriString.StartsWith(string.Format(CultureInfo.InvariantCulture, format, Uri.UriSchemeNetTcp, Uri.SchemeDelimiter), StringComparison.OrdinalIgnoreCase);
                        break;
                    case UriScheme.News:
                        isValid = uriString.StartsWith(string.Format(CultureInfo.InvariantCulture, format, Uri.UriSchemeNews, Uri.SchemeDelimiter), StringComparison.OrdinalIgnoreCase);
                        break;
                    case UriScheme.Nntp:
                        isValid = uriString.StartsWith(string.Format(CultureInfo.InvariantCulture, format, Uri.UriSchemeNntp, Uri.SchemeDelimiter), StringComparison.OrdinalIgnoreCase);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("uriSchemes");
                }
                if (isValid) { break; }
            }

            Uri result;
            if (!isValid ||
                !Uri.TryCreate(uriString, uriKind, out result)) { throw new ArgumentException("The specified uriString is not a valid URI.", "uriString"); }
            return result;
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