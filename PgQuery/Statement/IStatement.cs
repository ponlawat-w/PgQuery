namespace PgQuery
{
    /// <summary>
    /// SQL conditional or logical statement
    ///     Which provides logical true or false in SQL context
    /// </summary>
    public interface IStatement
    {
        /// <summary>
        /// Convert current statement to SQL command string
        /// </summary>
        /// <returns>SQL string of statement</returns>
        string GenerateQuery();
    }
}
