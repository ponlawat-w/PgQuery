namespace PgQuery
{
    /// <summary>
    /// End-level conditional statement
    ///     Which means the statement provides true/false in SQL context
    ///     without containing sub-logical symbols (and/or)
    /// </summary>
    public abstract class ConditionStatement: IStatement
    {
        /// <summary>
        /// Field name to check
        /// </summary>
        public string FieldName;

        /// <summary>
        /// If true, the statement will be negated
        /// </summary>
        public bool Negated = false;

        /// <summary>
        /// Convert current statement to SQL command string
        /// </summary>
        /// <returns>SQL string of statement</returns>
        public abstract string GenerateQuery();
    }
}
