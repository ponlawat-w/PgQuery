using System;
using System.Collections.Generic;
using System.Linq;
using Npgsql;

namespace PgQuery
{
    /// <summary>
    /// An exception when statement contains no values
    /// </summary>
    public class PgQueryInsertNoValuesException : Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public PgQueryInsertNoValuesException() : base("An exception thrown by trying to insert record with no values")
        {
        }
    }

    /// <summary>
    /// Record inserting command
    /// </summary>
    public class InsertCommand : SqlBuilder
    {
        /// <summary>
        /// Table Name
        /// </summary>
        public string Table;

        /// <summary>
        /// Returning Field, if set, the statement expects result after executed
        /// </summary>
        public string ReturningField;

        private IDictionary<string, int> Values;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tableName">Target table name</param>
        /// <param name="returning">Returning field (if any)</param>
        public InsertCommand(string tableName, string returning = null)
        {
            this.Table = tableName;
            this.ReturningField = returning;
            this.Values = new Dictionary<string, int>();
        }

        /// <summary>
        /// Set returning field, overwrite if already exists
        /// </summary>
        /// <param name="returningField">Returning field name</param>
        /// <returns>self</returns>
        public InsertCommand Returning(string returningField)
        {
            this.ReturningField = returningField;
            return this;
        }

        /// <summary>
        /// Insert to given field with given value
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <returns>self</returns>
        public InsertCommand With(string fieldName, object value)
        {
            if (value != null)
            {
                this.Values[fieldName] = this.ParamBinder.Add(value);
            }
            return this;
        }

        /// <summary>
        /// Execute command or data reader, depends on returning field
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public override bool Execute(NpgsqlConnection connection = null)
        {
            return this.ReturningField == null ?
                this.ExecuteCommand(this.PrepareCommand(connection))
                    :
                this.ExecuteReader(this.PrepareCommand(connection));
        }

        /// <summary>
        /// Generate SQL Query
        /// </summary>
        /// <returns>SQL string</returns>
        public override string GenerateQuery()
        {
            if (this.Values.Count == 0)
            {
                throw new PgQueryInsertNoValuesException();
            }

            IEnumerable<string> fields = this.Values.Select(value => value.Key);
            IEnumerable<string> paramNames = this.Values.Select(value => $"@{value.Value}");

            string fieldStatement = String.Join(", ", fields);
            string valueStatement = String.Join(", ", paramNames);
            string returningStatement = this.ReturningField == null ?
                "" : $" RETURNING {this.ReturningField}";

            return $"INSERT INTO {this.Table} ({fieldStatement}) VALUES ({valueStatement}){returningStatement}";
        }
    }
}
