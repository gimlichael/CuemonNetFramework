using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace Cuemon.IO.Compression
{
	/// <summary>
	/// This utility class is designed to make <see cref="Stream"/> related compression operations easier to work with.
	/// </summary>
	public static class CompressionUtility
	{
		/// <summary>
		/// Compresses the <paramref name="source"/> stream using the Deflate algorithm.
		/// </summary>
		/// <param name="source">The source stream to compress.</param>
		/// <returns>A compressed <see cref="System.IO.Stream"/> of the <paramref name="source"/>.</returns>
		public static Stream CompressStream(Stream source)
		{
			return CompressStream(source, CompressionType.Deflate);
		}

		/// <summary>
		/// Compresses the <paramref name="source"/> stream using the specified <paramref name="compressionType"/> algorithm.
		/// </summary>
		/// <param name="source">The source stream to compress.</param>
		/// <param name="compressionType">The compression algorithm to use for the compression.</param>
		/// <returns>A compressed <see cref="System.IO.Stream"/> of the <paramref name="source"/>.</returns>
		public static Stream CompressStream(Stream source, CompressionType compressionType)
		{
			if (source == null) { throw new ArgumentNullException("source"); }
			MemoryStream target = new MemoryStream();
			try
			{
				Stream compressed = null;
				switch (compressionType)
				{
					case CompressionType.Deflate:
						compressed = new DeflateStream(target, CompressionMode.Compress, true);
						break;
					case CompressionType.GZip:
						compressed = new GZipStream(target, CompressionMode.Compress, true);
						break;
					default:
						throw new ArgumentOutOfRangeException("compressionType", "Specified argument was out of the range of valid values.");
				}

				using (compressed)
				{
					List<byte> sourceContent = new List<byte>();
					source.Position = 0;
					while (source.Position != source.Length) { sourceContent.Add((byte)source.ReadByte()); }
					compressed.Write(sourceContent.ToArray(), 0, sourceContent.Count);
					compressed.Flush();
				}
			}
			catch (Exception)
			{
				target.Dispose();
				throw;
			}
		    target.Position = 0;
			return target;
		}

		/// <summary>
		/// Decompresses the source stream using the Deflate algorithm.
		/// </summary>
		/// <param name="source">The source stream to decompress.</param>
		/// <returns>A decompressed <see cref="System.IO.Stream"/> of the <paramref name="source"/>.</returns>
		public static Stream DecompressStream(Stream source)
		{
			return DecompressStream(source, CompressionType.Deflate);
		}

		/// <summary>
		/// Decompresses the source stream using the specified <paramref name="compressionType"/> algorithm.
		/// </summary>
		/// <param name="source">The source stream to decompress.</param>
		/// <param name="compressionType">The compression algorithm to use for the decompression.</param>
		/// <returns>A decompressed <see cref="System.IO.Stream"/> of the <paramref name="source"/>.</returns>
		public static Stream DecompressStream(Stream source, CompressionType compressionType)
		{
			if (source == null) { throw new ArgumentNullException("source"); }
			MemoryStream target = new MemoryStream();
			try
			{
				Stream uncompressed = null;
				switch (compressionType)
				{
					case CompressionType.Deflate:
						uncompressed = new DeflateStream(source, CompressionMode.Decompress, true);
						break;
					case CompressionType.GZip:
						uncompressed = new GZipStream(source, CompressionMode.Decompress, true);
						break;
					default:
						throw new ArgumentOutOfRangeException("compressionType", "Specified argument was out of the range of valid values.");
				}

				source.Position = 0;
				using (uncompressed)
				{
					byte[] buffer = new byte[512];
					int compressedByte;
					while ((compressedByte = uncompressed.Read(buffer, 0, buffer.Length)) > 0)
					{
						target.Write(buffer, 0, compressedByte);
					}
					uncompressed.Flush();
				}
			}
			catch (Exception)
			{
				target.Dispose();
				throw;
			}
            target.Position = 0;
			return target;
		}
	}

	/// <summary>
	/// Specifies the algorithm used for compression.
	/// </summary>
	public enum CompressionType
	{
		/// <summary>
		/// A fast and efficient compression using a combination of the LZ77 algorithm and Huffman coding.
		/// </summary>
		Deflate,
		/// <summary>
		/// A slower but otherwise identical compression to the <see cref="CompressionType.Deflate"/> with cyclic redundancy check value for data corruption detection.
		/// </summary>
		GZip
	}
}