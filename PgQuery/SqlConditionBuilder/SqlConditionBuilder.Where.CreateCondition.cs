using System.Collections.Generic;

namespace PgQuery
{
    public abstract partial class SqlConditionBuilder : SqlBuilder
    {
        /// <summary>
        /// Create an instance of where condition, forked from current object
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <param name="whereOperator">Comparing operator (default is equal)</param>
        /// <param name="negated">Negated statement (default is false)</param>
        /// <returns>Condition Instance</returns>
        public SingleValueCondition CreateWhereCondition(
            string fieldName,
            object value,
            SingleValueOperator whereOperator = SingleValueOperator.Equal,
            bool negated = false
        )
        {
            return new SingleValueCondition(fieldName, value, this.ParamBinder)
            {
                Operator = whereOperator,
                Negated = negated
            };
        }

        /// <summary>
        /// Create an instance of null condition, forked from current object
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <returns>Condition Instance</returns>
        public NullCondition CreateNullCondition(string fieldName)
        {
            return new NullCondition(fieldName);
        }

        /// <summary>
        /// Create an instance of not-null condition, forked from current object
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <returns>Condition Instance</returns>
        public NullCondition CreateNotNullCondition(string fieldName)
        {
            return new NullCondition(fieldName)
            {
                Operator = NullOperator.IsNotNull
            };
        }

        /// <summary>
        /// Create an instance of IN condition, forked from current object
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="values">Values</param>
        /// <returns>Condition Instance</returns>
        public MultiValueCondition CreateInCondition(string fieldName, IEnumerable<object> values)
        {
            return new MultiValueCondition(fieldName, values, this.ParamBinder);
        }

        /// <summary>
        /// Create an instance of NOT IN condition, forked from current object
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="values">Values</param>
        /// <returns>Condition Instance</returns>
        public MultiValueCondition CreateNotInCondition(string fieldName, IEnumerable<object> values)
        {
            return new MultiValueCondition(fieldName, values, this.ParamBinder)
            {
                Operator = MultiValueOperator.NotIn
            };
        }

        /// <summary>
        /// Create an instance of BETWEEN condition, forked from current object
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="lower">Lower bound</param>
        /// <param name="upper">Upper bound</param>
        /// <returns>Condition Instance</returns>
        public BetweenCondition CreateBetweenCondition(string fieldName, object lower, object upper)
        {
            return new BetweenCondition(fieldName, lower, upper, this.ParamBinder);
        }

        /// <summary>
        /// Create an instance of NOT BETWEEN condition, forked from current object
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="lower">Lower bound</param>
        /// <param name="upper">Uppder bound</param>
        /// <returns>Condition Instance</returns>
        public BetweenCondition CreateNotBetweenCondition(string fieldName, object lower, object upper)
        {
            return new BetweenCondition(fieldName, lower, upper, this.ParamBinder)
            {
                Operator = BetweenOperator.NotBetween
            };
        }
    }
}
