using System.Data;
using Cuemon.Collections.Generic;
using Cuemon.Runtime;

namespace Cuemon.Data
{
    /// <summary>
    /// This <see cref="DataDependency"/> class will monitor any changes occurred to an underlying data source while notifying subscribing objects.
    /// </summary>
    public sealed class DataDependency : WatcherDependency
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DataDependency"/> class.
        /// </summary>
        /// <param name="manager">The <see cref="DataManager"/> to be used for the underlying data operations.</param>
        /// <param name="command">The <see cref="IDataCommand"/> to execute and monitor for changes.</param>
        /// <param name="parameters">An optional sequence of <see cref="IDataParameter"/> to use with the associated <paramref name="command"/>.</param>
        /// <remarks>The signaling is default delayed 15 seconds before first invoke. Signaling occurs every 2 minutes.</remarks>
        public DataDependency(DataManager manager, IDataCommand command, params IDataParameter[] parameters) : this(manager, command, null, parameters)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataDependency"/> class.
        /// </summary>
        /// <param name="manager">The <see cref="DataManager"/> to be used for the underlying data operations.</param>
        /// <param name="command">The <see cref="IDataCommand"/> to execute and monitor for changes.</param>
        /// <param name="setup">The <see cref="WatcherOptions"/> which need to be configured.</param>
        /// <param name="parameters">An optional sequence of <see cref="IDataParameter"/> to use with the associated <paramref name="command"/>.</param>
        public DataDependency(DataManager manager, IDataCommand command, Act<WatcherOptions> setup, params IDataParameter[] parameters)
            : base(() =>
            {
                Validator.ThrowIfNull(manager, nameof(manager));
                Validator.ThrowIfNull(command, nameof(command));
                return EnumerableUtility.Yield(DelegateUtility.SafeInvokeDisposable(() => new DataWatcher(manager, command, setup, parameters)) as Watcher);
            })
        {
        }
        #endregion

        #region Properties
        #endregion

        #region Methods
        #endregion
    }
}