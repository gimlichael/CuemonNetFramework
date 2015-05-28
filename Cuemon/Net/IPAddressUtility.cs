using System;
using System.Net;
using System.Net.Sockets;
using Cuemon.Diagnostics;
namespace Cuemon.Net
{
    /// <summary>
    /// This utility class is designed to make IP related operations easier to work with.
    /// </summary>
    public static class IPAddressUtility
    {
        /// <summary>
        /// Determines whether the specified host address is an address for IP version 6.
        /// </summary>
        /// <param name="hostAddress">The host IP address to resolve.</param>
        /// <returns>
        /// 	<c>true</c> if the specified host address is an address for IP version 6; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsIPv6Address(string hostAddress)
        {
            return IsIPAddressVersion(hostAddress, AddressFamily.InterNetworkV6);
        }

        /// <summary>
        /// Determines whether the specified host address is an IP address for version 4.
        /// </summary>
        /// <param name="hostAddress">The host IP address to resolve.</param>
        /// <returns>
        /// 	<c>true</c> if the specified host address is an IP address for version 4; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsIPv4Address(string hostAddress)
        {
            return IsIPAddressVersion(hostAddress, AddressFamily.InterNetwork);
        }

        /// <summary>
        /// Determines whether the specified host address matches a local IP address.
        /// </summary>
        /// <param name="hostAddress">The IP address to match against (can be in either IPv4 dotted-quad or in IPv6 colon-hexadecimal notation) .</param>
        /// <returns>
        /// 	<c>true</c> if the specified host address matches a local IP address; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsLocal(string hostAddress)
        {
            if (string.IsNullOrEmpty(hostAddress)) { throw new ArgumentNullException("hostAddress", "The given argument cannot be null or empty!"); }
            hostAddress = hostAddress.Trim();
            return (hostAddress == "127.0.0.1" || hostAddress == "::1");
        }

        private static bool IsIPAddressVersion(string hostAddress, AddressFamily addressFamily)
        {
            if (string.IsNullOrEmpty(hostAddress)) { throw new ArgumentNullException("hostAddress", "The given argument cannot be null or empty!"); }
            if (!hostAddress.Contains(".") && !hostAddress.Contains(":")) { throw new ArgumentException("The given argument appears to have an invalid format!", "hostAddress"); }
            hostAddress = hostAddress.Trim();
            IPAddress ipAddress = IPAddress.Parse(hostAddress);
            return (ipAddress.AddressFamily == addressFamily);
        }
    }
}