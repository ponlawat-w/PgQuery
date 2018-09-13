using System;
using System.Collections.Generic;
using System.Text;

namespace PgQuery
{
    /// <summary>
    /// Custom Statement
    /// </summary>
    public class CustomStatement : ConditionStatement
    {
        string Statement;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="statement">Custom SQL statement</param>
        public CustomStatement(string statement) : base()
        {
            this.Statement = statement;
        }

        /// <summary>
        /// Generate SQL query
        /// </summary>
        /// <returns>SQL string</returns>
        public override string GenerateQuery()
        {
            return this.Negated ? $"NOT({this.Statement})" : this.Statement;
        }
    }
}
