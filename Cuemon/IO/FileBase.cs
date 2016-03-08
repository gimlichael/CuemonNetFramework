using System;
using System.IO;
using System.Net;
using Cuemon.Collections.Generic;
using Cuemon.Integrity;
using Cuemon.Net;

namespace Cuemon.IO
{
    /// <summary>
    /// Exposes an abstract interface for reading common properties of a file as well as content of the file.
    /// </summary>
    public abstract class FileBase
    {
        private CacheValidator _validation = null;
        private static readonly UriScheme[] ValidUriSchemes = EnumerableConverter.AsArray(UriScheme.File, UriScheme.Http, UriScheme.Ftp);

        #region Constructors
        private FileBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileBase"/> class.
        /// </summary>
        /// <param name="fileLocation">The location of the file to represent.</param>
        protected FileBase(string fileLocation) : this(new Uri(fileLocation))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileBase"/> class.
        /// </summary>
        /// <param name="fileLocation">The <see cref="Uri"/> of the file to represent.</param>
        protected FileBase(Uri fileLocation)
        {
            if (fileLocation == null) { throw new ArgumentNullException(nameof(fileLocation)); }
            if (!UriUtility.ContainsScheme(fileLocation, ValidUriSchemes)) { throw new ArgumentException("Only URI's with HTTP, FTP og File schemes is supported.", nameof(fileLocation)); }
            this.UriLocation = fileLocation;
            this.CanAccess = fileLocation.IsFile ? FileUtility.CanAccess(fileLocation.LocalPath, FileAccess.Read) : NetUtility.CanAccess(WebRequest.Create(fileLocation));
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether this file can be accessed.
        /// </summary>
        /// <value><c>true</c> if this file can be accessed; otherwise, <c>false</c>.</value>
        public bool CanAccess { get; private set; }

        /// <summary>
        /// Gets the location of the file as an URI.
        /// </summary>
        /// <value>The location of the file as an URI.</value>
        public Uri UriLocation { get; private set; }

        /// <summary>
        /// Gets a <see cref="CacheValidator" /> object that represents the content of the resource.
        /// </summary>
        /// <returns>A <see cref="CacheValidator" /> object that represents the content of the resource.</returns>
        /// <remarks>If unable to resolve the attributes of the file, the <see cref="CacheValidator" /> object of this property is initialized with <see cref="DateTime.MinValue" /> values.</remarks>
        public CacheValidator GetCacheValidator()
        {
            if (_validation == null)
            {
                if (this.CanAccess)
                {
                    WebRequest request;
                    DateTime created = DateTime.MinValue;
                    DateTime modified = DateTime.MinValue;
                    switch (UriSchemeConverter.FromString(this.UriLocation.Scheme))
                    {
                        case UriScheme.File:
                            FileInfo f = new FileInfo(this.UriLocation.LocalPath);
                            created = f.CreationTimeUtc;
                            modified = f.LastWriteTimeUtc;
                            break;
                        case UriScheme.Ftp:
                            request = WebRequest.Create(this.UriLocation);
                            request.Method = WebRequestMethods.Ftp.GetDateTimestamp;
                            using (FtpWebResponse response = NetUtility.GetFtpWebResponse((FtpWebRequest)request))
                            {
                                modified = response.LastModified.ToUniversalTime();
                                created = modified;
                            }
                            break;
                        case UriScheme.Http:
                            request = WebRequest.Create(this.UriLocation);
                            request.Method = WebRequestMethods.Http.Head;
                            using (HttpWebResponse response = NetUtility.GetHttpWebResponse((HttpWebRequest)request))
                            {
                                modified = response.LastModified.ToUniversalTime();
                                created = modified;
                            }
                            break;
                    }
                    _validation = new CacheValidator(created, modified);
                }
            }
            return _validation;
        }

        #endregion

        #region Methods
        /// <summary>
        /// Gets all the currently supported URI schemes for a file location.
        /// </summary>
        /// <returns>An <see cref="Array"/> of all the currently supported URI schemes for a file location.</returns>
        public static UriScheme[] GetValidUriSchemes()
        {
            return (UriScheme[])ValidUriSchemes.Clone();
        }

        /// <summary>
        /// Creates and returns a <see cref="Stream"/> representation of the file content.
        /// </summary>
        /// <returns>A <see cref="Stream"/> object.</returns>
        public Stream ToStream()
        {
            if (this.CanAccess)
            {
                if (this.UriLocation.IsFile) { return File.OpenRead(this.UriLocation.LocalPath); }
                switch (UriSchemeConverter.FromString(this.UriLocation.Scheme))
                {
                    case UriScheme.File:
                        using (FileWebResponse response = NetUtility.GetFileWebResponse((FileWebRequest)WebRequest.Create(this.UriLocation)))
                        {
                            return StreamUtility.CopyStream(response.GetResponseStream());
                        }
                    case UriScheme.Ftp:
                        using (FtpWebResponse response = NetUtility.GetFtpWebResponse((FtpWebRequest)WebRequest.Create(this.UriLocation)))
                        {
                            return StreamUtility.CopyStream(response.GetResponseStream());
                        }
                    case UriScheme.Http:
                        using (HttpWebResponse response = NetUtility.GetHttpWebResponse((HttpWebRequest)WebRequest.Create(this.UriLocation)))
                        {
                            return StreamUtility.CopyStream(response.GetResponseStream());
                        }
                }
            }

            throw new InvalidOperationException("The current state of this object indicates that the file cannot be accessed, hence it cannot be read.");
        }
        #endregion
    }
}