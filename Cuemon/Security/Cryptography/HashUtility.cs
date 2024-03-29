﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using Cuemon.Collections.Generic;
using Cuemon.IO;
using Cuemon.Runtime.Serialization;

namespace Cuemon.Security.Cryptography
{
    /// <summary>
    /// This utility class is designed to make <see cref="HashAlgorithm"/> operations easier to work with.
    /// </summary>
    public static class HashUtility
    {
        /// <summary>
        /// Computes a MD5 hash value of the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The object to compute a hash code for.</param>
        /// <returns>A <see cref="HashResult"/> containing the computed MD5 hash value of the specified <paramref name="value"/>.</returns>
        public static HashResult ComputeHash(object value)
        {
            return ComputeHash(value, HashAlgorithmType.MD5);
        }

        /// <summary>
        /// Computes a by parameter defined <see cref="HashAlgorithmType"/> hash value of the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The object to compute a hash code for.</param>
        /// <param name="algorithmType">The hash algorithm to use for the computation.</param>
        /// <returns>A <see cref="HashResult"/> containing the computed hash value of the specified <paramref name="value"/>.</returns>
        public static HashResult ComputeHash(object value, HashAlgorithmType algorithmType)
        {
            return ComputeHash(EnumerableConverter.AsArray(value), algorithmType);
        }

        /// <summary>
        /// Combines a sequence of objects into one object, and computes a MD5 hash value of the specified sequence, <paramref name="values"/>.
        /// </summary>
        /// <param name="values">The objects to compute a hash code for.</param>
        /// <returns>A <see cref="HashResult"/> containing the computed MD5 hash value of the specified object sequence <paramref name="values"/>.</returns>
        public static HashResult ComputeHash(object[] values)
        {
            return ComputeHash(values, HashAlgorithmType.MD5);
        }

        /// <summary>
        /// Combines a sequence of objects into one object, and computes a by parameter defined <see cref="HashAlgorithmType"/> hash value of the specified sequence, <paramref name="values"/>.
        /// </summary>
        /// <param name="values">The objects to compute a hash code for.</param>
        /// <param name="algorithmType">The hash algorithm to use for the computation.</param>
        /// <returns>A <see cref="HashResult"/> containing the computed hash value of the specified object sequence <paramref name="values"/>.</returns>
        public static HashResult ComputeHash(object[] values, HashAlgorithmType algorithmType)
        {
            if (values == null) { throw new ArgumentNullException(nameof(values)); }
            if (values.Length == 1) // only one object is needed for hashing; avoid the extra overhead by combining
            {
                if (values[0] == null) { throw new ArgumentNullException(nameof(values)); }
                return ComputeHash(SerializeObject(values[0]), algorithmType);
            }
            List<Stream> streams = new List<Stream>();
            foreach (object value in values)
            {
                if (value != null) { streams.Add(SerializeObject(value)); }
            }
            return ComputeHash(StreamUtility.CombineStreams(streams.ToArray()), algorithmType);
        }

        /// <summary>
        /// Computes a by parameter defined <see cref="HashAlgorithmType"/> hash value of the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The <see cref="Stream"/> object to compute a hash code for.</param>
        /// <returns>A <see cref="HashResult"/> containing the computed MD5 hash value of the specified <see cref="Stream"/> <paramref name="value"/>.</returns>
        public static HashResult ComputeHash(Stream value)
        {
            return ComputeHash(value, HashAlgorithmType.MD5);
        }

        /// <summary>
        /// Computes a by parameter defined <see cref="HashAlgorithmType"/> hash value of the specified <see cref="Stream"/> <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The <see cref="Stream"/> object to compute a hash code for.</param>
        /// <param name="algorithmType">The hash algorithm to use for the computation.</param>
        /// <returns>A <see cref="HashResult"/> containing the computed hash value of the specified <see cref="Stream"/> <paramref name="value"/>.</returns>
        public static HashResult ComputeHash(Stream value, HashAlgorithmType algorithmType)
        {
            return ComputeHash(value, algorithmType, false);
        }

        /// <summary>
        /// Computes a by parameter defined <see cref="HashAlgorithmType"/> hash value of the specified <see cref="Stream"/> <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The <see cref="Stream"/> object to compute a hash code for.</param>
        /// <param name="algorithmType">The hash algorithm to use for the computation.</param>
        /// <param name="leaveStreamOpen">if <c>true</c>, the <see cref="Stream"/> object is being left open; otherwise it is being closed and disposed.</param>
        /// <returns>A <see cref="HashResult"/> containing the computed hash value of the specified <see cref="Stream"/> <paramref name="value"/>.</returns>
        public static HashResult ComputeHash(Stream value, HashAlgorithmType algorithmType, bool leaveStreamOpen)
        {
            return ComputeHashCore(value, null, algorithmType, leaveStreamOpen);
        }

        /// <summary>
        /// Computes a MD5 hash value of the specified <see cref="byte"/> sequence, <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The <see cref="byte"/> array to compute a hash code for.</param>
        /// <returns>A <see cref="HashResult"/> containing the computed MD5 hash value of the specified <see cref="byte"/> sequence <paramref name="value"/>.</returns>
        public static HashResult ComputeHash(byte[] value)
        {
            return ComputeHash(value, HashAlgorithmType.MD5);
        }

        /// <summary>
        /// Computes a by parameter defined <see cref="HashAlgorithmType"/> hash value of the specified <see cref="byte"/> sequence, <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The <see cref="byte"/> array to compute a hash code for.</param>
        /// <param name="algorithmType">The hash algorithm to use for the computation.</param>
        /// <returns>A <see cref="HashResult"/> containing the computed hash value of the specified <see cref="byte"/> sequence <paramref name="value"/>.</returns>
        public static HashResult ComputeHash(byte[] value, HashAlgorithmType algorithmType)
        {
            return ComputeHashCore(null, value, algorithmType, false);
        }

        /// <summary>
        /// Computes a MD5 hash value of the specified <see cref="string"/> <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The <see cref="string"/> value to compute a hash code for.</param>
        /// <returns>A <see cref="HashResult"/> containing the computed MD5 hash value of the specified <see cref="string"/> <paramref name="value"/>.</returns>
        public static HashResult ComputeHash(string value)
        {
            return ComputeHash(value, HashAlgorithmType.MD5);
        }

        /// <summary>
        /// Computes a by parameter defined <see cref="HashAlgorithmType"/> hash value of the specified <see cref="string"/> <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The <see cref="string"/> value to compute a hash code for.</param>
        /// <param name="algorithmType">The hash algorithm to use for the computation.</param>
        /// <returns>A <see cref="HashResult"/> containing the computed hash value of the specified <see cref="string"/> <paramref name="value"/>.</returns>
        public static HashResult ComputeHash(string value, HashAlgorithmType algorithmType)
        {
            return ComputeHash(value, algorithmType, Encoding.Unicode);
        }

        /// <summary>
        /// Computes a by parameter defined <see cref="HashAlgorithmType"/> hash value of the specified <see cref="string"/> <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The <see cref="string"/> value to compute a hash code for.</param>
        /// <param name="algorithmType">The hash algorithm to use for the computation.</param>
        /// <param name="encoding">The encoding to use when computing the <paramref name="value"/>.</param>
        /// <returns>A <see cref="HashResult"/> containing the computed hash value of the specified <see cref="string"/> <paramref name="value"/>.</returns>
        public static HashResult ComputeHash(string value, HashAlgorithmType algorithmType, Encoding encoding)
        {
            return ComputeHash(ByteConverter.FromString(value, options =>
            {
                options.Encoding = encoding;
                options.Preamble = PreambleSequence.Remove;
            }), algorithmType);
        }

        /// <summary>
        /// Computes a MD5 hash value of the specified <see cref="string"/> sequence, <paramref name="values"/>.
        /// </summary>
        /// <param name="values">The <see cref="string"/> sequence to compute a hash code for.</param>
        /// <returns>A <see cref="HashResult"/> containing the computed MD5 hash value of the specified <see cref="string"/> sequence, <paramref name="values"/>.</returns>
        public static HashResult ComputeHash(string[] values)
        {
            return ComputeHash(values, HashAlgorithmType.MD5);
        }

        /// <summary>
        /// Computes a by parameter defined <see cref="HashAlgorithmType"/> hash value of the specified <see cref="string"/> sequence, <paramref name="values"/>.
        /// </summary>
        /// <param name="values">The <see cref="string"/> sequence to compute a hash code for.</param>
        /// <param name="algorithmType">The hash algorithm to use for the computation.</param>
        /// <returns>A <see cref="HashResult"/> containing the computed hash value of the specified <see cref="string"/> sequence, <paramref name="values"/>.</returns>
        public static HashResult ComputeHash(string[] values, HashAlgorithmType algorithmType)
        {
            return ComputeHash(values, algorithmType, Encoding.Unicode);
        }

        /// <summary>
        /// Computes a by parameter defined <see cref="HashAlgorithmType"/> hash value of the specified <see cref="string"/> sequence, <paramref name="values"/>.
        /// </summary>
        /// <param name="values">The <see cref="string"/> sequence to compute a hash code for.</param>
        /// <param name="algorithmType">The hash algorithm to use for the computation.</param>
        /// <param name="encoding">The encoding to use when computing the <paramref name="values"/> sequence.</param>
        /// <returns>A <see cref="HashResult"/> containing the computed hash value of the specified <see cref="string"/> sequence, <paramref name="values"/>.</returns>
        public static HashResult ComputeHash(string[] values, HashAlgorithmType algorithmType, Encoding encoding)
        {
            if (values == null) { throw new ArgumentNullException(nameof(values)); }
            MemoryStream tempStream = null;
            try
            {
                tempStream = new MemoryStream();
                using (StreamWriter writer = new StreamWriter(tempStream, encoding))
                {
                    foreach (string value in values)
                    {
                        if (value == null) { continue; }
                        writer.Write(value);
                    }
                    writer.Flush();
                    tempStream.Position = 0;
                    Stream stream = StreamConverter.RemovePreamble(tempStream, encoding);
                    tempStream = null;
                    return ComputeHash(stream, algorithmType);
                }
            }
            finally
            {
                if (tempStream != null) { tempStream.Dispose(); }
            }
        }

        private static HashResult ComputeHashCore(Stream value, byte[] hash, HashAlgorithmType algorithmType, bool leaveStreamOpen)
        {
            if (algorithmType > HashAlgorithmType.CRC32 || algorithmType < HashAlgorithmType.MD5) { throw new ArgumentOutOfRangeException(nameof(algorithmType), "Specified argument was out of the range of valid values."); }

            HashAlgorithm algorithm;
            switch (algorithmType)
            {
                case HashAlgorithmType.MD5:
                    goto default;
                case HashAlgorithmType.SHA1:
                    algorithm = new SHA1CryptoServiceProvider();
                    break;
                case HashAlgorithmType.SHA256:
                    algorithm = new SHA256Managed();
                    break;
                case HashAlgorithmType.SHA384:
                    algorithm = new SHA384Managed();
                    break;
                case HashAlgorithmType.SHA512:
                    algorithm = new SHA512Managed();
                    break;
                case HashAlgorithmType.RIPEMD160:
                    algorithm = new RIPEMD160Managed();
                    break;
                case HashAlgorithmType.CRC32:
                    algorithm = new CyclicRedundancyCheck32(PolynomialRepresentation.Reversed);
                    break;
                default:
                    algorithm = new MD5CryptoServiceProvider();
                    break;
            }

            return ComputeHashCore(value, hash, leaveStreamOpen, algorithm);
        }

        internal static HashResult ComputeHashCore(Stream value, byte[] hash, bool leaveStreamOpen, HashAlgorithm algorithm)
        {
            using (algorithm)
            {
                if (value != null)
                {
                    long startingPosition = value.Position;

                    if (value.CanSeek) { value.Position = 0; }
                    hash = algorithm.ComputeHash(value);
                    if (value.CanSeek) { value.Seek(startingPosition, SeekOrigin.Begin); } // reset to original position

                    if (!leaveStreamOpen)
                    {
                        value.Dispose();
                    }
                }
                else
                {
                    hash = algorithm.ComputeHash(hash); // convert original byte value to hash value
                }
            }
            return new HashResult(hash);
        }

        internal static Stream SerializeObject(object value)
        {
            return SerializationUtility.SerializeAsStream(new BinaryFormatter(), value);
        }
    }
}