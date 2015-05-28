using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Cuemon.Collections.Generic;
using Cuemon.Diagnostics;
namespace Cuemon.Runtime.Serialization
{
    /// <summary>
    /// This utility class is designed to make serialization operations easier to work with.
    /// </summary>
    public static class SerializationUtility
    {
        /// <summary>
        /// Creates and returns a clone of the specified <paramref name="value"/> object using <see cref="BinaryFormatter"/> for the serialization.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <returns>A clone of the <paramref name="value"/> object using <see cref="BinaryFormatter"/> for the cloning process.</returns>
        public static T SerializeAsClone<T>(T value)
        {
            return SerializeAsClone(new BinaryFormatter(), value);
        }

        /// <summary>
        /// Creates and returns a clone of the specified <paramref name="value"/> object using the provided <see cref="IFormatter"/> for the serialization.
        /// </summary>
        /// <param name="formatter">The formatter to use for the serialization.</param>
        /// <param name="value">The object to serialize.</param>
        /// <returns>A clone of the <paramref name="value"/> object using the provided <see cref="IFormatter"/> for the cloning process.</returns>
        public static T SerializeAsClone<T>(IFormatter formatter, T value)
        {
            if (formatter == null) { throw new ArgumentNullException("formatter"); }
            using (Stream serialized = SerializeAsStream(formatter, value))
            {
                return (T)formatter.Deserialize(serialized);
            }
        }

        /// <summary>
        /// Creates and returns a stream representation of the given object using <see cref="BinaryFormatter"/> for the serialization.
        /// </summary>
        /// <param name="value">The object to serialize.</param>
        /// <returns>The <see cref="Stream"/> where the <see cref="BinaryFormatter"/> puts the serialized data.</returns>
        public static Stream SerializeAsStream(object value)
        {
            return SerializeAsStream(new BinaryFormatter(), value);
        }

        /// <summary>
        /// Creates and returns a stream representation of the given object using the provided <see cref="IFormatter"/> for the serialization.
        /// </summary>
        /// <param name="formatter">The formatter to use for the serialization.</param>
        /// <param name="value">The object to serialize.</param>
        /// <returns>The <see cref="Stream"/> where the formatter puts the serialized data.</returns>
        public static Stream SerializeAsStream(IFormatter formatter, object value)
        {
            if (formatter == null) { throw new ArgumentNullException("formatter"); }
            MemoryStream tempStream = null;
            MemoryStream stream = null;
            try
            {
                tempStream = new MemoryStream();
                formatter.SurrogateSelector = new SimpleSurrogateSelector();
                formatter.Serialize(tempStream, value);
                tempStream.Position = 0;
                stream = tempStream;
                tempStream = null;
            }
            finally
            {
                if (tempStream != null) { tempStream.Dispose(); }
            }
            return stream;
        }

        /// <summary>
        /// Deserializes the data on the provided stream and reconstitutes the graph of objects using <see cref="BinaryFormatter"/> for the process.
        /// </summary>
        /// <typeparam name="T">The type of the object that this method returns.</typeparam>
        /// <param name="value">The stream that contains the data to deserialize.</param>
        /// <returns>The top object of the deserialized graph.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="value"/> is null.
        /// </exception>
        public static T Deserialize<T>(Stream value)
        {
            if (value == null) { throw new ArgumentNullException("value"); }
            return SerializationUtility.Deserialize<T>(value, false);
        }

        /// <summary>
        /// Deserializes the data on the provided stream and reconstitutes the graph of objects using <see cref="BinaryFormatter"/> for the process.
        /// </summary>
        /// <typeparam name="T">The type of the object that this method returns.</typeparam>
        /// <param name="value">The stream that contains the data to deserialize.</param>
        /// <param name="leaveStreamOpen">if <c>true</c>, the <paramref name="value"/> <see cref="Stream"/> is being left open; otherwise <paramref name="value"/> is disposed of.</param>
        /// <returns>The top object of the deserialized graph.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="value"/> is null.
        /// </exception>
        public static T Deserialize<T>(Stream value, bool leaveStreamOpen)
        {
            if (value == null) { throw new ArgumentNullException("value"); }
            return SerializationUtility.Deserialize<T>(new BinaryFormatter(), value, leaveStreamOpen);
        }

        /// <summary>
        /// Deserializes the data on the provided stream and reconstitutes the graph of objects.
        /// </summary>
        /// <typeparam name="T">The type of the object that this method returns.</typeparam>
        /// <param name="formatter">The formatter to use for the deserialization.</param>
        /// <param name="value">The stream that contains the data to deserialize.</param>
        /// <returns>The top object of the deserialized graph.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="value"/> is null.
        /// </exception>
        public static T Deserialize<T>(IFormatter formatter, Stream value)
        {
            if (formatter == null) { throw new ArgumentNullException("formatter"); }
            if (value == null) { throw new ArgumentNullException("value"); }
            return SerializationUtility.Deserialize<T>(formatter, value, false);
        }

        /// <summary>
        /// Deserializes the data on the provided stream and reconstitutes the graph of objects.
        /// </summary>
        /// <typeparam name="T">The type of the object that this method returns.</typeparam>
        /// <param name="formatter">The formatter to use for the deserialization.</param>
        /// <param name="value">The stream that contains the data to deserialize.</param>
        /// <param name="leaveStreamOpen">if <c>true</c>, the <paramref name="value"/> <see cref="Stream"/> is being left open; otherwise <paramref name="value"/> is disposed of.</param>
        /// <returns>The top object of the deserialized graph.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="value"/> is null.
        /// </exception>
        public static T Deserialize<T>(IFormatter formatter, Stream value, bool leaveStreamOpen)
        {
            if (formatter == null) { throw new ArgumentNullException("formatter"); }
            if (value == null) { throw new ArgumentNullException("value"); }
            try
            {
                return (T)formatter.Deserialize(value);
            }
            finally
            {
                if (!leaveStreamOpen) { value.Dispose(); }
            }
        }

        /// <summary>
        /// Determines whether the specified type is valid for serializing.
        /// </summary>
        /// <param name="value">The type to evaluate.</param>
        /// <returns>
        /// 	<c>true</c> if the specified type is valid for serializing; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsTypeValidForSerializing(Type value)
        {
            if (value == null) { throw new ArgumentNullException("value"); }
            return ((value.IsPrimitive || value.IsClass || value.IsInterface || value.IsValueType) && (!value.IsSubclassOf(typeof(Delegate)) && !value.IsMarshalByRef));
        }
    }
}