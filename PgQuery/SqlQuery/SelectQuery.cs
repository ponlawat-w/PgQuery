using System;
using System.Collections.Generic;
using System.Linq;
using Npgsql;

namespace PgQuery
{
    /// <summary>
    /// Query of selecting records from table
    /// </summary>
    public partial class SelectQuery : SqlConditionBuilder
    {
        string Table;
        string TableAlias = null;
        string[] Fields;

        string TableName
        {
            get => this.TableAlias == null ? this.Table : this.TableAlias;
        }

        string TableNameInFromStatement
        {
            get => this.Table + (this.TableAlias == null ? "" : $" {this.TableAlias}");
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tableName">Source table name</param>
        /// <param name="fields">Fields to select</param>
        public SelectQuery(string tableName, params string[] fields) : base()
        {
            this.Table = tableName;
            this.Fields = fields;
        }

        /// <summary>
        /// Constructor (selecting all fields)
        /// </summary>
        /// <param name="tableName">Source table name</param>
        public SelectQuery(string tableName) : this(tableName, new string[] { "*" })
        {
        }

        /// <summary>
        /// Alias current table name
        /// </summary>
        /// <param name="tableAlias">Alias name</param>
        /// <returns>self</returns>
        public SelectQuery Alias(string tableAlias)
        {
            this.TableAlias = tableAlias;
            return this;
        }

        /// <summary>
        /// Set selecting fields
        /// </summary>
        /// <param name="fields">Selecting fields</param>
        /// <returns>self</returns>
        public SelectQuery Select(params string[] fields)
        {
            this.Fields = fields;
            return this;
        }

        /// <summary>
        /// Execute data reader
        /// </summary>
        /// <param name="connection">Conenction</param>
        /// <returns>Success or not</returns>
        public override bool Execute(NpgsqlConnection connection = null)
        {
            NpgsqlCommand command = this.PrepareCommand(connection);
            return this.ExecuteReader(command);
        }

        /// <summary>
        /// Generate SQL Command
        /// </summary>
        /// <returns>SQL string</returns>
        public override string GenerateQuery()
        {
            string fieldsQuery = String.Join(", ", this.Fields);
            return $"SELECT {fieldsQuery} FROM {this.TableNameInFromStatement}{this.BuildJoinStatement()}{this.BuildConditionStatement()}";
        }
    }
}
