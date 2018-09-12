using System;
using System.Collections.Generic;
using System.Text;

namespace PgQuery
{
    public abstract partial class SqlConditionBuilder : SqlBuilder
    {
        int Offset = -1;
        int Amount = -1;

        /// <summary>
        /// Limit result amount
        /// </summary>
        /// <param name="amount">Maximum result amount</param>
        /// <returns>self</returns>
        public SqlConditionBuilder Limit(int amount)
        {
            this.Offset = -1;
            this.Amount = amount;
            return this;
        }

        /// <summary>
        /// Limit result amount which begins with given offset index
        /// </summary>
        /// <param name="offset">Offset index</param>
        /// <param name="amount">Maximum result amount starting first row with offset index</param>
        /// <returns>self</returns>
        public SqlConditionBuilder LimitOffset(int offset, int amount)
        {
            this.Offset = offset;
            this.Amount = amount;
            return this;
        }

        /// <summary>
        /// Build limit SQL statement
        /// </summary>
        /// <returns>SQL statement in LIMIT part</returns>
        public string BuildLimitStatement()
        {
            if (this.Offset < 0 && this.Amount < 0)
            {
                return "";
            }

            if (this.Offset < 0)
            {
                return $" LIMIT {this.Amount}";
            }
            return $" LIMIT {this.Offset}, {this.Amount}";
        }
    }
}
