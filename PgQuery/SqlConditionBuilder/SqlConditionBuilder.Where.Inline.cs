using System;
using System.Collections.Generic;
using System.Text;

namespace PgQuery
{
    public abstract partial class SqlConditionBuilder : SqlBuilder
    {
        /// <summary>
        /// Append a comparing statement (with AND logic to existing conditional statement)
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Comparing value</param>
        /// <param name="whereOperator">Operator (default is equal)</param>
        /// <param name="negated">Negate the statement (default is false)</param>
        /// <returns>self</returns>
        public SqlConditionBuilder Where(
            string fieldName,
            object value,
            SingleValueOperator whereOperator = SingleValueOperator.Equal,
            bool negated = false
        )
        {
            return this.AddStatement(this.CreateWhereCondition(fieldName, value, whereOperator, negated));
        }

        /// <summary>
        /// Append a null-comparing statement (with AND logic to existing conditional statement)
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <returns>self</returns>
        public SqlConditionBuilder WhereNull(string fieldName)
        {
            return this.AddStatement(this.CreateNullCondition(fieldName));
        }

        /// <summary>
        /// Append a not-null-comparing statement (with AND logic to existing conditional statement)
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <returns>self</returns>
        public SqlConditionBuilder WhereNotNull(string fieldName)
        {
            return this.AddStatement(this.CreateNotNullCondition(fieldName));
        }

        /// <summary>
        /// Append an IN statement (with AND logic to existing conditional statement)
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="values">Values</param>
        /// <returns>self</returns>
        public SqlConditionBuilder WhereIn(string fieldName, IEnumerable<object> values)
        {
            return this.AddStatement(this.CreateInCondition(fieldName, values));
        }

        /// <summary>
        /// Append a NOT IN statement (with AND logic to existing conditional statement)
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="values">Values</param>
        /// <returns>self</returns>
        public SqlConditionBuilder WhereNotIn(string fieldName, IEnumerable<object> values)
        {
            return this.AddStatement(this.CreateNotInCondition(fieldName, values));
        }

        /// <summary>
        /// Append a BETWEEN statement (with AND logic to existing conditional statement)
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="lower">Lower bound</param>
        /// <param name="upper">Upper bound</param>
        /// <returns>self</returns>
        public SqlConditionBuilder WhereBetween(string fieldName, object lower, object upper)
        {
            return this.AddStatement(this.CreateBetweenCondition(fieldName, lower, upper));
        }

        /// <summary>
        /// Append a NOT BETWEEN statement (with AND logic to existing conditional statement)
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="lower">Lower bound</param>
        /// <param name="upper">Upper bound</param>
        /// <returns>self</returns>
        public SqlConditionBuilder WhereNotBetween(string fieldName, object lower, object upper)
        {
            return this.AddStatement(this.CreateNotBetweenCondition(fieldName, lower, upper));
        }

        /// <summary>
        /// Append an AND logical statement (with AND logic to existing conditional statement)
        /// </summary>
        /// <param name="statements">Statement instances</param>
        /// <returns>self</returns>
        public SqlConditionBuilder And(params IStatement[] statements)
        {
            return this.AddStatement(new LogicAnd(statements));
        }

        /// <summary>
        /// Append an OR logical statement (with AND logic to existing conditional statement)
        /// </summary>
        /// <param name="statements">Statement instances</param>
        /// <returns>self</returns>
        public SqlConditionBuilder Or(params IStatement[] statements)
        {
            return this.AddStatement(new LogicOr(statements));
        }
    }
}
