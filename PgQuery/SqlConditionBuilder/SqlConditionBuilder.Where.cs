using System.Collections.Generic;

namespace PgQuery
{
    public abstract partial class SqlConditionBuilder : SqlBuilder
    {
        public IStatement WhereStatement = null;

        /// <summary>
        /// Add a conditional or logical statement
        /// </summary>
        /// <param name="statement">Statement</param>
        /// <returns>self</returns>
        public SqlConditionBuilder AddStatement(IStatement statement)
        {
            if (this.WhereStatement is LogicalStatement)
            {
                ((this.WhereStatement as LogicalStatement).Statements as List<IStatement>).Add(statement);
            }
            else if (this.WhereStatement is ConditionStatement)
            {
                IStatement currentStatement = this.WhereStatement;
                LogicalStatement logicStatement = new LogicalStatement();
                logicStatement.Statements = new List<IStatement> { currentStatement, statement };
                this.WhereStatement = logicStatement;
            }
            else
            {
                this.WhereStatement = statement;
            }

            return this;
        }

        /// <summary>
        /// Returns true if containing statements
        /// </summary>
        /// <returns>Whether having statements or not</returns>
        public bool HasStatement()
        {
            return this.WhereStatement != null;
        }

        /// <summary>
        /// Build sql where statement
        /// </summary>
        /// <returns>SQL statement in WHERE part</returns>
        public string BuildWhereStatement()
        {
            return this.HasStatement() ? $" WHERE {this.WhereStatement.GenerateQuery()}" : "";
        }
    }
}
