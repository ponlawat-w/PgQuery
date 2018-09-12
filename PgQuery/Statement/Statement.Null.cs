using System;
using System.Collections.Generic;
using System.Text;

namespace PgQuery
{
    public enum NullOperator
    {
        IsNull, IsNotNull
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

        public override string GenerateQuery()
        {
            string statement = this.FieldName +
                ((this.Operator == NullOperator.IsNull) ? " IS NULL" : " IS NOT NULL");
            return this.Negated ? $"NOT({statement})" : statement;
        }
    }
}
