using System;
using System.Collections.Generic;
using System.Linq;

namespace PgQuery
{
    public enum MultiValueOperator
    {
        In, NotIn
    }

    /// <summary>
    /// Conditional statement having more than one comparing values
    ///     (IN, NOT IN)
    /// </summary>
    public class MultiValueCondition : ConditionStatement
    {
        public MultiValueOperator Operator = MultiValueOperator.In;
        public IEnumerable<int> ParamIndices;

        private int Length;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="values">Values</param>
        /// <param name="paramBinder">Parameter binder</param>
        public MultiValueCondition(string fieldName, IEnumerable<object> values,
            ParameterBinder paramBinder) : base()
        {
            this.FieldName = fieldName;
            List<int> indices = new List<int>();
            foreach (object value in values)
            {
                indices.Add(paramBinder.Add(value));
            }
            this.ParamIndices = indices;
            this.Length = indices.Count;
        }

        public override string GenerateQuery()
        {
            if (this.Length == 0)
            {
                return "true";
            }

            string operatorString = this.Operator == MultiValueOperator.In ? "IN" : "NOT IN";
            string bindedParams = String.Join(", ", this.ParamIndices.Select(index => $"@{index}"));
            string statement = $"{this.FieldName} {operatorString} ({bindedParams})";
            return this.Negated ? $"NOT({statement})" : statement;
        }
    }
}
