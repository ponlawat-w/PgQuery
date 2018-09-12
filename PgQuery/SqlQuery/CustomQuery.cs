using Npgsql;

namespace PgQuery
{

    public class CustomQuery: CustomCommand
    {
        /// <summary>
        /// Custom SQL query (expecting result records)
        /// </summary>
        /// <param name="sql">SQL command</param>
        public CustomQuery(string sql): base(sql)
        {
        }

        public override bool Execute(NpgsqlConnection connection = null)
        {
            return base.ExecuteForReader(connection);
        }
    }
}
