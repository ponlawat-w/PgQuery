namespace PgQuery
{
    public abstract partial class SqlConditionBuilder : SqlBuilder
    {
        int OffsetParamIndex = -1;
        int LimitParamIndex = -1;

        /// <summary>
        /// Limit result amount
        /// </summary>
        /// <param name="amount">Maximum result amount</param>
        /// <returns>self</returns>
        public SqlConditionBuilder Limit(int amount)
        {
            this.LimitParamIndex = this.ParamBinder.Add(amount);
            return this;
        }

        /// <summary>
        /// Limit result amount which begins with given offset index
        /// </summary>
        /// <param name="offset">Offset index</param>
        /// <returns>self</returns>
        public SqlConditionBuilder Offset(int offset)
        {
            this.OffsetParamIndex = this.ParamBinder.Add(offset);
            return this;
        }

        /// <summary>
        /// Build limit SQL statement
        /// </summary>
        /// <returns>SQL statement in LIMIT part</returns>
        public string BuildLimitStatement()
        {
            string statement = "";

            if (this.LimitParamIndex > -1)
            {
                statement += $" LIMIT @{this.LimitParamIndex}";
            }
            if (this.OffsetParamIndex > -1)
            {
                statement += $" OFFSET @{this.OffsetParamIndex}";
            }

            return statement;
        }
    }
}
