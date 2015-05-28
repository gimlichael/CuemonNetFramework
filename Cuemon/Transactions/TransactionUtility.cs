using System;
using System.Transactions;

namespace Cuemon.Transactions
{
    /// <summary>
    /// This utility class is designed to make transaction related operations easier to work with.
    /// </summary>
    public static class TransactionUtility
    {
        /// <summary>
        /// Creates and initializes a new instance of the <see cref="TransactionScope"/> class.
        /// </summary>
        /// <returns>A new instance of the <see cref="TransactionScope"/> class.</returns>
        /// <remarks>Default transaction level is <see cref="IsolationLevel.ReadCommitted"/> and default timeout is 1 minute.</remarks>
        public static TransactionScope CreateTransactionScope()
        {
            return CreateTransactionScope(IsolationLevel.ReadCommitted);
        }

        /// <summary>
        /// Creates and initializes a new instance of the <see cref="TransactionScope"/> class.
        /// </summary>
        /// <param name="level">One of the <see cref="IsolationLevel"/> values that specifies the isolation level of the transaction.</param>
        /// <returns>A new instance of the <see cref="TransactionScope"/> class with the specified <paramref name="level"/>.</returns>
        /// <remarks>Default transaction timeout is 1 minute.</remarks>
        public static TransactionScope CreateTransactionScope(IsolationLevel level)
        {
            return CreateTransactionScope(level, TimeSpan.FromMinutes(1));
        }

        /// <summary>
        /// Creates and initializes a new instance of the <see cref="TransactionScope"/> class.
        /// </summary>
        /// <param name="level">One of the <see cref="IsolationLevel"/> values that specifies the isolation level of the transaction.</param>
        /// <param name="timeout">A <see cref="TimeSpan"/> that specifies the timeout period for the transaction.</param>
        /// <returns>A new instance of the <see cref="TransactionScope"/> class with the specified <paramref name="timeout"/> value and <paramref name="level"/>.</returns>
        public static TransactionScope CreateTransactionScope(IsolationLevel level, TimeSpan timeout)
        {
            return CreateTransactionScope(level, timeout, TransactionScopeOption.Required);
        }

        /// <summary>
        /// Creates and initializes a new instance of the <see cref="TransactionScope"/> class.
        /// </summary>
        /// <param name="level">One of the <see cref="IsolationLevel"/> values that specifies the isolation level of the transaction.</param>
        /// <param name="timeout">A <see cref="TimeSpan"/> that specifies the timeout period for the transaction.</param>
        /// <param name="option">One of the <see cref="TransactionScopeOption"/> values that specifies the additional options for creating the transaction scope.</param>
        /// <returns>A new instance of the <see cref="TransactionScope"/> class with the specified <paramref name="timeout"/> value, <paramref name="level"/> and <paramref name="option"/>.</returns>
        public static TransactionScope CreateTransactionScope(IsolationLevel level, TimeSpan timeout, TransactionScopeOption option)
        {
            TransactionOptions transactionOptions = new TransactionOptions { IsolationLevel = level, Timeout = timeout };
            return new TransactionScope(option, transactionOptions);
        }
    }
}
