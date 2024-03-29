﻿using System;

namespace Cuemon
{
    /// <summary>
    /// Specifies options that is related to <see cref="TransientOperation"/> handling.
    /// </summary>
    /// <seealso cref="TransientOperation"/>.
    public class TransientOperationOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransientOperationOptions"/> class.
        /// </summary>
        /// <remarks>
        /// The following table shows the initial property values for an instance of <see cref="TransientOperationOptions"/>.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Property</term>
        ///         <description>Initial Value</description>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="RetryAttempts"/></term>
        ///         <description><see cref="DefaultRetryAttempts"/></description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="EnableRecovery"/></term>
        ///         <description><see cref="RetryAttempts"/> > 0</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="RetryStrategy"/></term>
        ///         <description><c>retry</c> + 2^ to a maximum of 5; eg. 1, 2, 4, 8, 16 to a total of 31 seconds</description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="DetectionStrategy"/></term>
        ///         <description><c>exception => false</c></description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="MaximumAllowedLatency"/></term>
        ///         <description><c>5 minutes</c></description>
        ///     </item>
        /// </list>
        /// </remarks>
        public TransientOperationOptions()
        {
            RetryAttempts = DefaultRetryAttempts;
            EnableRecovery = RetryAttempts > 0;
            RetryStrategy = currentAttempt => TimeSpan.FromSeconds(Math.Pow(2, currentAttempt > 5 ? 5 : currentAttempt));
            DetectionStrategy = exception => false;
            MaximumAllowedLatency = TimeSpan.FromMinutes(5);
        }

        /// <summary>
        /// Gets or sets the amount of retry attempts for transient faults. Default value is <see cref="DefaultRetryAttempts"/>.
        /// </summary>
        /// <value>The retry attempts for transient faults.</value>
        public int RetryAttempts { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a transient operation should be attempted gracefully recovered in case of a transient fault or simply be invoked like a normal operation.
        /// </summary>
        /// <value><c>true</c> if a transient operation should be attempted gracefully recovered in case of a transient fault; otherwise, <c>false</c>.</value>
        /// <remarks>For testing or diagnostic purposes, it can sometimes come in handy to turn off transient fault recovery.</remarks>
        public bool EnableRecovery { get; set; }

        /// <summary>
        /// Gets or sets the callback function delegate that determines the amount of time to wait for a transient fault to recover gracefully before trying a new attempt.
        /// </summary>
        /// <returns>A <see cref="Doer{TResult}"/> that determines the amount of time to wait for a transient fault to recover gracefully.</returns>
        /// <remarks>Default implementation is <see cref="int"/> + 2^ to a maximum of 5; eg. 1, 2, 4, 8, 16 to a total of 31 seconds.</remarks>
        public Doer<int, TimeSpan> RetryStrategy { get; set; }

        /// <summary>
        /// Gets or sets the callback function delegate that determines if an <see cref="Exception"/> contains clues that would suggest a transient fault.
        /// </summary>
        /// <value>A <see cref="Doer{TResult}"/> that determines if an <see cref="Exception"/> contains clues that would suggest a transient fault.</value>
        /// <remarks>Default implementation is fixed to none-transient failure.</remarks>
        public Doer<Exception, bool> DetectionStrategy { get; set; }

        /// <summary>
        /// Gets or sets the maximum allowed latency before a <see cref="LatencyException"/> is raised.
        /// </summary>
        /// <value>A <see cref="TimeSpan"/> defining the maximum allowed latency.</value>
        public TimeSpan MaximumAllowedLatency { get; set; }

        /// <summary>
        /// Gets or sets the default amount of retry attempts for transient faults. Default is 5 attempts.
        /// </summary>
        /// <value>The default amount of retry attempts for transient faults.</value>
        public static byte DefaultRetryAttempts { get; set; } = 5;
    }
}