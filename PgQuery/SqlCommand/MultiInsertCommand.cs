using System;
using System.Collections.Generic;
using System.Linq;
using Npgsql;

namespace PgQuery
{
    /// <summary>
    /// An exception when trying to insert values which do not match with given fields
    /// </summary>
    public class PgQueryInsertFieldValueNotMatchException : Exception
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public PgQueryInsertFieldValueNotMatchException() : base("Fields and values do not match")
        {
        }
    }

    /// <summary>
    /// Multi-record inserting command
    /// </summary>
    public class MultiInsertCommand : SqlBuilder
    {
        private string Table;
        private string ReturningField = null;
        private string[] Fields;
        private List<int[]> Values;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tableName">Target table name</param>
        /// <param name="fields">Target fields</param>
        public MultiInsertCommand (string tableName, params string[] fields)
        {
            this.Table = tableName;
            this.Fields = fields;
            this.Values = new List<int[]>();
        }

        /// <summary>
        /// Set fields
        /// </summary>
        /// <param name="fields">Target field names</param>
        /// <returns>self</returns>
        public MultiInsertCommand IntoFields(params string[] fields)
        {
            this.Fields = fields;
            return this;
        }

        /// <summary>
        /// Set returning field
        /// </summary>
        /// <param name="returningField">Returning field name</param>
        /// <returns>self</returns>
        public MultiInsertCommand Returning(string returningField)
        {
            this.ReturningField = returningField;
            return this;
        }

        /// <summary>
        /// Add a record to statement
        /// </summary>
        /// <param name="values">Values to be inserted, corresponds with pre-configured field names</param>
        /// <returns>self</returns>
        public MultiInsertCommand AddRecord(params object[] values)
        {
            if (this.Fields.Length != values.Length)
            {
                throw new PgQueryInsertFieldValueNotMatchException();
            }

            this.Values.Add(values.Select(value => this.ParamBinder.Add(value)).ToArray());
            return this;
        }

        /// <summary>
        /// Generate SQL Query
        /// </summary>
        /// <returns>SQL string</returns>
        public override string GenerateQuery()
        {
            if (this.Fields.Length == 0 || this.Values.Count == 0)
            {
                throw new PgQueryInsertNoValuesException();
            }

            string fieldStatement = String.Join(", ", this.Fields);
            string valueStatement = String.Join(", ", this.Values.Select(
                paramIndices => "(" + String.Join(", ", paramIndices.Select(index => $"@{index}")) + ")"
            ));
            string returningStatement = this.ReturningField == null ? "" : $" RETURNING {this.ReturningField}";

            return $"INSERT INTO {this.Table} ({fieldStatement}) VALUES {valueStatement}{returningStatement}";
        }

        /// <summary>
        /// Execute command
        /// </summary>
        /// <param name="connection">Connection</param>
        /// <returns>Success or not</returns>
        public override bool Execute(NpgsqlConnection connection = null)
        {
            return this.ReturningField == null ?
                this.ExecuteCommand(this.PrepareCommand(connection))
                    :
                this.ExecuteReader(this.PrepareCommand(connection));
        }
    }
}
