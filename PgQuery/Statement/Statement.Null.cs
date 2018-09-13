using System;
using System.Collections.Generic;
using System.Text;

namespace PgQuery
{
    /// <summary>
    /// Operator when operand is null pointer
    /// </summary>
    public enum NullOperator
    {
        /// <summary>
        /// IS NULL
        /// </summary>
        IsNull,
        /// <summary>
        /// IS NOT NULL
        /// </summary>
        IsNotNull
    }

    /// <summary>
    /// Conditional statement which comparing value to null pointer
    /// </summary>
    public class NullCondition: ConditionStatement
    {
        public NullOperator Operator = NullOperator.IsNull;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fieldName">Field name</param>
        public NullCondition(string fieldName)
        {
            this.FieldName = fieldName;
        }

        /// <summary>
        /// Generate SQL Query
        /// </summary>
        /// <returns>SQL string</returns>
        public override string GenerateQuery()
        {
            string statement = this.FieldName +
                ((this.Operator == NullOperator.IsNull) ? " IS NULL" : " IS NOT NULL");
            return this.Negated ? $"NOT({statement})" : statement;
        }
    }
}
