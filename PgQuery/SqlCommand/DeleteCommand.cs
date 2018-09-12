using Npgsql;

namespace PgQuery
{
    /// <summary>
    /// Record deleting command
    /// </summary>
    public class DeleteCommand : SqlConditionBuilder
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

        public override string GenerateQuery()
        {
            return $"DELETE FROM {this.Table}{this.BuildConditionStatement()}";
        }

        public override bool Execute(NpgsqlConnection connection = null)
        {
            return this.ExecuteCommand(this.PrepareCommand(connection));
        }
    }
}
