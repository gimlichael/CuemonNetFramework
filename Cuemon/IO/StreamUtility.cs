using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using Cuemon.Diagnostics;
using Cuemon.Text;

namespace Cuemon.IO
{
    /// <summary>
    /// This utility class is designed to make <see cref="Stream"/> operations easier to work with.
    /// </summary>
    public static class StreamUtility
    {
        /// <summary>
        /// Removes the preamble information (if present) from the specified <see cref="Stream"/>. The encoding is tried resolved automatically, and will revert to <see cref="Encoding.Default"/> if no Unicode encoding could be determined.
        /// </summary>
        /// <param name="source">The input <see cref="Stream"/> to process.</param>
        /// <returns>A <see cref="Stream"/> without preamble information.</returns>
        public static Stream RemovePreamble(Stream source)
        {
            if (source == null) throw new ArgumentNullException("source");
            Encoding encoding = null;
            if (!EncodingUtility.TryParse(source, out encoding)) { encoding = Encoding.Default; } // no unicode could be determine - revert to O/S-default
            return RemovePreamble(source, encoding);
        }

        /// <summary>
        /// Removes the preamble information (if present) from the specified <see cref="Stream"/>, and <paramref name="source"/> is being closed and disposed.
        /// </summary>
        /// <param name="source">The input <see cref="Stream"/> to process.</param>
        /// <param name="encoding">The encoding to use when determining the preamble to remove.</param>
        /// <returns>A <see cref="Stream"/> without preamble information.</returns>
        public static Stream RemovePreamble(Stream source, Encoding encoding)
        {
            if (source == null) { throw new ArgumentNullException("source"); }
            byte[] bytes = ConvertUtility.ToByteArray(source);
            bytes = ByteUtility.RemovePreamble(bytes, encoding);

            MemoryStream output = null;
            MemoryStream tempOutput = null;
            try
            {
                tempOutput = new MemoryStream(bytes);
                tempOutput.Position = 0;
                output = tempOutput;
                tempOutput = null;
            }
            finally
            {
                if (tempOutput != null) { tempOutput.Dispose(); }
                using (source) // because we return a stream, close the source
                {
                }
            }
            output.Position = 0;
            return output;
        }

		/// <summary>
		/// Reads all the bytes from the <paramref name="source"/> stream and writes them to the <paramref name="destination"/> stream.
		/// </summary>
		/// <param name="source">The stream to read the contents from.</param>
		/// <param name="destination">The stream that will contain the contents of the <paramref name="source"/> stream.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="destination"/> is null.</exception>
		/// <exception cref="T:System.NotSupportedException">The <paramref name="source"/> stream does not support reading.-or-<paramref name="destination"/> does not support writing.</exception>
		/// <exception cref="T:System.ObjectDisposedException">Either the <paramref name="source"/> stream or <paramref name="destination"/> were closed before the <see cref="CopyStream(System.IO.Stream,System.IO.Stream)"/> method was called.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurred.</exception>
		public static void CopyStream(Stream source, Stream destination)
		{
			StreamUtility.CopyStream(source, destination, 2048);
		}


		/// <summary>
		/// Reads all the bytes from the <paramref name="source"/> stream and writes them to the <paramref name="destination"/> stream, using the specified buffer size of <paramref name="bufferSize"/>.
		/// </summary>
		/// <param name="source">The stream to read the contents from.</param>
		/// <param name="destination">The stream that will contain the contents of the <paramref name="source"/> stream.</param>
		/// <param name="bufferSize">The size of the buffer. This value must be greater than zero. The default size is 2048.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="destination"/> is null.</exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="bufferSize"/> is negative or zero.</exception>
		/// <exception cref="T:System.NotSupportedException">The <paramref name="source"/> stream does not support reading.-or-<paramref name="destination"/> does not support writing.</exception>
		/// <exception cref="T:System.ObjectDisposedException">Either the <paramref name="source"/> stream or <paramref name="destination"/> were closed before the <see cref="CopyStream(System.IO.Stream,System.IO.Stream)"/> method was called.</exception>
		/// <exception cref="T:System.IO.IOException">An I/O error occurred.</exception>
		public static void CopyStream(Stream source, Stream destination, int bufferSize)
		{
			if (source == null) { throw new ArgumentNullException("source"); }
			if (destination == null) { throw new ArgumentNullException("destination"); }
			if (bufferSize <= 0) { throw new ArgumentOutOfRangeException("bufferSize", "The buffer size is negative or zero."); }
			if (!source.CanRead && !source.CanWrite) { throw new ObjectDisposedException("source", "Source stream is disposed."); }
			if (!destination.CanRead && !destination.CanWrite) { throw new ObjectDisposedException("destination", "Destination stream is disposed."); }
			if (!source.CanRead) { throw new NotSupportedException("Source stream cannot be read from."); }
			if (!destination.CanWrite) { throw new NotSupportedException("Destination stream cannot be written to."); }

			byte[] buffer = new byte[bufferSize];
			int read;
			while ((read = source.Read(buffer, 0, buffer.Length)) != 0) { destination.Write(buffer, 0, read); }
		}


        /// <summary>
        /// Creates and returns a seekable copy of the source <see cref="Stream"/>.
        /// </summary>
        /// <param name="source">The source <see cref="Stream"/> to create a copy from.</param>
        /// <returns>A seekable <see cref="Stream"/> that will contain the contents of the source stream.</returns>
        public static Stream CopyStream(Stream source)
        {
            return CopyStream(source, false);
        }

        /// <summary>
        /// Creates and returns a seekable copy of the source <see cref="Stream"/>.
        /// </summary>
        /// <param name="source">The source <see cref="Stream"/> to create a copy from.</param>
        /// <param name="leaveStreamOpen">if <c>true</c>, the source <see cref="Stream"/> is being left open; otherwise it is being closed and disposed.</param>
        /// <returns>A seekable <see cref="Stream"/> that will contain the contents of the source stream.</returns>
        public static Stream CopyStream(Stream source, bool leaveStreamOpen)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (!source.CanRead) { throw new NotSupportedException("Source stream cannot be read from."); }

            long lastPosition = 0;
            if (source.CanSeek)
            {
                lastPosition = source.Position;
                if (source.CanSeek) { source.Position = 0; }
            }

            List<byte> destination = new List<byte>();
            int readByte;
            while ((readByte = source.ReadByte()) >= 0)
            {
                destination.Add((byte)readByte);
            }

            if (source.CanSeek) { source.Position = lastPosition; }

            if (!leaveStreamOpen)
            {
                using (source)
                {
                }
            }

            MemoryStream output = null;
            MemoryStream tempOutput = null;
            try
            {
                tempOutput = new MemoryStream(destination.ToArray());
                tempOutput.Position = 0;
                output = tempOutput;
                tempOutput = null;
            }
            finally
            {
                if (tempOutput != null) { tempOutput.Dispose(); }
            }
            output.Position = 0;
            return output;
        }

        /// <summary>
        /// Combines a variable number of streams into one stream.
        /// </summary>
        /// <param name="streams">The streams to combine.</param>
        /// <returns>A variable number of <b>streams</b> combined into one <b>stream</b>.</returns>
        public static Stream CombineStreams(params Stream[] streams)
        {
            if (streams == null) throw new ArgumentNullException("streams");
            List<byte[]> bytes = new List<byte[]>();
            foreach (Stream stream in streams)
            {
                byte[] bytesFromStream = new byte[stream.Length];
                using (stream) // close and dispose the stream, as we are returning a new combined stream
                {
                    stream.Read(bytesFromStream, 0, (int)stream.Length);
                    bytes.Add(bytesFromStream);
                }
            }

            MemoryStream output = null;
            MemoryStream tempOutput = null;
            try
            {
                tempOutput = new MemoryStream(ByteUtility.CombineByteArrays(bytes.ToArray()));
                tempOutput.Position = 0;
                output = tempOutput;
                tempOutput = null;
            }
            finally 
            {
                if (tempOutput != null) { tempOutput.Dispose(); }
            }
            output.Position = 0;
            return output;
        }
    }
}