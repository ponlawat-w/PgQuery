using Npgsql;

namespace PgQuery
{
    /// <summary>
    /// Custom SQL command expecting results
    /// </summary>
    public class CustomQuery: CustomCommand
    {
        /// <summary>
        /// Custom SQL query (expecting result records)
        /// </summary>
        /// <param name="sql">SQL command</param>
        public CustomQuery(string sql): base(sql)
        {
        }

        /// <summary>
        /// Execute data reader
        /// </summary>
        /// <param name="connection">Connection</param>
        /// <returns>Success or not</returns>
        public override bool Execute(NpgsqlConnection connection = null)
        {
            return base.ExecuteForReader(connection);
        }
    }
}
