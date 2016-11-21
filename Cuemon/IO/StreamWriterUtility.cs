using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Cuemon.IO
{
    /// <summary>
    /// This utility class is designed to make <see cref="StreamWriter"/> related operations easier to work with.
    /// </summary>
    public static class StreamWriterUtility
    {
        /// <summary>
        /// Creates and returns a <see cref="Stream"/> by the specified delegate <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">The delegate that will create an in-memory <see cref="Stream"/>.</param>
        /// <param name="setup">The <see cref="StreamWriterOptions"/> which need to be configured.</param>
        /// <returns>A <see cref="Stream"/> holding the content created by the delegate <paramref name="writer"/>.</returns>
        public static Stream CreateStream(Act<StreamWriter> writer, Act<StreamWriterOptions> setup = null)
        {
            var factory = ActFactory.Create(writer, null);
            return CreateStreamCore(factory, setup, MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Creates and returns a <see cref="Stream"/> by the specified delegate <paramref name="writer"/>.
        /// </summary>
        /// <typeparam name="T">The type of the parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <param name="writer">The delegate that will create an in-memory <see cref="Stream"/>.</param>
        /// <param name="arg">The parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="setup">The <see cref="StreamWriterOptions"/> which need to be configured.</param>
        /// <returns>A <see cref="Stream"/> holding the content created by the delegate <paramref name="writer"/>.</returns>
        public static Stream CreateStream<T>(Act<StreamWriter, T> writer, T arg, Act<StreamWriterOptions> setup = null)
        {
            var factory = ActFactory.Create(writer, null, arg);
            return CreateStreamCore(factory, setup, MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Creates and returns a <see cref="Stream"/> by the specified delegate <paramref name="writer"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <param name="writer">The delegate that will create an in-memory <see cref="Stream"/>.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="setup">The <see cref="StreamWriterOptions"/> which need to be configured.</param>
        /// <returns>A <see cref="Stream"/> holding the content created by the delegate <paramref name="writer"/>.</returns>
        public static Stream CreateStream<T1, T2>(Act<StreamWriter, T1, T2> writer, T1 arg1, T2 arg2, Act<StreamWriterOptions> setup = null)
        {
            var factory = ActFactory.Create(writer, null, arg1, arg2);
            return CreateStreamCore(factory, setup, MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Creates and returns a <see cref="Stream"/> by the specified delegate <paramref name="writer"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <param name="writer">The delegate that will create an in-memory <see cref="Stream"/>.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="setup">The <see cref="StreamWriterOptions"/> which need to be configured.</param>
        /// <returns>A <see cref="Stream"/> holding the content created by the delegate <paramref name="writer"/>.</returns>
        public static Stream CreateStream<T1, T2, T3>(Act<StreamWriter, T1, T2, T3> writer, T1 arg1, T2 arg2, T3 arg3, Act<StreamWriterOptions> setup = null)
        {
            var factory = ActFactory.Create(writer, null, arg1, arg2, arg3);
            return CreateStreamCore(factory, setup, MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Creates and returns a <see cref="Stream"/> by the specified delegate <paramref name="writer"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <param name="writer">The delegate that will create an in-memory <see cref="Stream"/>.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="setup">The <see cref="StreamWriterOptions"/> which need to be configured.</param>
        /// <returns>A <see cref="Stream"/> holding the content created by the delegate <paramref name="writer"/>.</returns>
        public static Stream CreateStream<T1, T2, T3, T4>(Act<StreamWriter, T1, T2, T3, T4> writer, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Act<StreamWriterOptions> setup = null)
        {
            var factory = ActFactory.Create(writer, null, arg1, arg2, arg3, arg4);
            return CreateStreamCore(factory, setup, MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Creates and returns a <see cref="Stream"/> by the specified delegate <paramref name="writer"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <param name="writer">The delegate that will create an in-memory <see cref="Stream"/>.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="setup">The <see cref="StreamWriterOptions"/> which need to be configured.</param>
        /// <returns>A <see cref="Stream"/> holding the content created by the delegate <paramref name="writer"/>.</returns>
        public static Stream CreateStream<T1, T2, T3, T4, T5>(Act<StreamWriter, T1, T2, T3, T4, T5> writer, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Act<StreamWriterOptions> setup = null)
        {
            var factory = ActFactory.Create(writer, null, arg1, arg2, arg3, arg4, arg5);
            return CreateStreamCore(factory, setup, MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Creates and returns a <see cref="Stream"/> by the specified delegate <paramref name="writer"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <param name="writer">The delegate that will create an in-memory <see cref="Stream"/>.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="setup">The <see cref="StreamWriterOptions"/> which need to be configured.</param>
        /// <returns>A <see cref="Stream"/> holding the content created by the delegate <paramref name="writer"/>.</returns>
        public static Stream CreateStream<T1, T2, T3, T4, T5, T6>(Act<StreamWriter, T1, T2, T3, T4, T5, T6> writer, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, Act<StreamWriterOptions> setup = null)
        {
            var factory = ActFactory.Create(writer, null, arg1, arg2, arg3, arg4, arg5, arg6);
            return CreateStreamCore(factory, setup, MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Creates and returns a <see cref="Stream"/> by the specified delegate <paramref name="writer"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <param name="writer">The delegate that will create an in-memory <see cref="Stream"/>.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="setup">The <see cref="StreamWriterOptions"/> which need to be configured.</param>
        /// <returns>A <see cref="Stream"/> holding the content created by the delegate <paramref name="writer"/>.</returns>
        public static Stream CreateStream<T1, T2, T3, T4, T5, T6, T7>(Act<StreamWriter, T1, T2, T3, T4, T5, T6, T7> writer, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, Act<StreamWriterOptions> setup = null)
        {
            var factory = ActFactory.Create(writer, null, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            return CreateStreamCore(factory, setup, MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Creates and returns a <see cref="Stream"/> by the specified delegate <paramref name="writer"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <param name="writer">The delegate that will create an in-memory <see cref="Stream"/>.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg8">The eighth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="setup">The <see cref="StreamWriterOptions"/> which need to be configured.</param>
        /// <returns>A <see cref="Stream"/> holding the content created by the delegate <paramref name="writer"/>.</returns>
        public static Stream CreateStream<T1, T2, T3, T4, T5, T6, T7, T8>(Act<StreamWriter, T1, T2, T3, T4, T5, T6, T7, T8> writer, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, Act<StreamWriterOptions> setup = null)
        {
            var factory = ActFactory.Create(writer, null, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            return CreateStreamCore(factory, setup, MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Creates and returns a <see cref="Stream"/> by the specified delegate <paramref name="writer"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <param name="writer">The delegate that will create an in-memory <see cref="Stream"/>.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg8">The eighth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg9">The ninth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="setup">The <see cref="StreamWriterOptions"/> which need to be configured.</param>
        /// <returns>A <see cref="Stream"/> holding the content created by the delegate <paramref name="writer"/>.</returns>
        public static Stream CreateStream<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Act<StreamWriter, T1, T2, T3, T4, T5, T6, T7, T8, T9> writer, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, Act<StreamWriterOptions> setup = null)
        {
            var factory = ActFactory.Create(writer, null, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
            return CreateStreamCore(factory, setup, MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Creates and returns a <see cref="Stream"/> by the specified delegate <paramref name="writer"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the delegate <paramref name="writer"/>.</typeparam>
        /// <param name="writer">The delegate that will create an in-memory <see cref="Stream"/>.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg8">The eighth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg9">The ninth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="arg10">The tenth parameter of the delegate <paramref name="writer"/>.</param>
        /// <param name="setup">The <see cref="StreamWriterOptions"/> which need to be configured.</param>
        /// <returns>A <see cref="Stream"/> holding the content created by the delegate <paramref name="writer"/>.</returns>
        public static Stream CreateStream<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Act<StreamWriter, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> writer, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, Act<StreamWriterOptions> setup = null)
        {
            var factory = ActFactory.Create(writer, null, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
            return CreateStreamCore(factory, setup, MethodBase.GetCurrentMethod());
        }

        private static Stream CreateStreamCore<TTuple>(ActFactory<TTuple> factory, Act<StreamWriterOptions> setup, MethodBase caller) where TTuple : Template<StreamWriter>
        {
            var options = DelegateUtility.ConfigureAction(setup);
            Stream output = null;
            MemoryStream tempOutput = null;
            try
            {
                tempOutput = new MemoryStream(options.BufferSize);
                StreamWriter writer = new InternalStreamWriter(tempOutput, options);
                {
                    factory.GenericArguments.Arg1 = writer;
                    factory.ExecuteMethod();
                    writer.Flush();
                }
                tempOutput.Flush();
                tempOutput.Position = 0;
                output = tempOutput;
                tempOutput = null;
            }
            catch (Exception ex)
            {
                List<object> parameters = new List<object>();
                parameters.AddRange(factory.GenericArguments.ToArray());
                parameters.Add(options);
                throw ExceptionUtility.Refine(new InvalidOperationException("There is an error in the Stream being written.", ex), caller, parameters.ToArray());
            }
            finally
            {
                if (tempOutput != null) { tempOutput.Dispose(); }
                tempOutput = null;
                factory = null;
            }
            return output;
        }
    }
}