using Npgsql;

namespace PgQuery
{
    /// <summary>
    /// Record deleting command
    /// </summary>
    public class DeleteCommand : SqlConditionBuilder<DeleteCommand>
    {
        string Table;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tableName">Target table name</param>
        public DeleteCommand(string tableName) : base()
        {
            this.Table = tableName;
        }

        /// <summary>
        /// Generate SQL Query
        /// </summary>
        /// <returns>SQL String</returns>
        public override string GenerateQuery()
        {
            return $"DELETE FROM {this.Table}{this.BuildConditionStatement()}";
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
    }
}
