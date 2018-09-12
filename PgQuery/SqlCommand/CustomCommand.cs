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
        readonly IDictionary<string, object> Parameters;

        /// <summary>
        /// Custom SQL Command Constructor
        /// </summary>
        /// <param name="sql">SQL Command</param>
        public CustomCommand(string sql)
        {
            this.SqlCommand = sql;
            this.Parameters = new Dictionary<string, object>();
        }

        /// <summary>
        /// Add binding parameter
        /// </summary>
        /// <param name="paramName">Parameter name appeared in sql command (without @ sign)</param>
        /// <param name="value">Value of correspongind parameter name</param>
        /// <returns>self</returns>
        public CustomCommand AddParameter(string paramName, object value)
        {
            this.Parameters[paramName] = value;
            return this;
        }

        /// <summary>
        /// Command prepare statement
        /// </summary>
        /// <param name="connection">Postgres Connection</param>
        /// <returns>Parameter-binded NpgsqlCommand object</returns>
        private NpgsqlCommand Prepare(NpgsqlConnection connection = null)
        {
            NpgsqlCommand command = this.PrepareCommand(connection);
            foreach (KeyValuePair<string, object> parameter in this.Parameters)
            {
                command.Parameters.AddWithValue(parameter.Key, parameter.Value);
            }
            return command;
        }
        
        protected bool ExecuteForReader(NpgsqlConnection connection = null)
        {
            NpgsqlCommand command = this.Prepare();
            return this.ExecuteReader(command);
        }
        
        public override bool Execute(NpgsqlConnection connection = null)
        {
            this.AffectedRows = this.Prepare(connection).ExecuteNonQuery();
            if (this.AffectedRows < 0)
            {
                return false;
            }
            this.Executed = true;
            return true;
        }
        
        public override string GenerateQuery()
        {
            return this.SqlCommand;
        }
    }
}
