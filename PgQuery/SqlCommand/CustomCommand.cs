using System;
using System.Collections.Generic;
using Npgsql;

namespace PgQuery
{
    /// <summary>
    /// Custom SQL Command (not expecting result records)
    /// </summary>
    public class CustomCommand : SqlBuilder
    {
        readonly string SqlCommand;

        /// <summary>
        /// Custom SQL Command Constructor
        /// </summary>
        /// <param name="sql">SQL Command</param>
        public CustomCommand(string sql)
        {
            this.SqlCommand = sql;
        }

        /// <summary>
        /// Add binding parameter
        /// </summary>
        /// <param name="paramName">Parameter name appeared in sql command (without @ sign)</param>
        /// <param name="value">Value of correspongind parameter name</param>
        /// <returns>self</returns>
        public CustomCommand AddParameter(string paramName, object value)
        {
            this.ParamBinder.SetCustom(paramName, value);
            return this;
        }
        
        /// <summary>
        /// Execute with expecting result (binding DataReader)
        /// </summary>
        /// <param name="connection">Connection</param>
        /// <returns>Success or not</returns>
        protected bool ExecuteForReader(NpgsqlConnection connection = null)
        {
            return this.ExecuteReader(this.PrepareCommand(connection));
        }
        
        /// <summary>
        /// Execute command
        /// </summary>
        /// <param name="connection">Connection</param>
        /// <returns>Success or not</returns>
        public override bool Execute(NpgsqlConnection connection = null)
        {
            return this.ExecuteCommand(this.PrepareCommand(connection));
        }
        
        /// <summary>
        /// Generate SQL Query
        /// </summary>
        /// <returns>SQL string</returns>
        public override string GenerateQuery()
        {
            return this.SqlCommand;
        }
    }
}
