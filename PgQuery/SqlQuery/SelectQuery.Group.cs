using System.Collections.Generic;

namespace PgQuery
{
    public partial class SelectQuery : SqlConditionBuilder<SelectQuery>
    {
        private string GroupByField = null;
        private IStatement HavingStatement = null;

        /// <summary>
        /// Set field name after group by statement
        /// </summary>
        /// <param name="fieldName">field name</param>
        /// <returns>self</returns>
        public SelectQuery GroupBy(string fieldName)
        {
            this.GroupByField = fieldName;
            return this;
        }

        /// <summary>
        /// Add HAVING condition with statement object, if already exists, join the current one(s) with AND boolean operator
        /// </summary>
        /// <param name="statement">Statement object</param>
        /// <returns>self</returns>
        public SelectQuery AddHavingStatement(IStatement statement)
        {
            if (this.HavingStatement is LogicalStatement)
            {
                ((this.HavingStatement as LogicalStatement).Statements as List<IStatement>)
                    .Add(statement);
            }
            else if (this.HavingStatement is ConditionStatement)
            {
                IStatement currentStatement = this.HavingStatement;
                this.HavingStatement = new LogicalStatement
                {
                    Statements = new List<IStatement> { currentStatement, statement },
                    Operator = LogicOperator.And
                };
            }
            else
            {
                this.HavingStatement = statement;
            }

            return this;
        }

        /// <summary>
        /// Add HAVING condition, if already exists, join the current one(s) with AND boolean operator
        /// </summary>
        /// <param name="fieldName">Field name to be compared</param>
        /// <param name="value">Value to be compared</param>
        /// <param name="op">Operator</param>
        /// <param name="negated">Statement negation marker</param>
        /// <returns>self</returns>
        public SelectQuery Having(
            string fieldName,
            object value,
            SingleValueOperator op = SingleValueOperator.Equal,
            bool negated = false
        )
        {
            return this.AddHavingStatement(
                this.CreateWhereCondition(fieldName, value, op, negated)
            );
        }

        /// <summary>
        /// Add custom HAVING condition, if already exists, join the current one(s) this AND boolean operator
        /// </summary>
        /// <param name="statement">Statement string</param>
        /// <returns>self</returns>
        public SelectQuery HavingCustom(string statement)
        {
            return this.AddHavingStatement(
                this.CreateCustomStatementCondition(statement)
            );
        }

        /// <summary>
        /// Build SQL statement in HAVING part
        /// </summary>
        /// <returns>SQL string</returns>
        public string BuildHavingStatement()
        {
            return this.HavingStatement == null ? "" : $" HAVING {this.HavingStatement.GenerateQuery()}";
        }

        /// <summary>
        /// Build SQL statement in GROUP BY and HAVING part
        /// </summary>
        /// <returns>SQL string</returns>
        public string BuildGroupByStatement()
        {
            return this.GroupByField == null ? "" : $" GROUP BY {this.GroupByField}{this.BuildHavingStatement()}";
        }
    }
}
