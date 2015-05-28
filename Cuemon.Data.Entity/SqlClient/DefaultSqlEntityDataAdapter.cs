using System;
using System.Globalization;
using Cuemon.Data.SqlClient;

namespace Cuemon.Data.Entity.SqlClient
{
    /// <summary>
    /// Provides a default Microsoft SQL Server implementation of the <see cref="SqlEntityDataAdapter"/> class.
    /// </summary>
    public class DefaultSqlEntityDataAdapter : SqlEntityDataAdapter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultSqlEntityDataAdapter"/> class.
        /// </summary>
        public DefaultSqlEntityDataAdapter()
            : this(DataManager.DefaultConnectionString)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultSqlEntityDataAdapter"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string used to establish the connection.</param>
        public DefaultSqlEntityDataAdapter(string connectionString)
            : this(null, ValidateConnectionString(connectionString))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultSqlEntityDataAdapter" /> class.
        /// </summary>
        /// <param name="entity">The calling business entity.</param>
        public DefaultSqlEntityDataAdapter(Entity entity)
            : this(entity, DataManager.DefaultConnectionString)
        {   
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultSqlEntityDataAdapter"/> class.
        /// </summary>
        /// <param name="entity">The calling business entity.</param>
        /// <param name="connectionString">The connection string used to establish the connection.</param>
        public DefaultSqlEntityDataAdapter(Entity entity, string connectionString)
            : base(entity, new SqlDataManager(ValidateConnectionString(connectionString)))
        {
        }

        private static string ValidateConnectionString(string connectionString)
        {
            string valueNotSetMessage = "No SQL connection string value specified.";
            Validator.ThrowIfNull(connectionString, "connectionString", valueNotSetMessage);
            Validator.ThrowIfEmpty(connectionString, "connectionString", valueNotSetMessage);
            return connectionString;
        }
    }
}
