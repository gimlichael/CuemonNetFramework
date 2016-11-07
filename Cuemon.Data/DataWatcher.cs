using System;
using System.Collections.Generic;
using System.Data;
using Cuemon.Runtime;
using Cuemon.Security.Cryptography;

namespace Cuemon.Data
{
    /// <summary>
    /// A <see cref="Watcher"/> implementation, that can monitor and signal changes of one or more data locations by raising the <see cref="Watcher.Changed"/> event.
    /// </summary>
    public sealed class DataWatcher : Watcher
    {
        private readonly object _locker = new object();

        #region Constructors
        DataWatcher()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataWatcher"/> class.
        /// </summary>
        /// <param name="manager">The <see cref="DataManager"/> to be used for the underlying data operations.</param>
        /// <param name="command">The <see cref="IDataCommand"/> to execute and monitor for changes.</param>
        /// <param name="parameters">An optional array of <see cref="IDataParameter"/> to use with the associated <paramref name="command"/>.</param>
        /// <remarks>Monitors the provided <paramref name="command"/> for changes, using a MD5 hash check on the query result.</remarks>
        public DataWatcher(DataManager manager, IDataCommand command, params IDataParameter[] parameters) : this(manager, command, null, parameters)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataWatcher"/> class.
        /// </summary>
        /// <param name="manager">The <see cref="DataManager"/> to be used for the underlying data operations.</param>
        /// <param name="command">The <see cref="IDataCommand"/> to execute and monitor for changes.</param>
        /// <param name="setup">The <see cref="WatcherOptions"/> which need to be configured.</param>
        /// <param name="parameters">An optional array of <see cref="IDataParameter"/> to use with the associated <paramref name="command"/>.</param>
        /// <remarks>Monitors the provided <paramref name="command"/> for changes as defined by the <paramref name="setup"/> delegate, using a MD5 hash check on the query result.</remarks>
        public DataWatcher(DataManager manager, IDataCommand command, Act<WatcherOptions> setup, params IDataParameter[] parameters) : base(setup)
        {
            if (command == null) { throw new ArgumentNullException(nameof(command)); }
            Manager = manager;
            Command = command;
            Parameters = parameters;
            Checksum = DefaultChecksum;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the associated <see cref="IDataCommand"/> of this <see cref="DataWatcher"/>.
        /// </summary>
        /// <value>The associated <see cref="IDataCommand"/> of this <see cref="DataWatcher"/>.</value>
        public IDataCommand Command { get; private set; }

        /// <summary>
        /// Gets the associated array of <see cref="IDataParameter"/> of this <see cref="DataWatcher"/>.
        /// </summary>
        /// <value>The associated array of <see cref="IDataParameter"/> of this <see cref="DataWatcher"/>.</value>
        public IDataParameter[] Parameters { get; private set; }

        /// <summary>
        /// Gets the associated <see cref="DataManager"/> of this <see cref="DataWatcher"/>.
        /// </summary>
        /// <value>The associated <see cref="DataManager"/> of this <see cref="DataWatcher"/>.</value>
        public DataManager Manager { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Handles the signaling of this <see cref="DataWatcher"/>.
        /// </summary>
        protected override void HandleSignaling()
        {
            lock (_locker)
            {
                DateTime utcLastModified = DateTime.UtcNow;
                List<object[]> values = new List<object[]>();
                DataManager manager = Manager.Clone();
                using (IDataReader reader = manager.ExecuteReader(Command, Parameters))
                {
                    while (reader.Read())
                    {
                        object[] readerValues = new object[reader.FieldCount];
                        reader.GetValues(readerValues);
                        values.Add(readerValues);
                    }
                }
                string currentChecksum = HashUtility.ComputeHash(values).ToHexadecimal();
                values.Clear();

                if (Checksum == DefaultChecksum) { Checksum = currentChecksum; }
                if (!Checksum.Equals(currentChecksum, StringComparison.OrdinalIgnoreCase))
                {
                    SetUtcLastModified(utcLastModified);
                    OnChangedRaised();
                }
                Checksum = currentChecksum;
            }
        }
        #endregion
    }
}