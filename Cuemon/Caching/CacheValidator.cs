using System;
using System.Collections.Generic;
using System.Reflection;
using Cuemon.Security.Cryptography;

namespace Cuemon.Caching
{
    /// <summary>
    /// Specifies the validation strength of a cache checksum.
    /// </summary>
    public enum ChecksumStrength
    {
        /// <summary>
        /// Indicates that no checksum was specified.
        /// </summary>
        None,
        /// <summary>
        /// Indicates that a weak, semantic equivalent checksum was specified.
        /// </summary>
        Weak,
        /// <summary>
        /// Indicates that a strong, byte-for-byte checksum was specified.
        /// </summary>
        Strong
    }

    /// <summary>
    /// Specifies ways for the checksum to be computed.
    /// </summary>
    public enum ChecksumMethod
    {
        /// <summary>
        /// Indicates default behavior which is leaving the checksum unaltered.
        /// </summary>
        Default,
        /// <summary>
        /// Indicates that a checksum is combined from all given input and hence always will be available.
        /// </summary>
        Combined
    }

    /// <summary>
    /// Provides a way to represent cacheable data-centric content that can be validated by cache-aware applications.
    /// </summary>
    public class CacheValidator
    {
        private static readonly CacheValidator defaultCacheValidatorValue = new CacheValidator(DateTime.MinValue, DateTime.MinValue);
        private static CacheValidator referencePointCacheValidator;
        private static Assembly assemblyValue = typeof(CacheValidator).Assembly;
        private static HashAlgorithmType algorithmTypeValue = HashAlgorithmType.MD5;
        private const long NullOrZeroLengthChecksum = 23719;

        /// <summary>
        /// Gets or sets the <see cref="System.Reflection.Assembly"/> that will serve as the ideal candidate for a <see cref="CacheValidator"/> reference point. Default is <see cref="Cuemon"/>.
        /// </summary>
        /// <value>The assembly to use as a <see cref="CacheValidator"/> reference point.</value>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="value"/> is null.
        /// </exception>
        public static Assembly Assembly
        {
            get { return assemblyValue; }
            set
            {
                if (assemblyValue == null) { throw new ArgumentNullException("value"); }
                assemblyValue = value;
                referencePointCacheValidator = null;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheValidator"/> class.
        /// </summary>
        /// <param name="created">A <see cref="DateTime"/> value for when data this instance represents was first created.</param>
        public CacheValidator(DateTime created) 
            : this(created, ChecksumMethod.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheValidator"/> class.
        /// </summary>
        /// <param name="created">A <see cref="DateTime"/> value for when data this instance represents was first created.</param>
        /// <param name="method">One of the enumeration values of <see cref="ChecksumMethod"/> that specifies the result of <see cref="Checksum"/>.</param>
        public CacheValidator(DateTime created, ChecksumMethod method)
            : this(created, created, method)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheValidator"/> class.
        /// </summary>
        /// <param name="created">A <see cref="DateTime"/> value for when data this instance represents was first created.</param>
        /// <param name="modified">A <see cref="DateTime"/> value for when data this instance represents was last modified.</param>
        public CacheValidator(DateTime created, DateTime modified)
            : this(created, modified, ChecksumMethod.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheValidator"/> class.
        /// </summary>
        /// <param name="created">A <see cref="DateTime"/> value for when data this instance represents was first created.</param>
        /// <param name="modified">A <see cref="DateTime"/> value for when data this instance represents was last modified.</param>
        /// <param name="method">One of the enumeration values of <see cref="ChecksumMethod"/> that specifies the result of <see cref="Checksum"/>.</param>
        public CacheValidator(DateTime created, DateTime modified, ChecksumMethod method)
            : this(created, modified, (byte[])null, method)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheValidator"/> class.
        /// </summary>
        /// <param name="created">A <see cref="DateTime"/> value for when data this instance represents was first created.</param>
        /// <param name="modified">A <see cref="DateTime"/> value for when data this instance represents was last modified.</param>
        /// <param name="checksum">A <see cref="Double"/> value containing a byte-for-byte checksum of the data this instance represents.</param>
        public CacheValidator(DateTime created, DateTime modified, double checksum)
            : this(created, modified, checksum, ChecksumMethod.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheValidator"/> class.
        /// </summary>
        /// <param name="created">A <see cref="DateTime"/> value for when data this instance represents was first created.</param>
        /// <param name="modified">A <see cref="DateTime"/> value for when data this instance represents was last modified.</param>
        /// <param name="checksum">A <see cref="Double"/> value containing a byte-for-byte checksum of the data this instance represents.</param>
        /// <param name="method">One of the enumeration values of <see cref="ChecksumMethod"/> that specifies the result of <see cref="Checksum"/>.</param>
        public CacheValidator(DateTime created, DateTime modified, double checksum, ChecksumMethod method)
            : this(created, modified, ConvertUtility.ToByteArray(checksum), method)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheValidator"/> class.
        /// </summary>
        /// <param name="created">A <see cref="DateTime"/> value for when data this instance represents was first created.</param>
        /// <param name="modified">A <see cref="DateTime"/> value for when data this instance represents was last modified.</param>
        /// <param name="checksum">A <see cref="Int16"/> value containing a byte-for-byte checksum of the data this instance represents.</param>
        public CacheValidator(DateTime created, DateTime modified, short checksum) 
            : this(created, modified, checksum, ChecksumMethod.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheValidator"/> class.
        /// </summary>
        /// <param name="created">A <see cref="DateTime"/> value for when data this instance represents was first created.</param>
        /// <param name="modified">A <see cref="DateTime"/> value for when data this instance represents was last modified.</param>
        /// <param name="checksum">A <see cref="Int16"/> value containing a byte-for-byte checksum of the data this instance represents.</param>
        /// <param name="method">One of the enumeration values of <see cref="ChecksumMethod"/> that specifies the result of <see cref="Checksum"/>.</param>
        public CacheValidator(DateTime created, DateTime modified, short checksum, ChecksumMethod method)
            : this(created, modified, ConvertUtility.ToByteArray(checksum), method)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheValidator"/> class.
        /// </summary>
        /// <param name="created">A <see cref="DateTime"/> value for when data this instance represents was first created.</param>
        /// <param name="modified">A <see cref="DateTime"/> value for when data this instance represents was last modified.</param>
        /// <param name="checksum">A <see cref="String"/> value containing a byte-for-byte checksum of the data this instance represents.</param>
        public CacheValidator(DateTime created, DateTime modified, string checksum)
            : this(created, modified, checksum, ChecksumMethod.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheValidator"/> class.
        /// </summary>
        /// <param name="created">A <see cref="DateTime"/> value for when data this instance represents was first created.</param>
        /// <param name="modified">A <see cref="DateTime"/> value for when data this instance represents was last modified.</param>
        /// <param name="checksum">A <see cref="String"/> value containing a byte-for-byte checksum of the data this instance represents.</param>
        /// <param name="method">One of the enumeration values of <see cref="ChecksumMethod"/> that specifies the result of <see cref="Checksum"/>.</param>
        public CacheValidator(DateTime created, DateTime modified, string checksum, ChecksumMethod method)
            : this(created, modified, checksum == null ? StructUtility.HashCodeForNullValue : StructUtility.GetHashCode64(checksum), method)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheValidator"/> class.
        /// </summary>
        /// <param name="created">A <see cref="DateTime"/> value for when data this instance represents was first created.</param>
        /// <param name="modified">A <see cref="DateTime"/> value for when data this instance represents was last modified.</param>
        /// <param name="checksum">A <see cref="Int32"/> value containing a byte-for-byte checksum of the data this instance represents.</param>
        public CacheValidator(DateTime created, DateTime modified, int checksum) 
            : this(created, modified, checksum, ChecksumMethod.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheValidator"/> class.
        /// </summary>
        /// <param name="created">A <see cref="DateTime"/> value for when data this instance represents was first created.</param>
        /// <param name="modified">A <see cref="DateTime"/> value for when data this instance represents was last modified.</param>
        /// <param name="checksum">A <see cref="Int32"/> value containing a byte-for-byte checksum of the data this instance represents.</param>
        /// <param name="method">One of the enumeration values of <see cref="ChecksumMethod"/> that specifies the result of <see cref="Checksum"/>.</param>
        public CacheValidator(DateTime created, DateTime modified, int checksum, ChecksumMethod method)
            : this(created, modified, ConvertUtility.ToByteArray(checksum), method)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheValidator"/> class.
        /// </summary>
        /// <param name="created">A <see cref="DateTime"/> value for when data this instance represents was first created.</param>
        /// <param name="modified">A <see cref="DateTime"/> value for when data this instance represents was last modified.</param>
        /// <param name="checksum">A <see cref="Int64"/> value containing a byte-for-byte checksum of the data this instance represents.</param>
        public CacheValidator(DateTime created, DateTime modified, long checksum)
            : this(created, modified, checksum, ChecksumMethod.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheValidator"/> class.
        /// </summary>
        /// <param name="created">A <see cref="DateTime"/> value for when data this instance represents was first created.</param>
        /// <param name="modified">A <see cref="DateTime"/> value for when data this instance represents was last modified.</param>
        /// <param name="checksum">A <see cref="Int64"/> value containing a byte-for-byte checksum of the data this instance represents.</param>
        /// <param name="method">One of the enumeration values of <see cref="ChecksumMethod"/> that specifies the result of <see cref="Checksum"/>.</param>
        public CacheValidator(DateTime created, DateTime modified, long checksum, ChecksumMethod method)
            : this(created, modified, ConvertUtility.ToByteArray(checksum), method)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheValidator"/> class.
        /// </summary>
        /// <param name="created">A <see cref="DateTime"/> value for when data this instance represents was first created.</param>
        /// <param name="modified">A <see cref="DateTime"/> value for when data this instance represents was last modified.</param>
        /// <param name="checksum">A <see cref="Single"/> value containing a byte-for-byte checksum of the data this instance represents.</param>
        public CacheValidator(DateTime created, DateTime modified, float checksum)
            : this(created, modified, checksum, ChecksumMethod.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheValidator"/> class.
        /// </summary>
        /// <param name="created">A <see cref="DateTime"/> value for when data this instance represents was first created.</param>
        /// <param name="modified">A <see cref="DateTime"/> value for when data this instance represents was last modified.</param>
        /// <param name="checksum">A <see cref="Single"/> value containing a byte-for-byte checksum of the data this instance represents.</param>
        /// <param name="method">One of the enumeration values of <see cref="ChecksumMethod"/> that specifies the result of <see cref="Checksum"/>.</param>
        public CacheValidator(DateTime created, DateTime modified, float checksum, ChecksumMethod method)
            : this(created, modified, ConvertUtility.ToByteArray(checksum), method)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheValidator"/> class.
        /// </summary>
        /// <param name="created">A <see cref="DateTime"/> value for when data this instance represents was first created.</param>
        /// <param name="modified">A <see cref="DateTime"/> value for when data this instance represents was last modified.</param>
        /// <param name="checksum">A <see cref="UInt16"/> value containing a byte-for-byte checksum of the data this instance represents.</param>
        public CacheValidator(DateTime created, DateTime modified, ushort checksum)
            : this(created, modified, checksum, ChecksumMethod.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheValidator"/> class.
        /// </summary>
        /// <param name="created">A <see cref="DateTime"/> value for when data this instance represents was first created.</param>
        /// <param name="modified">A <see cref="DateTime"/> value for when data this instance represents was last modified.</param>
        /// <param name="checksum">A <see cref="UInt16"/> value containing a byte-for-byte checksum of the data this instance represents.</param>
        /// <param name="method">One of the enumeration values of <see cref="ChecksumMethod"/> that specifies the result of <see cref="Checksum"/>.</param>
        public CacheValidator(DateTime created, DateTime modified, ushort checksum, ChecksumMethod method)
            : this(created, modified, ConvertUtility.ToByteArray(checksum), method)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheValidator"/> class.
        /// </summary>
        /// <param name="created">A <see cref="DateTime"/> value for when data this instance represents was first created.</param>
        /// <param name="modified">A <see cref="DateTime"/> value for when data this instance represents was last modified.</param>
        /// <param name="checksum">A <see cref="UInt32"/> value containing a byte-for-byte checksum of the data this instance represents.</param>
        public CacheValidator(DateTime created, DateTime modified, uint checksum) 
            : this(created, modified, checksum, ChecksumMethod.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheValidator"/> class.
        /// </summary>
        /// <param name="created">A <see cref="DateTime"/> value for when data this instance represents was first created.</param>
        /// <param name="modified">A <see cref="DateTime"/> value for when data this instance represents was last modified.</param>
        /// <param name="checksum">A <see cref="UInt32"/> value containing a byte-for-byte checksum of the data this instance represents.</param>
        /// <param name="method">One of the enumeration values of <see cref="ChecksumMethod"/> that specifies the result of <see cref="Checksum"/>.</param>
        public CacheValidator(DateTime created, DateTime modified, uint checksum, ChecksumMethod method)
            : this(created, modified, ConvertUtility.ToByteArray(checksum), method)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheValidator"/> class.
        /// </summary>
        /// <param name="created">A <see cref="DateTime"/> value for when data this instance represents was first created.</param>
        /// <param name="modified">A <see cref="DateTime"/> value for when data this instance represents was last modified.</param>
        /// <param name="checksum">A <see cref="UInt64"/> value containing a byte-for-byte checksum of the data this instance represents.</param>
        public CacheValidator(DateTime created, DateTime modified, ulong checksum) 
            : this(created, modified, checksum, ChecksumMethod.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheValidator"/> class.
        /// </summary>
        /// <param name="created">A <see cref="DateTime"/> value for when data this instance represents was first created.</param>
        /// <param name="modified">A <see cref="DateTime"/> value for when data this instance represents was last modified.</param>
        /// <param name="checksum">A <see cref="UInt64"/> value containing a byte-for-byte checksum of the data this instance represents.</param>
        /// <param name="method">One of the enumeration values of <see cref="ChecksumMethod"/> that specifies the result of <see cref="Checksum"/>.</param>
        public CacheValidator(DateTime created, DateTime modified, ulong checksum, ChecksumMethod method)
            : this(created, modified, ConvertUtility.ToByteArray(checksum), method)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheValidator"/> class.
        /// </summary>
        /// <param name="created">A <see cref="DateTime"/> value for when data this instance represents was first created.</param>
        /// <param name="modified">A <see cref="DateTime"/> value for when data this instance represents was last modified.</param>
        /// <param name="checksum">An array of bytes containing a checksum of the data this instance represents.</param>
        public CacheValidator(DateTime created, DateTime modified, byte[] checksum)
            : this(created, modified, checksum, ChecksumMethod.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheValidator"/> class.
        /// </summary>
        /// <param name="created">A <see cref="DateTime"/> value for when data this instance represents was first created.</param>
        /// <param name="modified">A <see cref="DateTime"/> value for when data this instance represents was last modified.</param>
        /// <param name="checksum">An array of bytes containing a checksum of the data this instance represents.</param>
        /// <param name="method">One of the enumeration values of <see cref="ChecksumMethod"/> that specifies the result of <see cref="Checksum"/>.</param>
        public CacheValidator(DateTime created, DateTime modified, byte[] checksum, ChecksumMethod method)
        {
            this.Created = created.ToUniversalTime();
            this.Modified = modified.ToUniversalTime(); ;
            bool isChecksumNullOrZeroLength = (checksum == null || checksum.Length == 0);

            ChecksumStrength strength = isChecksumNullOrZeroLength ? ChecksumStrength.None : ChecksumStrength.Strong;
            switch (method)
            {
                case ChecksumMethod.Default:
                    break;
                case ChecksumMethod.Combined:
                    long checksumValue = isChecksumNullOrZeroLength ? NullOrZeroLengthChecksum : ByteUtility.GetHashCode(checksum);
                    checksum = ConvertUtility.ToByteArray(this.Created.Ticks ^ this.Modified.Ticks ^ checksumValue);
                    strength = ChecksumStrength.Weak;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("method");
            }
            this.Bytes = checksum == null ? new List<byte>() : new List<byte>(checksum);
            this.Strength = strength;
            this.Method = method;
        }

        /// <summary>
        /// Gets a <see cref="DateTime"/> value from when data this instance represents was first created, expressed as the Coordinated Universal Time (UTC).
        /// </summary>
        /// <value>A <see cref="DateTime"/> value from when data this instance represents was first created, expressed as the Coordinated Universal Time (UTC).</value>
        public DateTime Created
        {
            get; private set;
        }

        /// <summary>
        /// Gets a <see cref="DateTime"/> value from when data this instance represents was last modified, expressed as the Coordinated Universal Time (UTC).
        /// </summary>
        /// <value>A <see cref="DateTime"/> value from when data this instance represents was last modified, expressed as the Coordinated Universal Time (UTC).</value>
        public DateTime Modified
        {
            get; private set;
        }

        /// <summary>
        /// Gets a <see cref="String"/> containing a computed hash value (<see cref="AlgorithmType"/>) of the data this instance represents.
        /// </summary>
        /// <value>A <see cref="String"/> containing a computed hash value  (<see cref="AlgorithmType"/>) of the data this instance represents.</value>
        public string Checksum
        {
            get { return this.ComputedChecksum ?? (this.ComputedChecksum = HashUtility.ComputeHash(this.Bytes.ToArray(), AlgorithmType)); }
        }

        private string ComputedChecksum { get; set; }

        /// <summary>
        /// Gets a <see cref="CacheValidator"/> object that is initialized to a default representation that should be considered invalid for usage beyond this check.
        /// </summary>
        /// <value>A <see cref="CacheValidator"/> object that is initialized to a default representation.</value>
        public static CacheValidator Default
        {
            get { return defaultCacheValidatorValue.Clone(); }
        }

        /// <summary>
        /// Gets a <see cref="CacheValidator"/> object that represents an <see cref="System.Reflection.Assembly"/> reference point.
        /// </summary>
        /// <value>A <see cref="CacheValidator"/> object that represents an <see cref="System.Reflection.Assembly"/> reference point.</value>
        public static CacheValidator ReferencePoint
        {
            get
            {
                if (referencePointCacheValidator == null)
                {
                    referencePointCacheValidator = AssemblyUtility.GetCacheValidator(Assembly);
                }
                return referencePointCacheValidator.Clone();
            }
        }

        /// <summary>
        /// Gets a byte array that is the result of the associated <see cref="CacheValidator"/>.
        /// </summary>
        /// <value>The byte array that is the result of the associated <see cref="CacheValidator"/>.</value>
        internal List<byte> Bytes { get; set; }

        /// <summary>
        /// Gets or sets the hash algorithm to use for the checksum computation. Default is <see cref="HashAlgorithmType.MD5"/>.
        /// </summary>
        /// <value>The hash algorithm to use for the checksum computation.</value>
        public static HashAlgorithmType AlgorithmType
        {
            get { return algorithmTypeValue; }
            set { algorithmTypeValue = value; }
        }

        /// <summary>
        /// Gets an enumeration value of <see cref="ChecksumMethod"/> indicating the usage method of this instance.
        /// </summary>
        /// <value>One of the enumeration values of <see cref="ChecksumMethod"/> that indicates the usage method of this instance.</value>
        public ChecksumMethod Method
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets an enumeration value of <see cref="ChecksumStrength"/> indicating the strength of this instance.
        /// </summary>
        /// <value>One of the enumeration values of <see cref="ChecksumStrength"/> that specifies the strength of this instance.</value>
        public ChecksumStrength Strength
        {
            get;
            private set;
        }

        /// <summary>
        /// Returns a computed checksum that represents the <see cref="Bytes"/> of this instance.
        /// </summary>
        /// <returns>A computed checksum that represents the <see cref="Bytes"/> of this instance.</returns>
        public override string ToString()
        {
            return Checksum;
        }

        /// <summary>
        /// Creates a shallow copy of the current <see cref="CacheValidator"/> object.
        /// </summary>
        /// <returns>A new <see cref="CacheValidator"/> that is a copy of this instance.</returns>
        public virtual CacheValidator Clone()
        {
            return new CacheValidator(this.Created, this.Modified, this.Bytes.ToArray());
        }

        /// <summary>
        /// Gets the most significant (largest) value of either <see cref="Created"/> or <see cref="Modified"/>.
        /// </summary>
        /// <returns>The most significant (largest) value of either <see cref="Created"/> or <see cref="Modified"/>.</returns>
        public DateTime GetMostSignificant()
        {
            return DateTimeUtility.GetHighestValue(this.Created, this.Modified);
        }

        /// <summary>
        /// Combines the <paramref name="additionalChecksum"/> to the representation of this instance.
        /// </summary>
        /// <param name="additionalChecksum">A <see cref="Double"/> array that contains zero or more checksums of the additional data this instance must represent.</param>
        public CacheValidator CombineWith(params double[] additionalChecksum)
        {
            return this.CombineWith(ConvertUtility.ToByteArray(additionalChecksum as IEnumerable<double>));
        }

        /// <summary>
        /// Combines the <paramref name="additionalChecksum"/> to the representation of this instance.
        /// </summary>
        /// <param name="additionalChecksum">An <see cref="Int16"/> array that contains zero or more checksums of the additional data this instance must represent.</param>
        public CacheValidator CombineWith(params short[] additionalChecksum)
        {
            return this.CombineWith(ConvertUtility.ToByteArray(additionalChecksum as IEnumerable<short>));
        }

        /// <summary>
        /// Combines the <paramref name="additionalChecksum"/> to the representation of this instance.
        /// </summary>
        /// <param name="additionalChecksum">A <see cref="String"/> array that contains zero or more checksums of the additional data this instance must represent.</param>
        public CacheValidator CombineWith(params string[] additionalChecksum)
        {
            List<long> result = new List<long>();
            if (additionalChecksum == null)
            {
                result.Add(StructUtility.HashCodeForNullValue);
            }
            else
            {
                for (int i = 0; i < additionalChecksum.Length; i++)
                {
                    result.Add(StructUtility.GetHashCode64(additionalChecksum[i]));
                }
            }
            return this.CombineWith(result.ToArray());
        }

        /// <summary>
        /// Combines the <paramref name="additionalChecksum"/> to the representation of this instance.
        /// </summary>
        /// <param name="additionalChecksum">An <see cref="Int32"/> array that contains zero or more checksums of the additional data this instance must represent.</param>
        public CacheValidator CombineWith(params int[] additionalChecksum)
        {
            return this.CombineWith(ConvertUtility.ToByteArray(additionalChecksum as IEnumerable<int>));
        }

        /// <summary>
        /// Combines the <paramref name="additionalChecksum"/> to the representation of this instance.
        /// </summary>
        /// <param name="additionalChecksum">An <see cref="Int64"/> array that contains zero or more checksums of the additional data this instance must represent.</param>
        public CacheValidator CombineWith(params long[] additionalChecksum)
        {
            return this.CombineWith(ConvertUtility.ToByteArray(additionalChecksum as IEnumerable<long>));
        }

        /// <summary>
        /// Combines the <paramref name="additionalChecksum"/> to the representation of this instance.
        /// </summary>
        /// <param name="additionalChecksum">A <see cref="Single"/> array that contains zero or more checksums of the additional data this instance must represent.</param>
        public CacheValidator CombineWith(params float[] additionalChecksum)
        {
            return this.CombineWith(ConvertUtility.ToByteArray(additionalChecksum as IEnumerable<float>));
        }

        /// <summary>
        /// Combines the <paramref name="additionalChecksum"/> to the representation of this instance.
        /// </summary>
        /// <param name="additionalChecksum">An <see cref="UInt16"/> array that contains zero or more checksums of the additional data this instance must represent.</param>
        public CacheValidator CombineWith(params ushort[] additionalChecksum)
        {
            return this.CombineWith(ConvertUtility.ToByteArray(additionalChecksum as IEnumerable<ushort>));
        }

        /// <summary>
        /// Combines the <paramref name="additionalChecksum"/> to the representation of this instance.
        /// </summary>
        /// <param name="additionalChecksum">An <see cref="UInt32"/> array that contains zero or more checksums of the additional data this instance must represent.</param>
        public CacheValidator CombineWith(params uint[] additionalChecksum)
        {
            return this.CombineWith(ConvertUtility.ToByteArray(additionalChecksum as IEnumerable<uint>));
        }

        /// <summary>
        /// Combines the <paramref name="additionalChecksum"/> to the representation of this instance.
        /// </summary>
        /// <param name="additionalChecksum">An <see cref="UInt64"/> array that contains zero or more checksums of the additional data this instance must represent.</param>
        public CacheValidator CombineWith(params ulong[] additionalChecksum)
        {
            return this.CombineWith(ConvertUtility.ToByteArray(additionalChecksum as IEnumerable<ulong>));
        }

        /// <summary>
        /// Combines the <paramref name="additionalChecksum"/> to the representation of this instance.
        /// </summary>
        /// <param name="additionalChecksum">An array of bytes containing a checksum of the additional data this instance must represent.</param>
        public CacheValidator CombineWith(params byte[] additionalChecksum)
        {
            if (additionalChecksum == null) { return this; }
            if (additionalChecksum.Length == 0) { return this; }
            this.ComputedChecksum = null;
            this.Bytes.AddRange(additionalChecksum);
            return this;
        }

        /// <summary>
        /// Gets the most significant <see cref="CacheValidator"/> object from the most significant (largest) value of either <see cref="Created"/> or <see cref="Modified"/> in the specified <paramref name="sequence"/>.
        /// </summary>
        /// <param name="sequence">A sequence of  <see cref="CacheValidator"/> objects to parse for the most significant (largest) value of either <see cref="Created"/> or <see cref="Modified"/>.</param>
        /// <returns>The most significant <see cref="CacheValidator"/> object from the specified <paramref name="sequence"/>.</returns>
        public static CacheValidator GetMostSignificant(params CacheValidator[] sequence)
        {
            if (sequence == null) { throw new ArgumentNullException("sequence"); }
            CacheValidator mostSignificant = Default;
            foreach (CacheValidator candidate in sequence)
            {
                if (candidate.GetMostSignificant().Ticks > mostSignificant.GetMostSignificant().Ticks) { mostSignificant = candidate; }
            }
            return mostSignificant;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return this.Checksum.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object" /> to compare with the current <see cref="T:System.Object" />.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            CacheValidator validator = obj as CacheValidator;
            if (validator == null) { return false; }
            return this.Equals(validator); 
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><c>true</c> if the current object is equal to the other parameter; otherwise, <c>false</c>. </returns>
        public bool Equals(CacheValidator other)
        {
            if (other == null) { return false; }
            return (this.Checksum == other.Checksum);
        }
    }
}