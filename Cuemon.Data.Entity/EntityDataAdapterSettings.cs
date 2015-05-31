namespace Cuemon.Data.Entity
{
    /// <summary>
    /// Specifies a set of features to support on the <see cref="EntityDataAdapter"/> object. This class cannot be inherited.
    /// </summary>
    public sealed class EntityDataAdapterSettings
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityDataAdapterSettings"/> class.
        /// </summary>
        public EntityDataAdapterSettings()
        {
            this.EnableBulkLoad = false;
            this.EnableReadLimit = false;
            this.EnableThrowOnNoRowsReturned = false;
            this.EnableConcurrencyCheck = false;
            this.EnableDerivedEntityLookup = true;
            this.EnableThrowOnNoRowsReturned = false;
            this.ReadLimit = 1000;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether a qualified <see cref="Entity"/> instance should perform a bulk load of the entities from the repository. Default is <c>false</c>.
        /// </summary>
        /// <value>
        ///   <c>true</c> if a qualified <see cref="Entity"/> instance should perform a bulk load of the entities from the repository; otherwise, <c>false</c>.
        /// </value>
        public bool EnableBulkLoad { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a <see cref="DataAdapterException"/> is thrown when no rows is returned from the repository. Default is <c>false</c>.
        /// </summary>
        /// <value><c>true</c> if a <see cref="DataAdapterException"/> is thrown when no rows is returned from the repository; otherwise, <c>false</c>.</value>
        public bool EnableThrowOnNoRowsReturned { get; set; }

        /// <summary>
        /// Gets a value limiting the maximum amount of records that can be retrieved from a repository.
        /// </summary>
        /// <value>
        /// The maximum amount of records that can be retrieved from a repository.
        /// </value>
        public int ReadLimit { get; set; }

        /// <summary>
        /// Gets a value indicating whether a query is restricted in how many records (<see cref="ReadLimit"/>) can be retrieved from a repository. Default is <c>false</c>.
        /// </summary>
        /// <value>
        ///   <c>true</c> if a query is restricted in how many records (<see cref="ReadLimit"/>) can be retrieved from a repository; otherwise, <c>false</c>.
        /// </value>
        public bool EnableReadLimit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the data mapping classes should do a concurrency check on the data fetched in accordance with the objects the data is exposed to. Default is <c>false</c>.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if data mapping classes should do a concurrency check on the data fetched in accordance with the objects the data is exposed to; otherwise, <c>false</c>.
        /// </value>
        public bool EnableConcurrencyCheck { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the data mapping logic should try to discover any deriving classes from the associated <see cref="Entity"/> class. Default is <c>true</c>.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the data mapping logic should try to discover any deriving classes from the associated <see cref="Entity"/> class; otherwise, <c>false</c>.
        /// </value>
        public bool EnableDerivedEntityLookup { get; set; }
        #endregion
    }
}
