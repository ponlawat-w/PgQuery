using System;
using System.Collections.Generic;
using System.Linq;
using Npgsql;

namespace PgQuery
{
    /// <summary>
    /// An exception when update statement contains no values to be updated
    /// </summary>
    public class PgQueryUpdateSetsNothingException : Exception
    {
        public PgQueryUpdateSetsNothingException() : base("Update command does not set any value")
        {
        }
    }

    /// <summary>
    /// Data updating command
    /// </summary>
    public class UpdateCommand : SqlConditionBuilder
    {
        private string Table;
        private IDictionary<string, string> SetValues;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tableName">Target table name</param>
        public UpdateCommand(string tableName)
        {
            this.Table = tableName;
            this.SetValues = new Dictionary<string, string>();
        }

        /// <summary>
        /// Set field value with given statement rather than a value
        /// </summary>
        /// <example>
        /// SetWithStatement("salary", "salary * 1.5")
        ///     is equivalent to SQL Command "SET salary = salary * 1.5"
        /// </example>
        /// <param name="fieldName">Target field name</param>
        /// <param name="statement">Statement</param>
        /// <returns>self</returns>
        public UpdateCommand SetWithStatement(string fieldName, string statement)
        {
            this.SetValues[fieldName] = statement;
            return this;
        }

        /// <summary>
        /// Set fied value with given value
        /// </summary>
        /// <param name="fieldName">Target field name</param>
        /// <param name="value">Value</param>
        /// <returns>self</returns>
        public UpdateCommand Set(string fieldName, object value)
        {
            this.SetValues[fieldName] = $"@{this.ParamBinder.Add(value)}";
            return this;
        }

        public override bool Execute(NpgsqlConnection connection = null)
        {
            NpgsqlCommand command = this.PrepareCommand(connection);
            return this.ExecuteCommand(command);
        }

        public override string GenerateQuery()
        {
            if (this.SetValues.Count == 0)
            {
                throw new PgQueryUpdateSetsNothingException();
            }

            string setStatement = String.Join(", ", this.SetValues.Select(setValue => $"{setValue.Key} = {setValue.Value}"));
            return $"UPDATE {this.Table} SET {setStatement}{this.BuildConditionStatement()}";
        }
    }
}
