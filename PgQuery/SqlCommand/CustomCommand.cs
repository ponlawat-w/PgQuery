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
        
        protected bool ExecuteForReader(NpgsqlConnection connection = null)
        {
            return this.ExecuteReader(this.PrepareCommand(connection));
        }
        
        public override bool Execute(NpgsqlConnection connection = null)
        {
            return this.ExecuteCommand(this.PrepareCommand(connection));
        }
        
        public override string GenerateQuery()
        {
            return this.SqlCommand;
        }
    }
}
