using System;
using System.IO;
using System.Web;

namespace Cuemon.Web
{
	/// <summary>
	/// An abstract class representing a <see cref="HttpResponse.Filter"/> whose backing store is memory.
	/// </summary>
	public abstract class HttpResponseFilter : Stream
	{
		private Stream _chainedOutputFilter;

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="HttpResponseFilter"/> class.
		/// </summary>
		/// <param name="context">The context of the ASP.NET application.</param>
		protected HttpResponseFilter(HttpApplication context) : base()
		{
			if (context == null) { throw new ArgumentNullException("context"); }
			_chainedOutputFilter = context.Response.Filter;
		}
		#endregion

		#region Properties
        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value><c>true</c> if this instance is disposed; otherwise, <c>false</c>.</value>
        protected bool IsDisposed { get; set; }

		/// <summary>
		/// Gets or sets an object that is used to modify the HTTP entity body before transmission.
		/// </summary>
		protected Stream ChainedOutputFilter
		{
			get { return _chainedOutputFilter; }
			set { _chainedOutputFilter = value; }
		}

		/// <summary>
		/// Gets a value indicating whether the current stream supports reading.
		/// </summary>
		/// <returns>true if the stream is open.
		///   </returns>
		public override bool CanRead
		{
			get { return this.ChainedOutputFilter.CanRead; }
		}

		/// <summary>
		/// Gets a value indicating whether the current stream supports seeking.
		/// </summary>
		/// <returns>true if the stream is open.
		///   </returns>
		public override bool CanSeek
		{
            get { return this.ChainedOutputFilter.CanSeek; }
		}

		/// <summary>
		/// Gets a value indicating whether the current stream supports writing.
		/// </summary>
		/// <value></value>
		/// <returns>true if the stream supports writing; otherwise, false.
		/// </returns>
		public override bool CanWrite
		{
            get { return this.ChainedOutputFilter.CanWrite; }
		}
		#endregion

		#region Methods
		/// <summary>
		/// Begins an asynchronous read operation.
		/// </summary>
		/// <param name="buffer">The buffer to read the data into.</param>
		/// <param name="offset">The byte offset in <paramref name="buffer"/> at which to begin writing data read from the stream.</param>
		/// <param name="count">The maximum number of bytes to read.</param>
		/// <param name="callback">An optional asynchronous callback, to be called when the read is complete.</param>
		/// <param name="state">A user-provided object that distinguishes this particular asynchronous read request from other requests.</param>
		/// <returns>
		/// An <see cref="T:System.IAsyncResult"/> that represents the asynchronous read, which could still be pending.
		/// </returns>
		/// <exception cref="T:System.IO.IOException">
		/// Attempted an asynchronous read past the end of the stream, or a disk error occurs.
		///   </exception>
		///   
		/// <exception cref="T:System.ArgumentException">
		/// One or more of the arguments is invalid.
		///   </exception>
		///   
		/// <exception cref="T:System.ObjectDisposedException">
		/// Methods were called after the stream was closed.
		///   </exception>
		///   
		/// <exception cref="T:System.NotSupportedException">
		/// The current Stream implementation does not support the read operation.
		///   </exception>
		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Begins an asynchronous write operation.
		/// </summary>
		/// <param name="buffer">The buffer to write data from.</param>
		/// <param name="offset">The byte offset in <paramref name="buffer"/> from which to begin writing.</param>
		/// <param name="count">The maximum number of bytes to write.</param>
		/// <param name="callback">An optional asynchronous callback, to be called when the write is complete.</param>
		/// <param name="state">A user-provided object that distinguishes this particular asynchronous write request from other requests.</param>
		/// <returns>
		/// An IAsyncResult that represents the asynchronous write, which could still be pending.
		/// </returns>
		/// <exception cref="T:System.IO.IOException">
		/// Attempted an asynchronous write past the end of the stream, or a disk error occurs.
		///   </exception>
		///   
		/// <exception cref="T:System.ArgumentException">
		/// One or more of the arguments is invalid.
		///   </exception>
		///   
		/// <exception cref="T:System.ObjectDisposedException">
		/// Methods were called after the stream was closed.
		///   </exception>
		///   
		/// <exception cref="T:System.NotSupportedException">
		/// The current Stream implementation does not support the write operation.
		///   </exception>
		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Waits for the pending asynchronous read to complete.
		/// </summary>
		/// <param name="asyncResult">The reference to the pending asynchronous request to finish.</param>
		/// <returns>
		/// The number of bytes read from the stream, between zero (0) and the number of bytes you requested. Streams return zero (0) only at the end of the stream, otherwise, they should block until at least one byte is available.
		/// </returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="asyncResult"/> is null.
		///   </exception>
		///   
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="asyncResult"/> did not originate from a <see cref="M:System.IO.Stream.BeginRead(System.Byte[],System.Int32,System.Int32,System.AsyncCallback,System.Object)"/> method on the current stream.
		///   </exception>
		///   
		/// <exception cref="T:System.IO.IOException">
		/// The stream is closed or an internal error has occurred.
		///   </exception>
		public override int EndRead(IAsyncResult asyncResult)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Ends an asynchronous write operation.
		/// </summary>
		/// <param name="asyncResult">A reference to the outstanding asynchronous I/O request.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="asyncResult"/> is null.
		///   </exception>
		///   
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="asyncResult"/> did not originate from a <see cref="M:System.IO.Stream.BeginWrite(System.Byte[],System.Int32,System.Int32,System.AsyncCallback,System.Object)"/> method on the current stream.
		///   </exception>
		///   
		/// <exception cref="T:System.IO.IOException">
		/// The stream is closed or an internal error has occurred.
		///   </exception>
		public override void EndWrite(IAsyncResult asyncResult)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Reads a block of bytes from the current stream and writes the data to <paramref name="buffer"/>.
		/// </summary>
		/// <param name="buffer">When this method returns, contains the specified byte array with the values between <paramref name="offset"/> and (<paramref name="offset"/> + <paramref name="count"/> - 1) replaced by the characters read from the current stream.</param>
		/// <param name="offset">The byte offset in <paramref name="buffer"/> at which to begin reading.</param>
		/// <param name="count">The maximum number of bytes to read.</param>
		/// <returns>
		/// The total number of bytes written into the buffer. This can be less than the number of bytes requested if that number of bytes are not currently available, or zero if the end of the stream is reached before any bytes are read.
		/// </returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer"/> is null.
		///   </exception>
		///   
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset"/> or <paramref name="count"/> is negative.
		///   </exception>
		///   
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="offset"/> subtracted from the buffer length is less than <paramref name="count"/>.
		///   </exception>
		///   
		/// <exception cref="T:System.ObjectDisposedException">
		/// The current stream instance is closed.
		///   </exception>
		public override int Read(byte[] buffer, int offset, int count)
		{
		    return this.ChainedOutputFilter.Read(buffer, offset, count);
		}

		/// <summary>
		/// Reads a byte from the current stream.
		/// </summary>
		/// <returns>
		/// The byte cast to a <see cref="T:System.Int32"/>, or -1 if the end of the stream has been reached.
		/// </returns>
		/// <exception cref="T:System.ObjectDisposedException">
		/// The current stream instance is closed.
		///   </exception>
		public override int ReadByte()
		{
		    return this.ChainedOutputFilter.ReadByte();
		}

		/// <summary>
		/// Sets the position within the current stream to the specified value.
		/// </summary>
		/// <param name="offset">The new position within the stream. This is relative to the <paramref name="origin"/> parameter, and can be positive or negative.</param>
        /// <param name="origin">A value of type <see cref="T:System.IO.SeekOrigin"/>, which acts as the seek reference point.</param>
		/// <returns>
		/// The new position within the stream, calculated by combining the initial reference point and the offset.
		/// </returns>
		/// <exception cref="T:System.IO.IOException">
		/// Seeking is attempted before the beginning of the stream.
		///   </exception>
		///   
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset"/> is greater than <see cref="F:System.Int32.MaxValue"/>.
		///   </exception>
		///   
		/// <exception cref="T:System.ArgumentException">
		/// There is an invalid <see cref="T:System.IO.SeekOrigin"/>.
		/// -or-
		///   <paramref name="offset"/> caused an arithmetic overflow.
		///   </exception>
		///   
		/// <exception cref="T:System.ObjectDisposedException">
		/// The current stream instance is closed.
		///   </exception>
		public override long Seek(long offset, SeekOrigin origin)
		{
            return this.ChainedOutputFilter.Seek(offset, origin);
		}

		/// <summary>
		/// Sets the length of the current stream to the specified value.
		/// </summary>
		/// <param name="value">The value at which to set the length.</param>
		/// <exception cref="T:System.NotSupportedException">
		/// The current stream is not resizable and <paramref name="value"/> is larger than the current capacity.
		/// -or-
		/// The current stream does not support writing.
		///   </exception>
		///   
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="value"/> is negative or is greater than the maximum length of the <see cref="T:System.IO.MemoryStream"/>, where the maximum length is(<see cref="F:System.Int32.MaxValue"/> - origin), and origin is the index into the underlying buffer at which the stream starts.
		///   </exception>
		public override void SetLength(long value)
		{
            this.ChainedOutputFilter.SetLength(value);
		}

		/// <summary>
		/// Writes a block of bytes to the current stream using data read from buffer.
		/// </summary>
		/// <param name="buffer">The buffer to write data from.</param>
		/// <param name="offset">The byte offset in <paramref name="buffer"/> at which to begin writing from.</param>
		/// <param name="count">The maximum number of bytes to write.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="buffer"/> is null.
		///   </exception>
		///   
		/// <exception cref="T:System.NotSupportedException">
		/// The stream does not support writing. For additional information see <see cref="P:System.IO.Stream.CanWrite"/>.
		/// -or-
		/// The current position is closer than <paramref name="count"/> bytes to the end of the stream, and the capacity cannot be modified.
		///   </exception>
		///   
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="offset"/> subtracted from the buffer length is less than <paramref name="count"/>.
		///   </exception>
		///   
		/// <exception cref="T:System.ArgumentOutOfRangeException">
		///   <paramref name="offset"/> or <paramref name="count"/> are negative.
		///   </exception>
		///   
		/// <exception cref="T:System.IO.IOException">
		/// An I/O error occurs.
		///   </exception>
		///   
		/// <exception cref="T:System.ObjectDisposedException">
		/// The current stream instance is closed.
		///   </exception>
		public override void Write(byte[] buffer, int offset, int count)
		{
			this.ChainedOutputFilter.Write(buffer, offset, count);
		}

		/// <summary>
		/// Writes a byte to the current stream at the current position.
		/// </summary>
		/// <param name="value">The byte to write.</param>
		/// <exception cref="T:System.NotSupportedException">
		/// The stream does not support writing. For additional information see <see cref="P:System.IO.Stream.CanWrite"/>.
		/// -or-
		/// The current position is at the end of the stream, and the capacity cannot be modified.
		///   </exception>
		///   
		/// <exception cref="T:System.ObjectDisposedException">
		/// The current stream is closed.
		///   </exception>
		public override void WriteByte(byte value)
		{
			this.ChainedOutputFilter.WriteByte(value);
		}

		/// <summary>
		/// Releases the unmanaged resources used by the <see cref="T:System.IO.MemoryStream"/> class and optionally releases the managed resources.
		/// </summary>
		/// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
		protected override void Dispose(bool disposing)
		{
		    try
		    {
		    }
		    finally
		    {
                base.Dispose(disposing);
                this.IsDisposed = true;
		    }
		}

        /// <summary>
        /// When overridden in a derived class, clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        public override void Flush()
        {
            this.ChainedOutputFilter.Flush();
        }

        /// <summary>
        /// When overridden in a derived class, gets the length in bytes of the stream.
        /// </summary>
        /// <value>The length of the stream in bytes.</value>
        /// <returns>A long value representing the length of the stream in bytes.</returns>
        public override long Length
        {
            get { return this.ChainedOutputFilter.Length; }
        }

        /// <summary>
        /// When overridden in a derived class, gets or sets the position within the current stream.
        /// </summary>
        /// <value>The position within the stream.</value>
        /// <returns>The current position within the stream.</returns>
        public override long Position
        {
            get { return this.ChainedOutputFilter.Position; }
            set { this.ChainedOutputFilter.Position = value; }
        }
		#endregion
	}
}