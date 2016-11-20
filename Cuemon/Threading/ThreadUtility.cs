using System;
using System.Reflection;
using System.Threading;
using Cuemon.Reflection;

namespace Cuemon.Threading
{
    /// <summary>
    /// Provide ways to work more efficient with <see cref="Thread" /> related tasks.
    /// </summary>
    public static class ThreadUtility
    {
        /// <summary>
        /// Retrieves the number of cores which is used for some thread related operations.
        /// </summary>
        public static readonly int NumberOfCores = GetNumberOfCores();

        /// <summary>
        /// Retrieves the logical number of processors which is used for some thread related operations.
        /// </summary>
        public static readonly int NumberOfLogicalProcessors = GetNumberOfLogicalProcessors();

        /// <summary>
        /// The default number of concurrent worker threads which is used for some thread related operations.
        /// </summary>
        /// <remarks>The default number of concurrent worker threads is calculated by (NumberOfCores * 4) + NumberOfLogicalProcessors.</remarks>
        public static readonly int DefaultNumberOfConcurrentWorkerThreads = (NumberOfCores * 4) + NumberOfLogicalProcessors;

        private static int GetNumberOfLogicalProcessors()
        {
            int result = 0;
            object rawNumberOfLogicalProcessors = PlatformUtility.Processor["NumberOfLogicalProcessors"];
            if (rawNumberOfLogicalProcessors != null)
            {
                string[] numberOfLogicalProcessors = rawNumberOfLogicalProcessors.ToString().Split(',');
                foreach (string numberOfLogicalProcessor in numberOfLogicalProcessors)
                {
                    result += Convert.ToInt32(numberOfLogicalProcessor.Trim());
                }
            }
            return result == 0 ? 1 : result;
        }

        private static int GetNumberOfCores()
        {
            int result = 0;
            object rawNumberOfCores = PlatformUtility.Processor["NumberOfCores"];
            if (rawNumberOfCores != null)
            {
                string[] numberOfCores = rawNumberOfCores.ToString().Split(',');
                foreach (string numberOfCore in numberOfCores)
                {
                    result += Convert.ToInt32(numberOfCore.Trim());
                }
            }
            return result == 0 ? 1 : result;
        }

        /// <summary>
        /// Causes the calling thread to yield execution to another thread that is ready to run on the current processor. The operating system selects the thread to yield to.
        /// </summary>
        /// <returns><c>true</c> if the operating system switched execution to another thread; otherwise <c>false</c>.</returns>
        public static bool Yield()
        {
            return SafeNativeMethods.SwitchToThread();
        }

        /// <summary>
        /// Specifies a set of features to apply on the <see cref="Thread"/> object.
        /// </summary>
        /// <returns>A <see cref="ThreadSettings"/> instance that specifies a set of features to apply the <see cref="Thread"/> object.</returns>
        /// <remarks>
        /// The following table shows the initial property values for an instance of <see cref="ThreadSettings"/>.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Property</term>
        ///         <description>Initial Value</description>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="ThreadSettings.Background"/></term>
        ///         <description><c>true</c></description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="ThreadSettings.Name"/></term>
        ///         <description>ThreadUtility.StartNew(<c>{0}</c>), where {0} will represent the arguments passed to the method.</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="ThreadSettings.Priority"/></term>
        ///         <description><see cref="ThreadPriority.Normal"/></description>
        ///     </item>
        /// </list>
        /// </remarks>
        public static ThreadSettings CreateSettings()
        {
            return CreateSettings(ThreadPriority.Normal);
        }

        /// <summary>
        /// Specifies a set of features to apply on the <see cref="Thread"/> object.
        /// </summary>
        /// <param name="priority">One of the <see cref="ThreadPriority"/> values. The default value is <see cref="ThreadPriority.Normal"/>.</param>
        /// <returns>A <see cref="ThreadSettings"/> instance that specifies a set of features to apply on the <see cref="Thread"/> object.</returns>
        public static ThreadSettings CreateSettings(ThreadPriority priority)
        {
            return CreateSettings(priority, true);
        }

        /// <summary>
        /// Specifies a set of features to apply on the <see cref="Thread"/> object.
        /// </summary>
        /// <param name="priority">One of the <see cref="ThreadPriority"/> values. The default value is <see cref="ThreadPriority.Normal"/>.</param>
        /// <param name="background"><c>true</c> if a <see cref="Thread"/> is initialized as a background thread; otherwise, <c>false</c>.</param>
        /// <returns>A <see cref="ThreadSettings"/> instance that specifies a set of features to apply on the <see cref="Thread"/> object.</returns>
        public static ThreadSettings CreateSettings(ThreadPriority priority, bool background)
        {
            return CreateSettings(priority, background, null);
        }

        /// <summary>
        /// Specifies a set of features to apply on the <see cref="Thread"/> object.
        /// </summary>
        /// <param name="priority">One of the <see cref="ThreadPriority"/> values. The default value is <see cref="ThreadPriority.Normal"/>.</param>
        /// <param name="background"><c>true</c> if a <see cref="Thread"/> is initialized as a background thread; otherwise, <c>false</c>.</param>
        /// <param name="name">The name of a <see cref="Thread"/>.</param>
        /// <returns>A <see cref="ThreadSettings"/> instance that specifies a set of features to apply on the <see cref="Thread"/> object.</returns>
        public static ThreadSettings CreateSettings(ThreadPriority priority, bool background, string name)
        {
            ThreadSettings settings = new ThreadSettings();
            settings.Background = background;
            settings.Name = name;
            settings.Priority = priority;
            return settings;
        }

        /// <summary>
        /// Creates and starts a new instance of the <see cref="Thread"/> class.
        /// </summary>
        /// <param name="method">The delegate that is being invoked on the thread.</param>
        /// <returns>The started <see cref="Thread "/> that is executing the specified <paramref name="method"/> delegate.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static Thread StartNew(Act method)
        {
            return StartNew(CreateSettings(), method);
        }

        /// <summary>
        /// Creates and starts a new instance of the <see cref="Thread"/> class.
        /// </summary>
        /// <typeparam name="T">The type of the parameter of the delegate <paramref name="method" />.</typeparam>
        /// <param name="method">The delegate that is being invoked on the thread.</param>
        /// <param name="arg">The parameter of the delegate <paramref name="method" />.</param>
        /// <returns>The started <see cref="Thread "/> that is executing the specified <paramref name="method"/> delegate.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static Thread StartNew<T>(Act<T> method, T arg)
        {
            return StartNew(CreateSettings(), method, arg);
        }

        /// <summary>
        /// Creates and starts a new instance of the <see cref="Thread"/> class.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method" />.</typeparam>
        /// <param name="method">The delegate that is being invoked on the thread.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method" />.</param>
        /// <returns>The started <see cref="Thread "/> that is executing the specified <paramref name="method"/> delegate.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static Thread StartNew<T1, T2>(Act<T1, T2> method, T1 arg1, T2 arg2)
        {
            return StartNew(CreateSettings(), method, arg1, arg2);
        }

        /// <summary>
        /// Creates and starts a new instance of the <see cref="Thread"/> class.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="method" />.</typeparam>
        /// <param name="method">The delegate that is being invoked on the thread.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="method" />.</param>
        /// <returns>The started <see cref="Thread "/> that is executing the specified <paramref name="method"/> delegate.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static Thread StartNew<T1, T2, T3>(Act<T1, T2, T3> method, T1 arg1, T2 arg2, T3 arg3)
        {
            return StartNew(CreateSettings(), method, arg1, arg2, arg3);
        }

        /// <summary>
        /// Creates and starts a new instance of the <see cref="Thread"/> class.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <param name="method">The delegate that is being invoked on the thread.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="method" />.</param>
        /// <returns>The started <see cref="Thread "/> that is executing the specified <paramref name="method"/> delegate.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static Thread StartNew<T1, T2, T3, T4>(Act<T1, T2, T3, T4> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return StartNew(CreateSettings(), method, arg1, arg2, arg3, arg4);
        }

        /// <summary>
        /// Creates and starts a new instance of the <see cref="Thread"/> class.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <param name="method">The delegate that is being invoked on the thread.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="method" />.</param>
        /// <returns>The started <see cref="Thread "/> that is executing the specified <paramref name="method"/> delegate.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static Thread StartNew<T1, T2, T3, T4, T5>(Act<T1, T2, T3, T4, T5> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return StartNew(CreateSettings(), method, arg1, arg2, arg3, arg4, arg5);
        }

        /// <summary>
        /// Creates and starts a new instance of the <see cref="Thread"/> class.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <param name="method">The delegate that is being invoked on the thread.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="method" />.</param>
        /// <returns>The started <see cref="Thread "/> that is executing the specified <paramref name="method"/> delegate.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static Thread StartNew<T1, T2, T3, T4, T5, T6>(Act<T1, T2, T3, T4, T5, T6> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            return StartNew(CreateSettings(), method, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        /// <summary>
        /// Creates and starts a new instance of the <see cref="Thread"/> class.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="method" />.</typeparam>
        /// <param name="method">The delegate that is being invoked on the thread.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="method" />.</param>
        /// <returns>The started <see cref="Thread "/> that is executing the specified <paramref name="method"/> delegate.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static Thread StartNew<T1, T2, T3, T4, T5, T6, T7>(Act<T1, T2, T3, T4, T5, T6, T7> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            return StartNew(CreateSettings(), method, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        /// <summary>
        /// Creates and starts a new instance of the <see cref="Thread"/> class.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <param name="method">The delegate that is being invoked on the thread.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg8">The eighth parameter of the delegate <paramref name="method" />.</param>
        /// <returns>The started <see cref="Thread "/> that is executing the specified <paramref name="method"/> delegate.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static Thread StartNew<T1, T2, T3, T4, T5, T6, T7, T8>(Act<T1, T2, T3, T4, T5, T6, T7, T8> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            return StartNew(CreateSettings(), method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        /// <summary>
        /// Creates and starts a new instance of the <see cref="Thread"/> class.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <param name="method">The delegate that is being invoked on the thread.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg8">The eighth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg9">The ninth parameter of the delegate <paramref name="method" />.</param>
        /// <returns>The started <see cref="Thread "/> that is executing the specified <paramref name="method"/> delegate.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static Thread StartNew<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Act<T1, T2, T3, T4, T5, T6, T7, T8, T9> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            return StartNew(CreateSettings(), method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        /// <summary>
        /// Creates and starts a new instance of the <see cref="Thread"/> class.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <param name="method">The delegate that is being invoked on the thread.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg8">The eighth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg9">The ninth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg10">The tenth parameter of the delegate <paramref name="method" />.</param>
        /// <returns>The started <see cref="Thread "/> that is executing the specified <paramref name="method"/> delegate.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static Thread StartNew<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            return StartNew(CreateSettings(), method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        /// <summary>
        /// Creates and starts a new instance of the <see cref="Thread"/> class.
        /// </summary>
        /// <param name="settings">The <see cref="ThreadSettings"/> object used to configure the new <see cref="Thread"/> instance.</param>
        /// <param name="method">The delegate that is being invoked on the thread.</param>
        /// <returns>The started <see cref="Thread "/> that is executing the specified <paramref name="method"/> delegate.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="method"/> is null.
        /// </exception>
        public static Thread StartNew(ThreadSettings settings, Act method)
        {
            ValidateStartNew(settings, method);
            var factory = ActFactory.Create(method);
            return CreateThreadCore(factory, settings, MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Creates and starts a new instance of the <see cref="Thread"/> class.
        /// </summary>
        /// <typeparam name="T">The type of the parameter of the delegate <paramref name="method" />.</typeparam>
        /// <param name="settings">The <see cref="ThreadSettings"/> object used to configure the new <see cref="Thread"/> instance.</param>
        /// <param name="method">The delegate that is being invoked on the thread.</param>
        /// <param name="arg">The parameter of the delegate <paramref name="method" />.</param>
        /// <returns>The started <see cref="Thread "/> that is executing the specified <paramref name="method"/> delegate.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="method"/> is null.
        /// </exception>
        public static Thread StartNew<T>(ThreadSettings settings, Act<T> method, T arg)
        {
            ValidateStartNew(settings, method);
            var factory = ActFactory.Create(method, arg);
            return CreateThreadCore(factory, settings, MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Creates and starts a new instance of the <see cref="Thread"/> class.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method" />.</typeparam>
        /// <param name="settings">The <see cref="ThreadSettings"/> object used to configure the new <see cref="Thread"/> instance.</param>
        /// <param name="method">The delegate that is being invoked on the thread.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method" />.</param>
        /// <returns>The started <see cref="Thread "/> that is executing the specified <paramref name="method"/> delegate.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="method"/> is null.
        /// </exception>
        public static Thread StartNew<T1, T2>(ThreadSettings settings, Act<T1, T2> method, T1 arg1, T2 arg2)
        {
            ValidateStartNew(settings, method);
            var factory = ActFactory.Create(method, arg1, arg2);
            return CreateThreadCore(factory, settings, MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Creates and starts a new instance of the <see cref="Thread"/> class.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="method" />.</typeparam>
        /// <param name="settings">The <see cref="ThreadSettings"/> object used to configure the new <see cref="Thread"/> instance.</param>
        /// <param name="method">The delegate that is being invoked on the thread.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="method" />.</param>
        /// <returns>The started <see cref="Thread "/> that is executing the specified <paramref name="method"/> delegate.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="method"/> is null.
        /// </exception>
        public static Thread StartNew<T1, T2, T3>(ThreadSettings settings, Act<T1, T2, T3> method, T1 arg1, T2 arg2, T3 arg3)
        {
            ValidateStartNew(settings, method);
            var factory = ActFactory.Create(method, arg1, arg2, arg3);
            return CreateThreadCore(factory, settings, MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Creates and starts a new instance of the <see cref="Thread"/> class.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <param name="settings">The <see cref="ThreadSettings"/> object used to configure the new <see cref="Thread"/> instance.</param>
        /// <param name="method">The delegate that is being invoked on the thread.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="method" />.</param>
        /// <returns>The started <see cref="Thread "/> that is executing the specified <paramref name="method"/> delegate.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="method"/> is null.
        /// </exception>
        public static Thread StartNew<T1, T2, T3, T4>(ThreadSettings settings, Act<T1, T2, T3, T4> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ValidateStartNew(settings, method);
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4);
            return CreateThreadCore(factory, settings, MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Creates and starts a new instance of the <see cref="Thread"/> class.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <param name="settings">The <see cref="ThreadSettings"/> object used to configure the new <see cref="Thread"/> instance.</param>
        /// <param name="method">The delegate that is being invoked on the thread.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="method" />.</param>
        /// <returns>The started <see cref="Thread "/> that is executing the specified <paramref name="method"/> delegate.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="method"/> is null.
        /// </exception>
        public static Thread StartNew<T1, T2, T3, T4, T5>(ThreadSettings settings, Act<T1, T2, T3, T4, T5> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ValidateStartNew(settings, method);
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5);
            return CreateThreadCore(factory, settings, MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Creates and starts a new instance of the <see cref="Thread"/> class.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <param name="settings">The <see cref="ThreadSettings"/> object used to configure the new <see cref="Thread"/> instance.</param>
        /// <param name="method">The delegate that is being invoked on the thread.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="method" />.</param>
        /// <returns>The started <see cref="Thread "/> that is executing the specified <paramref name="method"/> delegate.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="method"/> is null.
        /// </exception>
        public static Thread StartNew<T1, T2, T3, T4, T5, T6>(ThreadSettings settings, Act<T1, T2, T3, T4, T5, T6> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ValidateStartNew(settings, method);
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6);
            return CreateThreadCore(factory, settings, MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Creates and starts a new instance of the <see cref="Thread"/> class.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="method" />.</typeparam>
        /// <param name="settings">The <see cref="ThreadSettings"/> object used to configure the new <see cref="Thread"/> instance.</param>
        /// <param name="method">The delegate that is being invoked on the thread.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="method" />.</param>
        /// <returns>The started <see cref="Thread "/> that is executing the specified <paramref name="method"/> delegate.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="method"/> is null.
        /// </exception>
        public static Thread StartNew<T1, T2, T3, T4, T5, T6, T7>(ThreadSettings settings, Act<T1, T2, T3, T4, T5, T6, T7> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ValidateStartNew(settings, method);
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            return CreateThreadCore(factory, settings, MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Creates and starts a new instance of the <see cref="Thread"/> class.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <param name="settings">The <see cref="ThreadSettings"/> object used to configure the new <see cref="Thread"/> instance.</param>
        /// <param name="method">The delegate that is being invoked on the thread.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg8">The eighth parameter of the delegate <paramref name="method" />.</param>
        /// <returns>The started <see cref="Thread "/> that is executing the specified <paramref name="method"/> delegate.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="method"/> is null.
        /// </exception>
        public static Thread StartNew<T1, T2, T3, T4, T5, T6, T7, T8>(ThreadSettings settings, Act<T1, T2, T3, T4, T5, T6, T7, T8> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ValidateStartNew(settings, method);
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            return CreateThreadCore(factory, settings, MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Creates and starts a new instance of the <see cref="Thread"/> class.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <param name="settings">The <see cref="ThreadSettings"/> object used to configure the new <see cref="Thread"/> instance.</param>
        /// <param name="method">The delegate that is being invoked on the thread.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg8">The eighth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg9">The ninth parameter of the delegate <paramref name="method" />.</param>
        /// <returns>The started <see cref="Thread "/> that is executing the specified <paramref name="method"/> delegate.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="method"/> is null.
        /// </exception>
        public static Thread StartNew<T1, T2, T3, T4, T5, T6, T7, T8, T9>(ThreadSettings settings, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ValidateStartNew(settings, method);
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
            return CreateThreadCore(factory, settings, MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Creates and starts a new instance of the <see cref="Thread"/> class.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <param name="settings">The <see cref="ThreadSettings"/> object used to configure the new <see cref="Thread"/> instance.</param>
        /// <param name="method">The delegate that is being invoked on the thread.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg8">The eighth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg9">The ninth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg10">The tenth parameter of the delegate <paramref name="method" />.</param>
        /// <returns>The started <see cref="Thread "/> that is executing the specified <paramref name="method"/> delegate.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="method"/> is null.
        /// </exception>
        public static Thread StartNew<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(ThreadSettings settings, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ValidateStartNew(settings, method);
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
            return CreateThreadCore(factory, settings, MethodBase.GetCurrentMethod());
        }

        private static void ValidateStartNew(ThreadSettings settings, object method)
        {
            Validator.ThrowIfNull(settings, "settings");
            Validator.ThrowIfNull(method, "method");
        }

        private static Thread CreateThreadCore<TTuple>(ActFactory<TTuple> factory, ThreadSettings settings, MethodBase method) where TTuple : Template
        {
            Thread thread = new Thread(o => ((ActFactory<TTuple>)o).ExecuteMethod());
            if (!string.IsNullOrEmpty(settings.Name))
            {
                thread.Name = settings.Name;
            }
            else
            {
                MethodDescriptor signature = MethodDescriptor.Create(method);
                thread.Name = signature.ToString();
            }
            thread.Priority = settings.Priority;
            thread.IsBackground = settings.Background;
            thread.Start(factory);
            return thread;
        }
    }
}